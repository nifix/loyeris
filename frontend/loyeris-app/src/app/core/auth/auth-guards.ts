import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthSession } from './auth-session';

export const authGuard: CanActivateFn = () => {
  const session = inject(AuthSession);
  return session.authenticated() ? true : inject(Router).createUrlTree(['/login']);
};

export const guestGuard: CanActivateFn = () => {
  const session = inject(AuthSession);
  return session.authenticated() ? inject(Router).createUrlTree(['/dashboard']) : true;
};
