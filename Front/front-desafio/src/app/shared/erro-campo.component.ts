import { Component, computed, input } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { AbstractControl, ValidationErrors } from '@angular/forms';
import { map, startWith, switchMap } from 'rxjs';

@Component({
  selector: 'app-erro-campo',
  template: `
    @if (mensagem(); as mensagem) {
      <div class="invalid-feedback d-block">{{ mensagem }}</div>
    }
  `,
})
export class ErroCampoComponent {
  readonly controle = input.required<AbstractControl>();
  readonly mensagemFormato = input('Formato inválido.');

  private readonly estado = toSignal(
    toObservable(this.controle).pipe(
      switchMap((controle) =>
        controle.events.pipe(
          startWith(null),
          map(() => ({ visivel: controle.invalid && controle.touched, erros: controle.errors })),
        ),
      ),
    ),
  );

  protected readonly mensagem = computed(() => {
    const estado = this.estado();
    return estado?.visivel ? traduzir(estado.erros ?? {}, this.mensagemFormato()) : null;
  });
}

function traduzir(erros: ValidationErrors, mensagemFormato: string): string {
  if (erros['servidor']) return erros['servidor'];
  if (erros['estoque']) return erros['estoque'];
  if (erros['required']) return 'Campo obrigatório.';
  if (erros['min']) return `Valor mínimo: ${erros['min'].min}.`;
  if (erros['maxlength']) return `Máximo de ${erros['maxlength'].requiredLength} caracteres.`;
  if (erros['pattern']) return mensagemFormato;
  return 'Valor inválido.';
}
