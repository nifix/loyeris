import { Component } from '@angular/core';
import { AuthFeatureCard } from '../../ui-components/auth-feature-card/auth-feature-card';
import { AuthFormField } from '../../ui-components/auth-form-field/auth-form-field';
import { BrandMark } from '../../ui-components/brand-mark/brand-mark';

@Component({
  selector: 'app-auth',
  imports: [AuthFeatureCard, AuthFormField, BrandMark],
  templateUrl: './auth.html',
  styleUrl: './auth.css',
})

export class Auth {}
