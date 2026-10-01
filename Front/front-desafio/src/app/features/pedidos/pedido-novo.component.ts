import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { aplicarErrosDoServidor } from '../../core/problem-details';
import { ToastService } from '../../core/toast.service';
import { ErroCampoComponent } from '../../shared/erro-campo.component';
import { MoedaPipe } from '../../shared/moeda.pipe';
import { ClientesService } from '../clientes/clientes.service';
import { Produto } from '../produtos/produto.model';
import { ProdutosService } from '../produtos/produtos.service';
import { PedidosService } from './pedidos.service';

interface ItemCarrinho {
  produto: Produto;
  quantidade: number;
}

@Component({
  selector: 'app-pedido-novo',
  imports: [ReactiveFormsModule, RouterLink, ErroCampoComponent, MoedaPipe],
  templateUrl: './pedido-novo.component.html',
})
export class PedidoNovoComponent {
  private readonly pedidosService = inject(PedidosService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  private readonly fb = inject(NonNullableFormBuilder);

  protected readonly clientes = toSignal(inject(ClientesService).listar(), { initialValue: [] });
  protected readonly produtos = toSignal(inject(ProdutosService).listar(), { initialValue: [] });

  protected readonly form = this.fb.group({
    clienteId: ['', Validators.required],
  });

  protected readonly novoItem = this.fb.group({
    produtoId: ['', Validators.required],
    quantidade: [1, [Validators.required, Validators.min(1)]],
  });

  protected readonly itens = signal<ItemCarrinho[]>([]);

  protected readonly total = computed(() =>
    this.itens().reduce((soma, item) => soma + item.produto.preco * item.quantidade, 0),
  );

  adicionarItem() {
    const { produtoId, quantidade } = this.novoItem.getRawValue();
    const produto = this.produtos().find((p) => p.id === produtoId);

    if (this.form.invalid || this.novoItem.invalid || !produto) {
      this.form.markAllAsTouched();
      this.novoItem.markAllAsTouched();
      return;
    }

    const existente = this.itens().find((i) => i.produto.id === produtoId);
    const quantidadeTotal = (existente?.quantidade ?? 0) + quantidade;

    if (quantidadeTotal > produto.quantidadeEstoque) {
      const disponivel = produto.quantidadeEstoque - (existente?.quantidade ?? 0);
      this.novoItem.controls.quantidade.setErrors({
        estoque: `Máximo disponível: ${disponivel}.`,
      });
      this.novoItem.controls.quantidade.markAsTouched();
      return;
    }

    this.itens.update((itens) => {
      return existente
        ? itens.map((i) => (i === existente ? { ...i, quantidade: i.quantidade + quantidade } : i))
        : [...itens, { produto, quantidade }];
    });

    this.novoItem.reset();
  }

  removerItem(produtoId: string) {
    this.itens.update((itens) => itens.filter((i) => i.produto.id !== produtoId));
  }

  salvar() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.itens().length === 0) {
      this.toast.erro('Adicione ao menos um item ao pedido.');
      return;
    }

    const pedido = {
      clienteId: this.form.getRawValue().clienteId,
      itens: this.itens().map((i) => ({ produtoId: i.produto.id, quantidade: i.quantidade })),
    };

    this.pedidosService.criar(pedido).subscribe({
      next: () => {
        this.toast.sucesso('Pedido criado.');
        this.router.navigate(['/pedidos']);
      },
      error: (erro) => aplicarErrosDoServidor(this.form, erro),
    });
  }
}
