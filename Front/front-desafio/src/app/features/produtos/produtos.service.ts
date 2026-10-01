import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { DadosProduto, Produto } from './produto.model';

@Injectable({ providedIn: 'root' })
export class ProdutosService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/produtos`;

  listar() {
    return this.http.get<Produto[]>(this.url);
  }

  obter(id: string) {
    return this.http.get<Produto>(`${this.url}/${id}`);
  }

  criar(produto: DadosProduto) {
    return this.http.post<Produto>(this.url, produto);
  }

  atualizar(id: string, produto: DadosProduto) {
    return this.http.put<Produto>(`${this.url}/${id}`, produto);
  }
}
