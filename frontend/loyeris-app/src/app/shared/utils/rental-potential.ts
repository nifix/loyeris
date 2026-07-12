import {
  currentBusinessDate,
  leaseIncludesDate,
  type LeaseMonthlyAmount,
  type LeasePeriod,
} from './lease-period';

export interface RentalPotentialLot {
  readonly id: string;
  readonly status: 'Active' | 'Archived';
  readonly potentialRentExcludingChargesCents: number;
  readonly potentialChargesCents: number;
}

export interface LotLeaseMonthlyAmount extends LeaseMonthlyAmount {
  readonly lotId: string;
}

export interface MonthlyRentalPotential {
  readonly rentExcludingChargesCents: number;
  readonly chargesCents: number;
  readonly totalCents: number;
}

export function currentLeaseForDate<TLease extends LeasePeriod>(
  leases: readonly TLease[],
  date = currentBusinessDate(),
): TLease | undefined {
  return leases
    .filter((lease) => leaseIncludesDate(lease, date))
    .sort((first, second) => second.startsOn.localeCompare(first.startsOn))[0];
}

export function monthlyRentalPotential(
  lot: RentalPotentialLot,
  leases: readonly LotLeaseMonthlyAmount[],
  date = currentBusinessDate(),
): MonthlyRentalPotential {
  const currentLease = currentLeaseForDate(leases, date);
  const rentExcludingChargesCents = currentLease?.rentExcludingChargesCents
    ?? lot.potentialRentExcludingChargesCents;
  const chargesCents = currentLease?.chargesCents ?? lot.potentialChargesCents;

  return {
    rentExcludingChargesCents,
    chargesCents,
    totalCents: rentExcludingChargesCents + chargesCents,
  };
}

export function portfolioMonthlyRentalPotentialCents(
  lots: readonly RentalPotentialLot[],
  leases: readonly LotLeaseMonthlyAmount[],
  date = currentBusinessDate(),
): number {
  const leasesByLot = new Map<string, LotLeaseMonthlyAmount[]>();
  for (const lease of leases) {
    const lotLeases = leasesByLot.get(lease.lotId) ?? [];
    lotLeases.push(lease);
    leasesByLot.set(lease.lotId, lotLeases);
  }

  return lots
    .filter((lot) => lot.status === 'Active')
    .reduce(
      (total, lot) => total
        + monthlyRentalPotential(lot, leasesByLot.get(lot.id) ?? [], date).totalCents,
      0,
    );
}
