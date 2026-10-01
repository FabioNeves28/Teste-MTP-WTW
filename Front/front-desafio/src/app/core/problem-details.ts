import { HttpErrorResponse } from '@angular/common/http';
import { FormGroup } from '@angular/forms';

export interface ProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
  errors?: Record<string, string[]>;
}

export function mensagemDoErro(erro: HttpErrorResponse): string {
  if (erro.status === 0) {
    return 'Não foi possível conectar à API.';
  }

  const problem = erro.error as ProblemDetails | null;
  return problem?.detail ?? problem?.title ?? 'Ocorreu um erro inesperado.';
}

export function aplicarErrosDoServidor(form: FormGroup, erro: unknown) {
  if (!(erro instanceof HttpErrorResponse) || erro.status !== 400) {
    return;
  }

  const errors = (erro.error as ProblemDetails | null)?.errors ?? {};

  for (const [campo, mensagens] of Object.entries(errors)) {
    form.get(campo)?.setErrors({ servidor: mensagens.join(' ') });
  }

  form.markAllAsTouched();
}
