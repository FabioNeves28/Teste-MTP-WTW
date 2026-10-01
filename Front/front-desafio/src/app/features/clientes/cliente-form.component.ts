import { Component, OnInit, inject, input } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { aplicarErrosDoServidor } from '../../core/problem-details';
import { ToastService } from '../../core/toast.service';
import { ErroCampoComponent } from '../../shared/erro-campo.component';
import { ClientesService } from './clientes.service';

@Component({
  selector: 'app-cliente-form',
  imports: [ReactiveFormsModule, RouterLink, ErroCampoComponent],
  templateUrl: './cliente-form.component.html',
})
export class ClienteFormComponent implements OnInit {
  readonly id = input<string>();

  private readonly clientesService = inject(ClientesService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  protected readonly form = inject(NonNullableFormBuilder).group({
    nome: ['', [Validators.required, Validators.maxLength(150)]],
    email: ['', [Validators.required, Validators.email]],
    documento: ['', [Validators.required, Validators.pattern(/^(\D*\d){11}(\D*\d{3})?\D*$/)]],
  });

  ngOnInit() {
    const id = this.id();
    if (id) {
      this.clientesService.obter(id).subscribe((cliente) => this.form.patchValue(cliente));
    }
  }

  salvar() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const id = this.id();
    const dados = this.form.getRawValue();
    const requisicao = id
      ? this.clientesService.atualizar(id, dados)
      : this.clientesService.criar(dados);

    requisicao.subscribe({
      next: () => {
        this.toast.sucesso('Cliente salvo.');
        this.router.navigate(['/clientes']);
      },
      error: (erro) => aplicarErrosDoServidor(this.form, erro),
    });
  }
}
