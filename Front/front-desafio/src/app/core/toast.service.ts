import { Injectable, signal } from '@angular/core';

export type TipoToast = 'sucesso' | 'erro';

export interface Toast {
  id: number;
  mensagem: string;
  tipo: TipoToast;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private proximoId = 0;
  readonly toasts = signal<Toast[]>([]);

  sucesso(mensagem: string) {
    this.exibir(mensagem, 'sucesso');
  }

  erro(mensagem: string) {
    this.exibir(mensagem, 'erro');
  }

  remover(id: number) {
    this.toasts.update((toasts) => toasts.filter((t) => t.id !== id));
  }

  private exibir(mensagem: string, tipo: TipoToast) {
    const id = ++this.proximoId;
    this.toasts.update((toasts) => [...toasts, { id, mensagem, tipo }]);
    setTimeout(() => this.remover(id), 5000);
  }
}
