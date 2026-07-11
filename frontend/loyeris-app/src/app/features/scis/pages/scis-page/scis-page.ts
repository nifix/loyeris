import { Component, computed, inject, input, OnDestroy, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { take } from 'rxjs';
import { AppShell } from '../../../../core/layouts/app-shell/app-shell';
import { MetricCard } from '../../../../shared/components/ui-metric-card/metric-card';
import { PageHeader } from '../../../../shared/components/ui-page-header/page-header';
import { SciCard } from '../../components/ui-sci-card/sci-card';
import { type Sci, SciApi } from '../../services/sci-api';

export type SciListState = 'loading' | 'loaded' | 'error';

@Component({
  selector: 'app-scis-page',
  imports: [AppShell, MetricCard, PageHeader, RouterLink, SciCard],
  templateUrl: './scis-page.html',
  styleUrl: './scis-page.css',
})
export class ScisPage implements OnInit, OnDestroy {
  readonly presentationScis = input<readonly Sci[] | null>(null);
  readonly presentationState = input<SciListState | null>(null);
  readonly creationSucceeded = input(false);
  readonly updateSucceeded = input(false);

  private readonly api = inject(SciApi);
  private readonly route = inject(ActivatedRoute);
  private readonly loadedScis = signal<readonly Sci[]>([]);
  private readonly localState = signal<SciListState>('loading');
  private readonly createdFromNavigation = signal(false);
  private readonly updatedFromNavigation = signal(false);
  private readonly creationMessageDismissed = signal(false);
  private creationMessageTimer: ReturnType<typeof setTimeout> | undefined;

  protected readonly scis = computed(() =>
    [...(this.presentationScis() ?? this.loadedScis())].sort((first, second) => {
      const statusOrder = Number(first.status === 'Archived') - Number(second.status === 'Archived');
      return statusOrder || first.name.localeCompare(second.name, 'fr');
    }),
  );
  protected readonly state = computed(() => this.presentationState() ?? this.localState());
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
    this.api
      .list()
      .pipe(take(1))
      .subscribe({
        next: (scis) => {
          this.loadedScis.set(scis);
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
}
