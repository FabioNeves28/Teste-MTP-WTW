import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { AcaoPedido, NovoPedido, Pedido, StatusPedido } from './pedido.model';

@Injectable({ providedIn: 'root' })
export class PedidosService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/pedidos`;

  listar(status?: StatusPedido) {
    const params = status ? new HttpParams().set('status', status) : undefined;
    return this.http.get<Pedido[]>(this.url, { params });
  }

  criar(pedido: NovoPedido) {
    return this.http.post<Pedido>(this.url, pedido);
  }

  executar(id: string, acao: AcaoPedido) {
    return this.http.post<Pedido>(`${this.url}/${id}/${acao}`, null);
  }
}
