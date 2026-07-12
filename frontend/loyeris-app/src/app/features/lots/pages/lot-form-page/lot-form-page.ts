import { HttpErrorResponse } from '@angular/common/http';
import { NgTemplateOutlet } from '@angular/common';
import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize, forkJoin, map, of, switchMap, take } from 'rxjs';

import { AppShell } from '../../../../core/layouts/app-shell/app-shell';
import { PageHeader } from '../../../../shared/components/ui-page-header/page-header';
import { type Sci, SciApi } from '../../../scis/services/sci-api';
import { LotApi, type Lot, type LotType, type SaveLotRequest } from '../../services/lot-api';
import {
  type AvailableTenant,
  LotOccupancyApi,
  type LotOccupancy,
  type SaveLotOccupancyRequest,
  type UpdateLotLeaseRequest,
} from '../../services/lot-occupancy-api';

interface ProblemDetails {
  errorCode?: string;
}

type LotFormError =
  | 'duplicate-reference'
  | 'tenant-unavailable'
  | 'archived-occupied'
  | 'invalid-lease-dates'
  | 'lease-overlap'
  | 'tenant-required'
  | 'forbidden'
  | 'assignment'
  | 'generic';

const decimalPattern = /^\d+(?:[.,]\d{1,2})?$/;
type LeaseEditorMode = 'closed' | 'assign' | 'edit';
type LeaseEditorError = 'invalid' | 'overlap' | 'forbidden' | 'generic';

@Component({
  selector: 'app-lot-form-page',
  imports: [AppShell, NgTemplateOutlet, PageHeader, ReactiveFormsModule, RouterLink],
  templateUrl: './lot-form-page.html',
  styleUrl: './lot-form-page.css',
})
export class LotFormPage implements OnInit {
  readonly presentationLot = input<Lot | null>(null);
  readonly presentationScis = input<readonly Sci[] | null>(null);
  readonly presentationTenants = input<readonly AvailableTenant[] | null>(null);
  readonly presentationOccupancy = input<LotOccupancy | null>(null);
  readonly presentationLeases = input<readonly LotOccupancy[] | null>(null);
  readonly submitting = input(false);

  private readonly formBuilder = inject(FormBuilder);
  private readonly lotApi = inject(LotApi);
  private readonly occupancyApi = inject(LotOccupancyApi);
  private readonly sciApi = inject(SciApi);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly routeLotId = this.route.snapshot.paramMap.get('lotId');
  private readonly persistedLotId = signal<string | null>(null);
  private readonly persistedLot = signal<Lot | null>(null);
  private readonly loadedScis = signal<readonly Sci[]>([]);
  private readonly loadedTenants = signal<readonly AvailableTenant[]>([]);
  private readonly loadedLeases = signal<readonly LotOccupancy[]>([]);
  private readonly localLoading = signal(false);
  private readonly localLoadError = signal(false);
  private readonly localSubmitting = signal(false);
  private readonly submitted = signal(false);
  private readonly localError = signal<LotFormError | null>(null);
  private readonly localLeaseEditorMode = signal<LeaseEditorMode>('closed');
  private readonly editingLeaseId = signal<string | null>(null);
  private readonly localLeaseSubmitting = signal(false);
  private readonly localLeaseError = signal<LeaseEditorError | null>(null);
  private readonly localLeaseMessage = signal<'saved' | 'deleted' | null>(null);
  private readonly confirmingLeaseDeletion = signal(false);
  private readonly localEditMode = signal(true);
  private readonly lotSaved = signal(false);

  protected readonly today = new Date().toISOString().slice(0, 10);
  protected readonly editingId = computed(
    () => this.persistedLotId() ?? this.presentationLot()?.id ?? this.routeLotId,
  );
  protected readonly isEditing = computed(() => this.editingId() !== null);
  protected readonly editMode = this.localEditMode.asReadonly();
  protected readonly readOnly = computed(() => this.isEditing() && !this.editMode());
  protected readonly isLoading = this.localLoading.asReadonly();
  protected readonly loadError = this.localLoadError.asReadonly();
  protected readonly isSubmitting = computed(() => this.submitting() || this.localSubmitting());
  protected readonly errorCode = this.localError.asReadonly();
  protected readonly scis = computed(() => this.presentationScis() ?? this.loadedScis());
  protected readonly tenants = computed(
    () => this.presentationTenants() ?? this.loadedTenants(),
  );
  protected readonly assignmentTenants = computed(() =>
    this.tenants().filter((tenant) => !tenant.isCurrent),
  );
  protected readonly leases = computed(
    () => this.presentationLeases() ?? this.loadedLeases(),
  );
  protected readonly leaseEditorMode = this.localLeaseEditorMode.asReadonly();
  protected readonly leaseEditorOpen = computed(() => this.leaseEditorMode() !== 'closed');
  protected readonly editingLease = computed(() =>
    this.leases().find((lease) => lease.leaseId === this.editingLeaseId()),
  );
  protected readonly leaseSubmitting = this.localLeaseSubmitting.asReadonly();
  protected readonly leaseError = this.localLeaseError.asReadonly();
  protected readonly leaseMessage = this.localLeaseMessage.asReadonly();
  protected readonly confirmingDeletion = this.confirmingLeaseDeletion.asReadonly();
  protected readonly savedMessage = this.lotSaved.asReadonly();
  protected readonly headerTitle = computed(() =>
    !this.isEditing() ? 'Ajouter un lot' : this.readOnly() ? 'Détails du lot' : 'Modifier le lot',
  );
  protected readonly headerDescription = computed(() =>
    !this.isEditing()
      ? 'Créez la fiche du bien et affectez-lui éventuellement un locataire disponible.'
      : this.readOnly()
        ? 'Consultez les caractéristiques du bien et son historique locatif.'
        : 'Mettez à jour le bien, son potentiel locatif et son occupation actuelle.',
  );
  protected readonly form = this.formBuilder.nonNullable.group({
    sciId: ['', Validators.required],
    reference: ['', [Validators.required, Validators.maxLength(80)]],
    type: ['T2' as LotType, Validators.required],
    active: [true],
    street: ['', [Validators.required, Validators.maxLength(180)]],
    postalCode: ['', [Validators.required, Validators.maxLength(20)]],
    city: ['', [Validators.required, Validators.maxLength(120)]],
    country: ['FR' as const, Validators.required],
    surfaceSqm: ['', Validators.pattern(decimalPattern)],
    potentialRent: ['0', [Validators.required, Validators.pattern(decimalPattern)]],
    potentialCharges: ['0', [Validators.required, Validators.pattern(decimalPattern)]],
    suggestedDeposit: ['0', [Validators.required, Validators.pattern(decimalPattern)]],
    notes: [''],
    tenantId: [''],
    leaseStartsOn: [this.today, Validators.required],
    leaseEndsOn: [''],
    rentDueDay: [5, [Validators.required, Validators.min(1), Validators.max(28)]],
    leaseRent: ['0', [Validators.required, Validators.pattern(decimalPattern)]],
    leaseCharges: ['0', [Validators.required, Validators.pattern(decimalPattern)]],
    leaseDeposit: ['0', [Validators.required, Validators.pattern(decimalPattern)]],
    paymentTerms: [''],
    leaseNotes: [''],
  });

  ngOnInit(): void {
    if (this.presentationLot()) {
      this.loadedScis.set(this.presentationScis() ?? []);
      this.loadedTenants.set(this.presentationTenants() ?? []);
      this.loadedLeases.set(
        this.presentationLeases()
        ?? (this.presentationOccupancy() ? [this.presentationOccupancy()!] : []),
      );
      this.populate(this.presentationLot()!);
      this.enterReadOnlyMode();
      return;
    }
    if (this.presentationScis() !== null) {
      this.loadedScis.set(this.presentationScis() ?? []);
      this.loadedTenants.set(this.presentationTenants() ?? []);
      const firstSci = this.presentationScis()?.[0];
      if (firstSci) {
        this.form.controls.sciId.setValue(firstSci.id);
      }
      return;
    }
    this.load();
  }

  protected load(): void {
    this.localLoading.set(true);
    this.localLoadError.set(false);
    const lotId = this.routeLotId;
    const request = lotId
      ? forkJoin({
          lot: this.lotApi.get(lotId),
          scis: this.sciApi.list(),
          tenants: this.occupancyApi.listAvailableTenants(lotId),
          leases: this.occupancyApi.listLeases(lotId),
        })
      : forkJoin({
          lot: [null as Lot | null],
          scis: this.sciApi.list(),
          tenants: this.occupancyApi.listAvailableTenants(),
          leases: [[] as readonly LotOccupancy[]],
        });

    request
      .pipe(
        take(1),
        finalize(() => this.localLoading.set(false)),
      )
      .subscribe({
        next: ({ lot, scis, tenants, leases }) => {
          this.loadedScis.set(scis);
          this.loadedTenants.set(tenants);
          this.loadedLeases.set(leases);
          if (lot) {
            this.populate(lot);
            this.enterReadOnlyMode();
          } else if (scis.length > 0) {
            this.form.controls.sciId.setValue(scis.find((sci) => sci.status === 'Active')?.id ?? scis[0].id);
          }
        },
        error: () => this.localLoadError.set(true),
      });
  }

  protected tenantChanged(): void {
    if (!this.form.controls.tenantId.value) {
      return;
    }
    if (this.moneyToCents(this.form.controls.leaseRent.value) === 0) {
      this.form.controls.leaseRent.setValue(this.form.controls.potentialRent.value);
      this.form.controls.leaseCharges.setValue(this.form.controls.potentialCharges.value);
      this.form.controls.leaseDeposit.setValue(this.form.controls.suggestedDeposit.value);
    }
  }

  protected openAssignment(): void {
    this.localLeaseError.set(null);
    this.localLeaseMessage.set(null);
    this.confirmingLeaseDeletion.set(false);
    this.localLeaseEditorMode.set('assign');
    this.editingLeaseId.set(null);
    this.form.patchValue({
      tenantId: '',
      leaseStartsOn: this.today,
      leaseEndsOn: '',
      rentDueDay: 5,
      leaseRent: this.form.controls.potentialRent.value,
      leaseCharges: this.form.controls.potentialCharges.value,
      leaseDeposit: this.form.controls.suggestedDeposit.value,
      paymentTerms: '',
      leaseNotes: '',
    });
  }

  protected enableEditing(): void {
    this.form.enable();
    this.localEditMode.set(true);
    this.lotSaved.set(false);
  }

  protected cancelEditing(): void {
    const lot = this.persistedLot();
    if (lot) {
      this.populate(lot);
    }
    this.closeLeaseEditor();
    this.enterReadOnlyMode();
  }

  protected editLease(lease: LotOccupancy): void {
    this.localLeaseError.set(null);
    this.localLeaseMessage.set(null);
    this.confirmingLeaseDeletion.set(false);
    this.localLeaseEditorMode.set('edit');
    this.editingLeaseId.set(lease.leaseId);
    this.form.patchValue({
      tenantId: lease.tenantId,
      leaseStartsOn: lease.startsOn,
      leaseEndsOn: lease.endsOn ?? '',
      rentDueDay: lease.rentDueDay,
      leaseRent: this.centsToMoney(lease.rentExcludingChargesCents),
      leaseCharges: this.centsToMoney(lease.chargesCents),
      leaseDeposit: this.centsToMoney(lease.depositCents),
      paymentTerms: lease.paymentTerms ?? '',
      leaseNotes: lease.notes ?? '',
    });
  }

  protected closeLeaseEditor(): void {
    this.localLeaseEditorMode.set('closed');
    this.editingLeaseId.set(null);
    this.form.controls.tenantId.setValue('');
    this.localLeaseError.set(null);
    this.confirmingLeaseDeletion.set(false);
  }

  protected saveLease(): void {
    const lotId = this.editingId();
    if (!lotId || this.localLeaseSubmitting() || !this.validateLeaseEditor()) {
      return;
    }

    const value = this.form.getRawValue();
    const operation = this.leaseEditorMode() === 'edit' && this.editingLeaseId()
      ? this.occupancyApi.updateLease(
          lotId,
          this.editingLeaseId()!,
          this.buildLeaseRequest(value),
        )
      : this.occupancyApi.save(lotId, this.buildOccupancyRequest(value));

    this.localLeaseSubmitting.set(true);
    this.localLeaseError.set(null);
    operation
      .pipe(
        switchMap(() => forkJoin({
          leases: this.occupancyApi.listLeases(lotId),
          tenants: this.occupancyApi.listAvailableTenants(lotId),
        })),
        take(1),
        finalize(() => this.localLeaseSubmitting.set(false)),
      )
      .subscribe({
        next: ({ leases, tenants }) => {
          this.loadedLeases.set(leases);
          this.loadedTenants.set(tenants);
          this.closeLeaseEditor();
          this.localLeaseMessage.set('saved');
        },
        error: (error: HttpErrorResponse) => this.handleLeaseError(error),
      });
  }

  protected requestLeaseDeletion(): void {
    this.confirmingLeaseDeletion.set(true);
  }

  protected cancelLeaseDeletion(): void {
    this.confirmingLeaseDeletion.set(false);
  }

  protected deleteLease(): void {
    const lotId = this.editingId();
    const leaseId = this.editingLeaseId();
    if (!lotId || !leaseId || this.localLeaseSubmitting()) {
      return;
    }

    this.localLeaseSubmitting.set(true);
    this.localLeaseError.set(null);
    this.occupancyApi.deleteLease(lotId, leaseId)
      .pipe(
        switchMap(() => forkJoin({
          leases: this.occupancyApi.listLeases(lotId),
          tenants: this.occupancyApi.listAvailableTenants(lotId),
        })),
        take(1),
        finalize(() => this.localLeaseSubmitting.set(false)),
      )
      .subscribe({
        next: ({ leases, tenants }) => {
          this.loadedLeases.set(leases);
          this.loadedTenants.set(tenants);
          this.closeLeaseEditor();
          this.localLeaseMessage.set('deleted');
        },
        error: (error: HttpErrorResponse) => this.handleLeaseError(error),
      });
  }

  protected leasePeriod(lease: LotOccupancy): string {
    return `${this.dateLabel(lease.startsOn)} — ${lease.endsOn ? this.dateLabel(lease.endsOn) : 'En cours'}`;
  }

  protected leaseAmount(lease: LotOccupancy): string {
    return this.currency(lease.rentExcludingChargesCents + lease.chargesCents);
  }

  protected leaseBreakdown(lease: LotOccupancy): string {
    return `${this.currency(lease.rentExcludingChargesCents)} HC + ${this.currency(lease.chargesCents)} de charges`;
  }

  protected leaseStatus(lease: LotOccupancy): string {
    if (lease.startsOn > this.today) {
      return 'À venir';
    }
    return lease.endsOn && lease.endsOn < this.today ? 'Terminé' : 'En cours';
  }

  protected fieldInvalid(name: keyof typeof this.form.controls): boolean {
    const control = this.form.controls[name];
    return control.invalid && (control.touched || this.submitted());
  }

  protected submit(): void {
    if (this.isSubmitting()) {
      return;
    }
    this.submitted.set(true);
    this.localError.set(null);
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      return;
    }

    const value = this.form.getRawValue();
    if (this.leaseEditorMode() === 'assign' && !value.tenantId) {
      this.localError.set('tenant-required');
      return;
    }
    if (!value.active
        && (this.hasCurrentLease() || this.leaseEditorMode() === 'assign')) {
      this.localError.set('archived-occupied');
      return;
    }
    if (this.leaseEditorOpen()
        && value.leaseEndsOn
        && (value.leaseEndsOn < value.leaseStartsOn
          || this.leaseEditorMode() === 'assign' && value.leaseEndsOn < this.today)) {
      this.localError.set('invalid-lease-dates');
      return;
    }

    const lotRequest: SaveLotRequest = {
      sciId: value.sciId,
      reference: value.reference.trim(),
      type: value.type,
      status: value.active ? 'Active' : 'Archived',
      street: value.street.trim(),
      postalCode: value.postalCode.trim(),
      city: value.city.trim(),
      country: value.country,
      surfaceSqm: value.surfaceSqm ? this.decimal(value.surfaceSqm) : null,
      potentialRentExcludingChargesCents: this.moneyToCents(value.potentialRent),
      potentialChargesCents: this.moneyToCents(value.potentialCharges),
      suggestedDepositCents: this.moneyToCents(value.suggestedDeposit),
      notes: value.notes.trim() || null,
    };
    const wasEditing = this.isEditing();
    const currentId = this.editingId();

    this.localSubmitting.set(true);
    (currentId ? this.lotApi.update(currentId, lotRequest) : this.lotApi.create(lotRequest))
      .pipe(
        switchMap((lot) => {
          this.persistedLotId.set(lot.id);
          if (!currentId && this.leaseEditorMode() === 'assign') {
            return this.occupancyApi.save(lot.id, this.buildOccupancyRequest(value))
              .pipe(map(() => lot));
          }
          return of(lot);
        }),
        take(1),
        finalize(() => this.localSubmitting.set(false)),
      )
      .subscribe({
        next: (lot) => {
          if (wasEditing) {
            this.populate(lot);
            this.enterReadOnlyMode();
            this.lotSaved.set(true);
            return;
          }
          void this.router.navigate(['/lots'], { queryParams: { created: '1' } });
        },
        error: (error: HttpErrorResponse) => this.handleError(error, !currentId && this.persistedLotId() !== null),
      });
  }

  private populate(lot: Lot): void {
    this.persistedLot.set(lot);
    this.form.reset({
      sciId: lot.sciId,
      reference: lot.reference,
      type: lot.type,
      active: lot.status === 'Active',
      street: lot.street,
      postalCode: lot.postalCode,
      city: lot.city,
      country: lot.country as 'FR',
      surfaceSqm: lot.surfaceSqm?.toString().replace('.', ',') ?? '',
      potentialRent: this.centsToMoney(lot.potentialRentExcludingChargesCents),
      potentialCharges: this.centsToMoney(lot.potentialChargesCents),
      suggestedDeposit: this.centsToMoney(lot.suggestedDepositCents),
      notes: lot.notes ?? '',
      tenantId: '',
      leaseStartsOn: this.today,
      leaseEndsOn: '',
      rentDueDay: 5,
      leaseRent: '0,00',
      leaseCharges: '0,00',
      leaseDeposit: '0,00',
      paymentTerms: '',
      leaseNotes: '',
    });
  }

  private enterReadOnlyMode(): void {
    this.localEditMode.set(false);
    this.form.disable();
  }

  private handleError(error: HttpErrorResponse, assignmentFailedAfterCreation: boolean): void {
    if (assignmentFailedAfterCreation) {
      this.localError.set('assignment');
      return;
    }
    const code = (error.error as ProblemDetails | null)?.errorCode;
    this.localError.set(
      code === 'portfolio.lot.reference_already_exists'
        ? 'duplicate-reference'
        : code === 'leasing.occupancy.tenant_unavailable'
          ? 'tenant-unavailable'
        : code === 'leasing.lease.overlap'
          ? 'lease-overlap'
          : code === 'portfolio.lot.active_occupancy'
              || code === 'leasing.occupancy.archived_lot'
            ? 'archived-occupied'
          : error.status === 403
            ? 'forbidden'
            : 'generic',
    );
  }

  private validateLeaseEditor(): boolean {
    const controls = [
      this.form.controls.leaseStartsOn,
      this.form.controls.leaseEndsOn,
      this.form.controls.rentDueDay,
      this.form.controls.leaseRent,
      this.form.controls.leaseCharges,
      this.form.controls.leaseDeposit,
    ];
    controls.forEach((control) => control.markAsTouched());
    const value = this.form.getRawValue();
    const tenantMissing = this.leaseEditorMode() === 'assign' && !value.tenantId;
    const invalidDates = Boolean(
      value.leaseEndsOn
      && (value.leaseEndsOn < value.leaseStartsOn
        || this.leaseEditorMode() === 'assign' && value.leaseEndsOn < this.today),
    );
    if (controls.some((control) => control.invalid) || tenantMissing || invalidDates) {
      this.localLeaseError.set('invalid');
      return false;
    }
    return true;
  }

  private buildOccupancyRequest(
    value: ReturnType<typeof this.form.getRawValue>,
  ): SaveLotOccupancyRequest {
    return {
      tenantId: value.tenantId || null,
      startsOn: value.tenantId ? value.leaseStartsOn : null,
      endsOn: value.tenantId && value.leaseEndsOn ? value.leaseEndsOn : null,
      rentDueDay: value.rentDueDay,
      rentExcludingChargesCents: this.moneyToCents(value.leaseRent),
      chargesCents: this.moneyToCents(value.leaseCharges),
      depositCents: this.moneyToCents(value.leaseDeposit),
      paymentTerms: value.paymentTerms.trim() || null,
      notes: value.leaseNotes.trim() || null,
    };
  }

  private buildLeaseRequest(
    value: ReturnType<typeof this.form.getRawValue>,
  ): UpdateLotLeaseRequest {
    return {
      startsOn: value.leaseStartsOn,
      endsOn: value.leaseEndsOn || null,
      rentDueDay: value.rentDueDay,
      rentExcludingChargesCents: this.moneyToCents(value.leaseRent),
      chargesCents: this.moneyToCents(value.leaseCharges),
      depositCents: this.moneyToCents(value.leaseDeposit),
      paymentTerms: value.paymentTerms.trim() || null,
      notes: value.leaseNotes.trim() || null,
    };
  }

  private handleLeaseError(error: HttpErrorResponse): void {
    const code = (error.error as ProblemDetails | null)?.errorCode;
    this.localLeaseError.set(
      code === 'leasing.lease.overlap'
        ? 'overlap'
        : error.status === 403
          ? 'forbidden'
          : code === 'leasing.lease.invalid'
            ? 'invalid'
            : 'generic',
    );
  }

  private moneyToCents(value: string): number {
    return Math.round(this.decimal(value) * 100);
  }

  private decimal(value: string): number {
    return Number(value.replace(',', '.'));
  }

  private centsToMoney(cents: number): string {
    return (cents / 100).toFixed(2).replace('.', ',');
  }

  private hasCurrentLease(): boolean {
    return this.leases().some((lease) =>
      lease.startsOn <= this.today && (!lease.endsOn || lease.endsOn >= this.today),
    );
  }

  private dateLabel(value: string): string {
    const [year, month, day] = value.split('-');
    return `${day}/${month}/${year}`;
  }

  private currency(cents: number): string {
    return new Intl.NumberFormat('fr-FR', {
      style: 'currency',
      currency: 'EUR',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(cents / 100);
  }
}
