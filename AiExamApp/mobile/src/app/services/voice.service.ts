import { Injectable } from '@angular/core';
import { Capacitor } from '@capacitor/core';
import { SpeechRecognition } from '@capacitor-community/speech-recognition';
import { KeepAwake } from '@capacitor-community/keep-awake';

export type ListeningState = 'idle' | 'listening' | 'processing';

interface WebSpeechResult {
  transcript: string;
  isFinal: boolean;
}

interface WebSpeechRecognition {
  lang: string;
  continuous: boolean;
  interimResults: boolean;
  maxAlternatives: number;
  onresult: ((event: { results: ArrayLike<ArrayLike<WebSpeechResult>> }) => void) | null;
  onend: (() => void) | null;
  onerror: ((event: { error: string }) => void) | null;
  start(): void;
  stop(): void;
  abort(): void;
}

declare global {
  interface Window {
    webkitSpeechRecognition?: new () => WebSpeechRecognition;
  }
}

@Injectable({ providedIn: 'root' })
export class VoiceService {
  private state: ListeningState = 'idle';
  private finalText: string[] = [];
  private webRecognition: WebSpeechRecognition | null = null;
  private onWebFinal: ((text: string) => void) | null = null;
  private listeners: Array<() => void> = [];

  constructor() {}

  getState(): ListeningState {
    return this.state;
  }

  setState(next: ListeningState): void {
    this.state = next;
  }

  async init(): Promise<void> {
    if (Capacitor.isNativePlatform()) {
      await KeepAwake.keepAwake();
      const available = await SpeechRecognition.available();
      if (!available.available) throw new Error('Reconocimiento de voz no disponible');
    } else {
      if (!window.webkitSpeechRecognition) {
        throw new Error('Tu navegador no soporta reconocimiento de voz. Usa Chrome/Edge.');
      }
      try {
        await KeepAwake.keepAwake();
      } catch {
        // Wake Lock puede no estar soportado en todos los navegadores; no es fatal.
      }
    }
  }

  async dispose(): Promise<void> {
    try {
      await KeepAwake.allowSleep();
    } catch {
      // ignorar
    }
    this.listeners.forEach((remove) => void remove());
    this.listeners = [];
  }

  async requestPermission(): Promise<boolean> {
    if (!Capacitor.isNativePlatform()) return true;
    const status = await SpeechRecognition.checkPermissions();
    if (status.speechRecognition === 'granted') return true;
    const request = await SpeechRecognition.requestPermissions();
    return request.speechRecognition === 'granted';
  }

  async start(onPartial: (text: string) => void, onFinal: (text: string) => void): Promise<void> {
    this.finalText = [];
    this.state = 'listening';
    if (Capacitor.isNativePlatform()) {
      await this.startNative(onPartial, onFinal);
    } else {
      this.startWeb(onPartial, onFinal);
    }
  }

  async stop(): Promise<void> {
    try {
      if (Capacitor.isNativePlatform()) {
        await SpeechRecognition.stop();
      } else {
        this.webRecognition?.stop();
      }
    } finally {
      this.state = 'idle';
    }
  }

  getFullTranscript(): string {
    return this.finalText.join(' ').trim();
  }

  private async startNative(onPartial: (text: string) => void, onFinal: (text: string) => void): Promise<void> {
    await SpeechRecognition.start({
      language: 'es-ES',
      maxResults: 1,
      partialResults: true,
      popup: false,
    });

    const listener = await SpeechRecognition.addListener('partialResults', (data: { matches: string[] }) => {
      const partial = data.matches?.[0] ?? '';
      if (partial) {
        this.finalText.push(partial);
        onPartial(this.getFullTranscript());
        onFinal(this.getFullTranscript());
      }
    });
    this.listeners.push(() => void listener.remove());
  }

  private startWeb(onPartial: (text: string) => void, onFinal: (text: string) => void): void {
    const Recognition = window.webkitSpeechRecognition!;
    const rec = new Recognition();
    rec.lang = 'es-ES';
    rec.continuous = true;
    rec.interimResults = true;
    rec.maxAlternatives = 1;

    rec.onresult = (event) => {
      let interim = '';
      const full: string[] = [];
      for (let i = 0; i < event.results.length; i++) {
        const result = event.results[i][0];
        if (!result) continue;
        if (result.isFinal) {
          full.push(result.transcript);
        } else {
          interim += result.transcript;
        }
      }
      if (full.length) {
        this.finalText.push(...full);
        onPartial(this.finalText.join(' ') + (interim ? ` ${interim}` : ''));
        onFinal(this.finalText.join(' '));
      } else if (interim) {
        onPartial(interim);
      }
    };

    rec.onend = () => {
      this.state = 'idle';
    };

    rec.onerror = (event) => {
      if (event.error !== 'aborted') {
        this.state = 'idle';
      }
    };

    this.onWebFinal = onFinal;
    this.webRecognition = rec;
    rec.start();
  }
}