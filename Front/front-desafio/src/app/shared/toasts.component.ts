import { Component, inject } from '@angular/core';
import { ToastService } from '../core/toast.service';

@Component({
  selector: 'app-toasts',
  template: `
    <div class="toast-container position-fixed top-0 end-0 p-3">
      @for (toast of toastService.toasts(); track toast.id) {
        <div
          class="toast show align-items-center text-white border-0"
          [class.bg-success]="toast.tipo === 'sucesso'"
          [class.bg-danger]="toast.tipo === 'erro'"
          role="alert"
        >
          <div class="d-flex">
            <div class="toast-body">{{ toast.mensagem }}</div>
            <button
              type="button"
              class="btn-close btn-close-white me-2 m-auto"
              (click)="toastService.remover(toast.id)"
            ></button>
          </div>
        </div>
      }
    </div>
  `,
})
export class ToastsComponent {
  protected readonly toastService = inject(ToastService);
}
