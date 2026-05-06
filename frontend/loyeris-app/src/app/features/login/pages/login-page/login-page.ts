import { Component } from '@angular/core';
import { BrandMark } from '../../components/brand-mark/brand-mark';
import { FeatureCard } from '../../components/feature-card/feature-card';
import { FormField } from '../../components/form-field/form-field';

@Component({
  selector: 'app-login-page',
  imports: [FeatureCard, FormField, BrandMark],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
})
export class LoginPage {}
