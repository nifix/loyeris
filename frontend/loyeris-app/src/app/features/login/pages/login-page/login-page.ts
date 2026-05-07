import { Component } from '@angular/core';
import { FeatureCard } from '../../components/ui-feature-card/feature-card';
import { FormField } from '../../components/ui-form-field/form-field';
import { BrandMark } from '../../../../shared/components/ui-brand-mark/brand-mark';

@Component({
  selector: 'app-login-page',
  imports: [FeatureCard, FormField, BrandMark],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
})
export class LoginPage {}
