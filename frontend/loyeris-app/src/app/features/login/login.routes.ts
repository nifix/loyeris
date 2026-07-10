import { Routes } from '@angular/router';

import { LoginPage } from './pages/login-page/login-page';
import { RegisterPage } from './pages/register-page/register-page';
import { EmailVerificationPage } from './pages/email-verification-page/email-verification-page';

const loginRoutes: Routes = [
  {
    path: '',
    component: LoginPage,
  },
];

const registerRoutes: Routes = [
  {
    path: '',
    component: RegisterPage,
  },
];

const emailVerificationRoutes: Routes = [
  {
    path: '',
    component: EmailVerificationPage,
  },
];

export { emailVerificationRoutes, loginRoutes, registerRoutes };
