import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { mensagemDoErro } from './problem-details';
import { ToastService } from './toast.service';

export const erroHttpInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((erro: HttpErrorResponse) => {
      toast.erro(mensagemDoErro(erro));
      return throwError(() => erro);
    }),
  );
};
