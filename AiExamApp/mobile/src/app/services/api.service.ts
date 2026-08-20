import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

export interface ModelDefinition {
  id: string;
  name: string;
  free: boolean;
}

export interface ModelsResponse {
  default: string;
  models: ModelDefinition[];
}

export interface AskResponse {
  answer: string;
  model: string;
  elapsedMs: number;
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  readonly baseUrl: string;

  constructor(private readonly http: HttpClient) {
    this.baseUrl = this.resolveBaseUrl();
  }

  private resolveBaseUrl(): string {
    const configured = (window as unknown as { AI_EXAM_API_URL?: string }).AI_EXAM_API_URL;
    if (configured) return configured;
    return 'http://localhost:5000/api';
  }

  async getModels(): Promise<ModelsResponse> {
    return firstValueFrom(this.http.get<ModelsResponse>(`${this.baseUrl}/models`));
  }

  async ask(question: string, model?: string): Promise<AskResponse> {
    return firstValueFrom(
      this.http.post<AskResponse>(`${this.baseUrl}/ask`, { question, model }),
    );
  }
}