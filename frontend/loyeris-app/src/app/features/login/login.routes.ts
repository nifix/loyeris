import { Routes } from '@angular/router';

import { LoginPage } from './pages/login-page/login-page';
import { RegisterPage } from './pages/register-page/register-page';
import { EmailVerificationPage } from './pages/email-verification-page/email-verification-page';
import { ForgotPasswordPage } from './pages/forgot-password-page/forgot-password-page';
import { ResetPasswordPage } from './pages/reset-password-page/reset-password-page';

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

const forgotPasswordRoutes: Routes = [
  {
    path: '',
    component: ForgotPasswordPage,
  },
];

const resetPasswordRoutes: Routes = [
  {
    path: '',
    component: ResetPasswordPage,
  },
];

export {
  emailVerificationRoutes,
  forgotPasswordRoutes,
  loginRoutes,
  registerRoutes,
  resetPasswordRoutes,
};
