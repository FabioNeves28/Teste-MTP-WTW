import { Component, OnInit, inject, input } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { aplicarErrosDoServidor } from '../../core/problem-details';
import { ToastService } from '../../core/toast.service';
import { ErroCampoComponent } from '../../shared/erro-campo.component';
import { ProdutosService } from './produtos.service';

@Component({
  selector: 'app-produto-form',
  imports: [ReactiveFormsModule, RouterLink, ErroCampoComponent],
  templateUrl: './produto-form.component.html',
})
export class ProdutoFormComponent implements OnInit {
  readonly id = input<string>();

  private readonly produtosService = inject(ProdutosService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  protected readonly form = inject(NonNullableFormBuilder).group({
    nome: ['', [Validators.required, Validators.maxLength(150)]],
    descricao: ['', Validators.maxLength(1000)],
    preco: [0, [Validators.required, Validators.min(0)]],
    quantidadeEstoque: [0, [Validators.required, Validators.min(0)]],
  });

  ngOnInit() {
    const id = this.id();
    if (id) {
      this.produtosService.obter(id).subscribe((produto) => this.form.patchValue(produto));
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
      ? this.produtosService.atualizar(id, dados)
      : this.produtosService.criar(dados);

    requisicao.subscribe({
      next: () => {
        this.toast.sucesso('Produto salvo.');
        this.router.navigate(['/produtos']);
      },
      error: (erro) => aplicarErrosDoServidor(this.form, erro),
    });
  }
}
