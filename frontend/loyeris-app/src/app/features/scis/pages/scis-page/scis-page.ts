import { Component, computed, inject, input, OnDestroy, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin, take } from 'rxjs';
import { AppShell } from '../../../../core/layouts/app-shell/app-shell';
import { MetricCard } from '../../../../shared/components/ui-metric-card/metric-card';
import { PageHeader } from '../../../../shared/components/ui-page-header/page-header';
import {
  currentBusinessDate,
  leaseIncludesDate,
} from '../../../../shared/utils/lease-period';
import { portfolioMonthlyRentalPotentialCents } from '../../../../shared/utils/rental-potential';
import { LotApi, type Lot } from '../../../lots/services/lot-api';
import {
  LotOccupancyApi,
  type LotOccupancy,
} from '../../../lots/services/lot-occupancy-api';
import { SciCard } from '../../components/ui-sci-card/sci-card';
import { type Sci, SciApi } from '../../services/sci-api';

export type SciListState = 'loading' | 'loaded' | 'error';

interface SciIndicators {
  readonly lotCount: number;
  readonly tenantCount: number;
  readonly occupancyRate: number;
  readonly monthlyPotential: string;
}

@Component({
  selector: 'app-scis-page',
  imports: [AppShell, MetricCard, PageHeader, RouterLink, SciCard],
  templateUrl: './scis-page.html',
  styleUrl: './scis-page.css',
})
export class ScisPage implements OnInit, OnDestroy {
  readonly presentationScis = input<readonly Sci[] | null>(null);
  readonly presentationLots = input<readonly Lot[] | null>(null);
  readonly presentationOccupancies = input<readonly LotOccupancy[] | null>(null);
  readonly presentationState = input<SciListState | null>(null);
  readonly creationSucceeded = input(false);
  readonly updateSucceeded = input(false);

  private readonly sciApi = inject(SciApi);
  private readonly lotApi = inject(LotApi);
  private readonly occupancyApi = inject(LotOccupancyApi);
  private readonly route = inject(ActivatedRoute);
  private readonly loadedScis = signal<readonly Sci[]>([]);
  private readonly loadedLots = signal<readonly Lot[]>([]);
  private readonly loadedOccupancies = signal<readonly LotOccupancy[]>([]);
  private readonly localState = signal<SciListState>('loading');
  private readonly createdFromNavigation = signal(false);
  private readonly updatedFromNavigation = signal(false);
  private readonly creationMessageDismissed = signal(false);
  protected readonly showArchivedScis = signal(false);
  private creationMessageTimer: ReturnType<typeof setTimeout> | undefined;

  protected readonly scis = computed(() =>
    [...(this.presentationScis() ?? this.loadedScis())].sort((first, second) => {
      const statusOrder = Number(first.status === 'Archived') - Number(second.status === 'Archived');
      return statusOrder || first.name.localeCompare(second.name, 'fr');
    }),
  );
  protected readonly state = computed(() => this.presentationState() ?? this.localState());
  protected readonly lots = computed(() => this.presentationLots() ?? this.loadedLots());
  protected readonly occupancies = computed(
    () => this.presentationOccupancies() ?? this.loadedOccupancies(),
  );
  protected readonly activeScis = computed(() =>
    this.scis().filter((sci) => sci.status === 'Active'),
  );
  protected readonly archivedScis = computed(() =>
    this.scis().filter((sci) => sci.status === 'Archived'),
  );
  protected readonly indicators = computed(() => {
    const result = new Map<string, SciIndicators>();
    const currentDate = currentBusinessDate();

    for (const sci of this.scis().filter((item) => item.status === 'Active')) {
      const sciLots = this.lots().filter((lot) => lot.sciId === sci.id);
      const activeLots = sciLots.filter((lot) => lot.status === 'Active');
      const activeLotIds = new Set(activeLots.map((lot) => lot.id));
      const currentOccupancies = this.occupancies().filter((occupancy) =>
        activeLotIds.has(occupancy.lotId) && leaseIncludesDate(occupancy, currentDate),
      );
      const occupiedLotCount = new Set(
        currentOccupancies.map((occupancy) => occupancy.lotId),
      ).size;
      const tenantCount = new Set(
        currentOccupancies.map((occupancy) => occupancy.tenantId),
      ).size;
      const potentialCents = portfolioMonthlyRentalPotentialCents(
        activeLots,
        this.occupancies().filter((occupancy) => activeLotIds.has(occupancy.lotId)),
        currentDate,
      );

      result.set(sci.id, {
        lotCount: activeLots.length,
        tenantCount,
        occupancyRate: activeLots.length === 0
          ? 0
          : Math.round(occupiedLotCount / activeLots.length * 100),
        monthlyPotential: this.currency(potentialCents),
      });
    }

    return result;
  });
  protected readonly showCreatedMessage = computed(
    () => !this.creationMessageDismissed()
      && (this.creationSucceeded() || this.createdFromNavigation()),
  );
  protected readonly showUpdatedMessage = computed(
    () => this.updateSucceeded() || this.updatedFromNavigation(),
  );
  protected readonly headerChips = computed(() => {
    const items = this.scis();
    const activeCount = items.filter((sci) => sci.status === 'Active').length;
    return [
      `${items.length} structure${items.length > 1 ? 's' : ''} suivie${items.length > 1 ? 's' : ''}`,
      `${activeCount} active${activeCount > 1 ? 's' : ''}`,
    ];
  });
  protected readonly metrics = computed(() => {
    const items = this.scis();
    return [
      {
        title: 'Structures actives',
        value: String(items.filter((sci) => sci.status === 'Active').length),
        description: `${items.length} structure${items.length > 1 ? 's' : ''} au total`,
        tone: 'primary' as const,
      },
      {
        title: 'SIREN renseignés',
        value: String(items.filter((sci) => Boolean(sci.siren)).length),
        description: 'Identifiants juridiques disponibles',
        tone: 'neutral' as const,
      },
      {
        title: 'Adresses renseignées',
        value: String(items.filter((sci) => Boolean(sci.street || sci.city)).length),
        description: 'Sièges sociaux documentés',
        tone: 'success' as const,
      },
    ];
  });

  ngOnInit(): void {
    this.createdFromNavigation.set(this.route.snapshot.queryParamMap.get('created') === '1');
    this.updatedFromNavigation.set(this.route.snapshot.queryParamMap.get('updated') === '1');

    if (this.creationSucceeded() || this.createdFromNavigation()) {
      this.creationMessageTimer = setTimeout(() => this.creationMessageDismissed.set(true), 5_000);
    }

    if (this.presentationScis() !== null || this.presentationState() !== null) {
      return;
    }

    this.load();
  }

  ngOnDestroy(): void {
    if (this.creationMessageTimer) {
      clearTimeout(this.creationMessageTimer);
    }
  }

  protected load(): void {
    this.localState.set('loading');
    forkJoin({
      scis: this.sciApi.list(),
      lots: this.lotApi.list(),
      occupancies: this.occupancyApi.list(),
    })
      .pipe(take(1))
      .subscribe({
        next: ({ scis, lots, occupancies }) => {
          this.loadedScis.set(scis);
          this.loadedLots.set(lots);
          this.loadedOccupancies.set(occupancies);
          this.localState.set('loaded');
        },
        error: () => this.localState.set('error'),
      });
  }

  protected address(sci: Sci): string {
    const locality = [sci.postalCode, sci.city].filter(Boolean).join(' ');
    return [sci.street, locality, sci.country !== 'FR' ? sci.country : null]
      .filter(Boolean)
      .join(', ') || 'Adresse non renseignée';
  }

  protected formattedDate(value: string | null): string | undefined {
    if (!value) {
      return undefined;
    }

    return new Intl.DateTimeFormat('fr-FR', { dateStyle: 'long' }).format(new Date(value));
  }

  protected formattedIncorporationDate(value: string | null): string | undefined {
    if (!value) {
      return undefined;
    }

    const [year, month, day] = value.split('-');
    return `${day}/${month}/${year}`;
  }

  protected formattedArchivedDate(value: string | null): string | undefined {
    if (!value) {
      return undefined;
    }

    return new Intl.DateTimeFormat('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    }).format(new Date(value));
  }

  protected toggleArchivedScis(): void {
    this.showArchivedScis.update((visible) => !visible);
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
