import { Component } from '@angular/core';
import { AuthFeatureCard } from './auth-feature-card/auth-feature-card';
import { AuthFormField } from './auth-form-field/auth-form-field';
import { BrandMark } from './brand-mark/brand-mark';

@Component({
  selector: 'app-auth',
  imports: [AuthFeatureCard, AuthFormField, BrandMark],
  templateUrl: './auth.html',
  styleUrl: './auth.css',
})
export class Auth {}
