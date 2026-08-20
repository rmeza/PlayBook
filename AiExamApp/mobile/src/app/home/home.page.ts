import { Component, OnDestroy, OnInit } from '@angular/core';
import { TextToSpeech } from '@capacitor-community/text-to-speech';
import { addIcons } from 'ionicons';
import { mic, micOff } from 'ionicons/icons';
import { ApiService, type AskResponse, type ModelDefinition } from '../services/api.service';
import { VoiceService, type ListeningState } from '../services/voice.service';

const SILENCE_MS = 1200;
const MIN_QUESTION_LENGTH = 2;

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
  standalone: false,
})
export class HomePage implements OnInit, OnDestroy {
  models: ModelDefinition[] = [];
  defaultModel = '';
  selectedModel = '';
  transcript = '';
  answer: AskResponse | null = null;
  error: string | null = null;
  state: ListeningState = 'idle';
  ttsEnabled = false;
  autoSendEnabled = true;
  apiBaseUrl = '';
  private silenceTimer: ReturnType<typeof setTimeout> | null = null;
  private listeners: Array<() => void> = [];
  private lastProcessed = '';

  constructor(
    private readonly api: ApiService,
    private readonly voice: VoiceService,
  ) {
    addIcons({ mic, micOff });
  }

  get listening(): boolean {
    return this.state === 'listening';
  }

  get processing(): boolean {
    return this.state === 'processing';
  }

  async ngOnInit(): Promise<void> {
    this.apiBaseUrl = this.api.baseUrl;
    await this.loadModels();
    try {
      await this.voice.init();
      await this.startListening();
    } catch (err) {
      this.state = 'idle';
      this.error = err instanceof Error ? err.message : 'No se pudo iniciar el micrófono.';
    }
  }

  ngOnDestroy(): void {
    this.clearSilenceTimer();
    this.listeners.forEach((remove) => remove());
    void this.voice.dispose();
  }

  async loadModels(): Promise<void> {
    try {
      const res = await this.api.getModels();
      this.models = res.models;
      this.defaultModel = res.default;
      this.selectedModel = res.default;
    } catch {
      this.error = `No se pudo conectar al backend (${this.apiBaseUrl}).`;
    }
  }

  async startListening(): Promise<void> {
    this.error = null;
    const granted = await this.voice.requestPermission();
    if (!granted) {
      this.state = 'idle';
      this.error = 'Permiso de micrófono denegado.';
      return;
    }

    this.state = 'listening';
    await this.voice.start(
      (partial) => {
        this.transcript = partial;
        this.bumpSilenceTimer();
      },
      (final) => {
        this.transcript = final || this.transcript;
      },
    );

    if (this.autoSendEnabled) {
      this.scheduleAutoSend();
    }
  }

  async stopListening(): Promise<void> {
    this.clearSilenceTimer();
    await this.voice.stop();
    this.state = 'idle';
  }

  async toggleListening(): Promise<void> {
    if (this.listening) {
      await this.stopListening();
    } else {
      await this.startListening();
    }
  }

  async sendManual(): Promise<void> {
    const text = this.transcript.trim() || this.voice.getFullTranscript();
    if (text.length < MIN_QUESTION_LENGTH) {
      if (this.listening && this.autoSendEnabled) this.scheduleAutoSend();
      return;
    }
    this.clearSilenceTimer();
    await this.ask(text);
  }

  async changeModel(event: Event): Promise<void> {
    const value = (event as CustomEvent).detail?.value as string | undefined;
    if (value) this.selectedModel = value;
  }

  toggleTts(): void {
    this.ttsEnabled = !this.ttsEnabled;
    if (this.ttsEnabled && this.answer) {
      void this.speak(this.answer.answer);
    } else {
      void TextToSpeech.stop();
    }
  }

  toggleAutoSend(): void {
    this.autoSendEnabled = !this.autoSendEnabled;
    if (this.autoSendEnabled && this.listening) {
      this.scheduleAutoSend();
    } else {
      this.clearSilenceTimer();
    }
  }

  async speak(text: string): Promise<void> {
    await TextToSpeech.speak({
      text,
      lang: 'es-ES',
      rate: 1.0,
    });
  }

  private bumpSilenceTimer(): void {
    if (this.autoSendEnabled && this.listening) {
      this.scheduleAutoSend();
    }
  }

  private scheduleAutoSend(): void {
    this.clearSilenceTimer();
    this.silenceTimer = setTimeout(() => {
      void this.sendManual();
    }, SILENCE_MS);
  }

  private clearSilenceTimer(): void {
    if (this.silenceTimer) {
      clearTimeout(this.silenceTimer);
      this.silenceTimer = null;
    }
  }

  private async ask(question: string): Promise<void> {
    if (!question || question === this.lastProcessed) return;
    this.lastProcessed = question;
    this.state = 'processing';
    this.clearSilenceTimer();
    this.error = null;

    try {
      this.answer = await this.api.ask(question, this.selectedModel);
      if (this.ttsEnabled) {
        void this.speak(this.answer.answer);
      }
      this.transcript = '';
    } catch (err) {
      this.error = 'La API no respondió. Revisa el backend.';
    } finally {
      if (this.autoSendEnabled) {
        await this.startListening();
      } else {
        this.state = 'idle';
      }
    }
  }
}