import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { ToastService } from '../../core/toast.service';
import { MoedaPipe } from '../../shared/moeda.pipe';
import { AcaoPedido, Pedido, STATUS_PEDIDO, StatusPedido, acoesPermitidas } from './pedido.model';
import { PedidosService } from './pedidos.service';

const CLASSE_STATUS: Record<StatusPedido, string> = {
  Criado: 'text-bg-secondary',
  Confirmado: 'text-bg-primary',
  Cancelado: 'text-bg-danger',
  Finalizado: 'text-bg-success',
};

const ROTULO_ACAO: Record<AcaoPedido, string> = {
  confirmar: 'Confirmar',
  cancelar: 'Cancelar',
  finalizar: 'Finalizar',
};

@Component({
  selector: 'app-pedidos-lista',
  imports: [RouterLink, DatePipe, MoedaPipe],
  templateUrl: './pedidos-lista.component.html',
})
export class PedidosListaComponent {
  private readonly pedidosService = inject(PedidosService);
  private readonly toast = inject(ToastService);

  protected readonly statusDisponiveis = STATUS_PEDIDO;
  protected readonly classeStatus = CLASSE_STATUS;
  protected readonly rotuloAcao = ROTULO_ACAO;
  protected readonly acoesPermitidas = acoesPermitidas;

  protected readonly filtroStatus = signal<StatusPedido | undefined>(undefined);
  protected readonly processando = signal<string | null>(null);

  protected readonly pedidos = rxResource({
    params: () => ({ status: this.filtroStatus() }),
    stream: ({ params }) => this.pedidosService.listar(params.status),
    defaultValue: [],
  });

  filtrar(status: string) {
    this.filtroStatus.set((status || undefined) as StatusPedido | undefined);
  }

  executar(pedido: Pedido, acao: AcaoPedido) {
    this.processando.set(pedido.id);

    this.pedidosService.executar(pedido.id, acao).subscribe({
      next: (atualizado) => {
        this.toast.sucesso(`Pedido ${atualizado.status.toLowerCase()}.`);
        this.pedidos.reload();
        this.processando.set(null);
      },
      error: () => this.processando.set(null),
    });
  }
}
