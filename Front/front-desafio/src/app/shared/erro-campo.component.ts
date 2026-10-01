import { Component, input } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Component({
  selector: 'app-erro-campo',
  template: `
    @if (controle().invalid && controle().touched) {
      <div class="invalid-feedback d-block">{{ mensagem() }}</div>
    }
  `,
})
export class ErroCampoComponent {
  readonly controle = input.required<AbstractControl>();

  protected mensagem(): string {
    const erros = this.controle().errors ?? {};

    if (erros['servidor']) return erros['servidor'];
    if (erros['required']) return 'Campo obrigatório.';
    if (erros['email']) return 'E-mail inválido.';
    if (erros['min']) return `Valor mínimo: ${erros['min'].min}.`;
    if (erros['maxlength']) return `Máximo de ${erros['maxlength'].requiredLength} caracteres.`;
    if (erros['pattern']) return 'Formato inválido.';
    return 'Valor inválido.';
  }
}
