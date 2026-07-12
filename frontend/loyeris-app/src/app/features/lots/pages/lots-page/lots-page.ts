import { Component, computed, inject, input, OnDestroy, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin, take } from 'rxjs';

import { AppShell } from '../../../../core/layouts/app-shell/app-shell';
import { MetricCard } from '../../../../shared/components/ui-metric-card/metric-card';
import { PageHeader } from '../../../../shared/components/ui-page-header/page-header';
import {
  currentBusinessDate,
  expectedMonthlyReceiptsCents,
  leaseIncludesDate,
} from '../../../../shared/utils/lease-period';
import {
  currentLeaseForDate,
  monthlyRentalPotential,
  portfolioMonthlyRentalPotentialCents,
} from '../../../../shared/utils/rental-potential';
import { LotsTable, type LotListItem } from '../../components/ui-lots-table/lots-table';
import { LotApi, type Lot } from '../../services/lot-api';
import { LotOccupancyApi, type LotOccupancy } from '../../services/lot-occupancy-api';

export type LotListState = 'loading' | 'loaded' | 'error';

@Component({
  selector: 'app-lots-page',
  imports: [AppShell, LotsTable, MetricCard, PageHeader, RouterLink],
  templateUrl: './lots-page.html',
  styleUrl: './lots-page.css',
})
export class LotsPage implements OnInit, OnDestroy {
  readonly presentationLots = input<readonly Lot[] | null>(null);
  readonly presentationOccupancies = input<readonly LotOccupancy[] | null>(null);
  readonly presentationState = input<LotListState | null>(null);

  private readonly lotApi = inject(LotApi);
  private readonly occupancyApi = inject(LotOccupancyApi);
  private readonly route = inject(ActivatedRoute);
  private readonly loadedLots = signal<readonly Lot[]>([]);
  private readonly loadedOccupancies = signal<readonly LotOccupancy[]>([]);
  private readonly localState = signal<LotListState>('loading');
  private readonly successMessage = signal<'created' | 'updated' | null>(null);
  private successTimer: ReturnType<typeof setTimeout> | undefined;

  protected readonly state = computed(() => this.presentationState() ?? this.localState());
  protected readonly lots = computed(() =>
    [...(this.presentationLots() ?? this.loadedLots())].sort((first, second) => {
      const statusOrder = Number(first.status === 'Archived') - Number(second.status === 'Archived');
      return statusOrder || first.sciName.localeCompare(second.sciName, 'fr')
        || first.reference.localeCompare(second.reference, 'fr');
    }),
  );
  protected readonly occupancies = computed(
    () => this.presentationOccupancies() ?? this.loadedOccupancies(),
  );
  protected readonly message = this.successMessage.asReadonly();
  protected readonly headerChips = computed(() => {
    const lots = this.lots();
    const occupied = this.currentOccupiedLotIds(lots).size;
    return [
      `${lots.length} lot${lots.length > 1 ? 's' : ''} suivi${lots.length > 1 ? 's' : ''}`,
      `${occupied} occupé${occupied > 1 ? 's' : ''}`,
      `${this.currency(this.portfolioPotentialCents())} de potentiel mensuel`,
    ];
  });
  protected readonly metrics = computed(() => {
    const lots = this.lots();
    const activeLots = lots.filter((lot) => lot.status === 'Active');
    const occupied = this.currentOccupiedLotIds(lots).size;
    const vacant = activeLots.length - occupied;
    const occupancyRate = activeLots.length === 0
      ? 0
      : Math.round(occupied / activeLots.length * 100);
    const sciCount = new Set(lots.map((lot) => lot.sciId)).size;
    return [
      {
        title: 'Lots gérés',
        value: String(lots.length),
        description: `${sciCount} SCI`,
        tone: 'neutral' as const,
      },
      {
        title: 'Occupés',
        value: `${occupancyRate} %`,
        description: `${occupied} sur ${activeLots.length} lot${activeLots.length === 1 ? '' : 's'} actif${activeLots.length === 1 ? '' : 's'} · ${vacant} vacant${vacant === 1 ? '' : 's'}`,
        tone: 'success' as const,
      },
      {
        title: 'Potentiel mensuel du parc',
        value: this.currency(this.portfolioPotentialCents()),
        description: 'Loyers et charges, sans prorata',
        tone: 'primary' as const,
      },
    ];
  });
  protected readonly tableLots = computed<readonly LotListItem[]>(() => {
    const occupanciesByLot = new Map<string, LotOccupancy[]>();
    for (const occupancy of this.occupancies()) {
      const lotOccupancies = occupanciesByLot.get(occupancy.lotId) ?? [];
      lotOccupancies.push(occupancy);
      occupanciesByLot.set(occupancy.lotId, lotOccupancies);
    }
    const currentDate = currentBusinessDate();

    return this.lots().map((lot) => {
      const archived = lot.status === 'Archived';
      const occupancies = occupanciesByLot.get(lot.id) ?? [];
      const currentOccupancy = archived
        ? undefined
        : currentLeaseForDate(occupancies, currentDate);
      const potential = monthlyRentalPotential(lot, occupancies, currentDate);
      const receivableCents = expectedMonthlyReceiptsCents(occupancies);
      return {
        id: lot.id,
        reference: lot.reference,
        sci: lot.sciName,
        address: `${lot.street}, ${lot.postalCode} ${lot.city}`,
        type: this.typeLabel(lot.type),
        occupation: currentOccupancy
          ? `${currentOccupancy.tenantFirstName} ${currentOccupancy.tenantLastName}`
          : '—',
        amount: archived ? '—' : this.currency(potential.totalCents),
        amountBreakdown: archived
          ? receivableCents > 0
            ? `${this.currency(receivableCents)} à percevoir ce mois`
            : ''
          : `${this.currency(potential.rentExcludingChargesCents)} HC + ${this.currency(potential.chargesCents)} charges`,
        archived,
        hasReceivableAmount: receivableCents > 0,
        status: archived ? 'Archivé' : currentOccupancy ? 'Occupé' : 'Vacant',
        statusTone: archived ? 'neutral' : currentOccupancy ? 'success' : 'warning',
        editUrl: `/lots/${lot.id}`,
      };
    });
  });

  ngOnInit(): void {
    const message = this.route.snapshot.queryParamMap.get('created') === '1'
      ? 'created'
      : this.route.snapshot.queryParamMap.get('updated') === '1'
        ? 'updated'
        : null;
    if (message) {
      this.successMessage.set(message);
      this.successTimer = setTimeout(() => this.successMessage.set(null), 5_000);
    }

    if (this.presentationLots() !== null || this.presentationState() !== null) {
      return;
    }
    this.load();
  }

  ngOnDestroy(): void {
    if (this.successTimer) {
      clearTimeout(this.successTimer);
    }
  }

  protected load(): void {
    this.localState.set('loading');
    forkJoin({ lots: this.lotApi.list(), occupancies: this.occupancyApi.list() })
      .pipe(take(1))
      .subscribe({
        next: ({ lots, occupancies }) => {
          this.loadedLots.set(lots);
          this.loadedOccupancies.set(occupancies);
          this.localState.set('loaded');
        },
        error: () => this.localState.set('error'),
      });
  }

  private portfolioPotentialCents(): number {
    return portfolioMonthlyRentalPotentialCents(this.lots(), this.occupancies());
  }

  private currentOccupiedLotIds(lots: readonly Lot[]): ReadonlySet<string> {
    const activeLotIds = new Set(
      lots.filter((lot) => lot.status === 'Active').map((lot) => lot.id),
    );
    const currentDate = currentBusinessDate();
    return new Set(
      this.occupancies()
        .filter((occupancy) =>
          activeLotIds.has(occupancy.lotId)
          && leaseIncludesDate(occupancy, currentDate),
        )
        .map((occupancy) => occupancy.lotId),
    );
  }


  private currency(cents: number): string {
    return new Intl.NumberFormat('fr-FR', {
      style: 'currency',
      currency: 'EUR',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(cents / 100);
  }

  private typeLabel(type: Lot['type']): string {
    return type === 'Local' ? 'Local commercial' : type === 'Other' ? 'Autre' : type;
  }
}
