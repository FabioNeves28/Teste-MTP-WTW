import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { MoedaPipe } from '../../shared/moeda.pipe';
import { ProdutosService } from './produtos.service';

@Component({
  selector: 'app-produtos-lista',
  imports: [RouterLink, MoedaPipe],
  templateUrl: './produtos-lista.component.html',
})
export class ProdutosListaComponent {
  protected readonly produtos = toSignal(inject(ProdutosService).listar(), { initialValue: [] });
}
