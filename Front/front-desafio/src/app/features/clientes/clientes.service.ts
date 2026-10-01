import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Cliente, DadosCliente } from './cliente.model';

@Injectable({ providedIn: 'root' })
export class ClientesService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/clientes`;

  listar() {
    return this.http.get<Cliente[]>(this.url);
  }

  obter(id: string) {
    return this.http.get<Cliente>(`${this.url}/${id}`);
  }

  criar(cliente: DadosCliente) {
    return this.http.post<Cliente>(this.url, cliente);
  }

  atualizar(id: string, cliente: DadosCliente) {
    return this.http.put<Cliente>(`${this.url}/${id}`, cliente);
  }
}
