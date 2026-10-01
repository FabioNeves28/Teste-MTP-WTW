import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { ClientesService } from './clientes.service';

@Component({
  selector: 'app-clientes-lista',
  imports: [RouterLink],
  templateUrl: './clientes-lista.component.html',
})
export class ClientesListaComponent {
  protected readonly clientes = toSignal(inject(ClientesService).listar(), { initialValue: [] });
}
