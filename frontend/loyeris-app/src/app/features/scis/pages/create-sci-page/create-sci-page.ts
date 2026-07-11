import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize, take } from 'rxjs';

import { AppShell } from '../../../../core/layouts/app-shell/app-shell';
import { PageHeader } from '../../../../shared/components/ui-page-header/page-header';
import { type SaveSciRequest, type Sci, SciApi } from '../../services/sci-api';

interface ProblemDetails {
  errorCode?: string;
}

export type SciCreationErrorCode = 'duplicate-name' | 'duplicate-siren' | 'forbidden' | 'generic';

const trimmedRequired: ValidatorFn = (control: AbstractControl): ValidationErrors | null =>
  typeof control.value === 'string' && control.value.trim().length > 0 ? null : { required: true };

const notFutureDate: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  if (!control.value) {
    return null;
  }

  const today = new Date().toISOString().slice(0, 10);
  return control.value <= today ? null : { futureDate: true };
};

@Component({
  selector: 'app-create-sci-page',
  imports: [AppShell, PageHeader, ReactiveFormsModule, RouterLink],
  templateUrl: './create-sci-page.html',
  styleUrl: './create-sci-page.css',
})
export class CreateSciPage implements OnInit {
  readonly submissionErrorCode = input<SciCreationErrorCode | null>(null);
  readonly submitting = input(false);
  readonly editingSci = input<Sci | null>(null);

  private readonly api = inject(SciApi);
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly routeSciId = this.route.snapshot.paramMap.get('sciId');
  private readonly localSubmitting = signal(false);
  private readonly submitted = signal(false);
  private readonly apiErrorCode = signal<SciCreationErrorCode | null>(null);
  private readonly localLoading = signal(false);
  private readonly localLoadError = signal(false);

  protected readonly today = new Date().toISOString().slice(0, 10);
  protected readonly editingId = computed(() => this.editingSci()?.id ?? this.routeSciId);
  protected readonly isEditing = computed(() => this.editingId() !== null);
  protected readonly isLoading = computed(() => this.localLoading());
  protected readonly loadError = computed(() => this.localLoadError());
  protected readonly isSubmitting = computed(() => this.submitting() || this.localSubmitting());
  protected readonly errorCode = computed(() => this.apiErrorCode() ?? this.submissionErrorCode());
  protected readonly headerBadge = computed(() => this.isEditing() ? 'Fiche juridique' : 'Nouvelle structure');
  protected readonly headerTitle = computed(() => this.isEditing() ? 'Modifier la SCI' : 'Ajouter une SCI');
  protected readonly headerDescription = computed(() => this.isEditing()
    ? 'Mettez à jour les informations juridiques, le siège social et le statut de gestion de cette SCI.'
    : 'Créez la fiche juridique de votre SCI. Vous pourrez ensuite lui rattacher ses lots, ses associés et ses locataires.');
  protected readonly form = this.formBuilder.nonNullable.group({
    name: ['', [trimmedRequired, Validators.maxLength(180)]],
    siren: ['', Validators.pattern(/^\d{9}$/)],
    taxRegime: ['IR' as const, Validators.required],
    active: [true],
    street: ['', Validators.maxLength(180)],
    postalCode: ['', Validators.pattern(/^\d{5}$/)],
    city: ['', Validators.maxLength(120)],
    country: ['FR' as const, Validators.required],
    incorporatedOn: ['', notFutureDate],
  });

  ngOnInit(): void {
    const sci = this.editingSci();
    if (sci) {
      this.populateForm(sci);
      return;
    }

    if (this.routeSciId) {
      this.loadSci();
    }
  }

  protected loadSci(): void {
    const sciId = this.editingId();
    if (!sciId) {
      return;
    }

    this.localLoading.set(true);
    this.localLoadError.set(false);
    this.api
      .get(sciId)
      .pipe(
        take(1),
        finalize(() => this.localLoading.set(false)),
      )
      .subscribe({
        next: (sci) => this.populateForm(sci),
        error: () => this.localLoadError.set(true),
      });
  }

  protected fieldError(
    fieldName: 'name' | 'siren' | 'street' | 'postalCode' | 'city' | 'incorporatedOn',
  ): string | undefined {
    const control = this.form.controls[fieldName];
    if (!control.invalid || (!control.touched && !this.submitted())) {
      return undefined;
    }

    if (fieldName === 'name') {
      return control.hasError('required')
        ? 'Le nom de la SCI est obligatoire.'
        : 'Le nom ne peut pas dépasser 180 caractères.';
    }
    if (fieldName === 'siren') {
      return 'Le SIREN doit contenir exactement 9 chiffres.';
    }
    if (fieldName === 'postalCode') {
      return 'Saisissez un code postal français à 5 chiffres.';
    }
    if (fieldName === 'incorporatedOn') {
      return 'La date de constitution ne peut pas être future.';
    }

    return 'Cette valeur est trop longue.';
  }

  protected submit(): void {
    if (this.isSubmitting()) {
      return;
    }

    this.submitted.set(true);
    this.apiErrorCode.set(null);
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      return;
    }

    const value = this.form.getRawValue();
    const request: SaveSciRequest = {
      name: value.name.trim(),
      siren: value.siren || null,
      taxRegime: value.taxRegime,
      status: value.active ? 'Active' : 'Archived',
      street: value.street.trim() || null,
      postalCode: value.postalCode || null,
      city: value.city.trim() || null,
      country: value.country,
      incorporatedOn: value.incorporatedOn || null,
    };
    const sciId = this.editingId();
    this.localSubmitting.set(true);
    (sciId ? this.api.update(sciId, request) : this.api.create(request))
      .pipe(
        take(1),
        finalize(() => this.localSubmitting.set(false)),
      )
      .subscribe({
        next: () => void this.router.navigate(['/scis'], {
          queryParams: sciId ? { updated: '1' } : { created: '1' },
        }),
        error: (error: HttpErrorResponse) => {
          const errorCode = (error.error as ProblemDetails | null)?.errorCode;
          this.apiErrorCode.set(
            errorCode === 'portfolio.sci.name_already_exists'
              ? 'duplicate-name'
              : errorCode === 'portfolio.sci.siren_already_exists'
                ? 'duplicate-siren'
                : error.status === 403
                  ? 'forbidden'
                  : 'generic',
          );
        },
      });
  }

  private populateForm(sci: Sci): void {
    this.form.reset({
      name: sci.name,
      siren: sci.siren ?? '',
      taxRegime: sci.taxRegime,
      active: sci.status === 'Active',
      street: sci.street ?? '',
      postalCode: sci.postalCode ?? '',
      city: sci.city ?? '',
      country: sci.country as 'FR',
      incorporatedOn: sci.incorporatedOn ?? '',
    });
  }
}
