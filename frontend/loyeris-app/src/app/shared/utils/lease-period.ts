export interface LeasePeriod {
  readonly startsOn: string;
  readonly endsOn: string | null;
}

export interface LeaseMonthlyAmount extends LeasePeriod {
  readonly rentExcludingChargesCents: number;
  readonly chargesCents: number;
}

export function currentBusinessDate(referenceDate = new Date()): string {
  const year = referenceDate.getFullYear();
  const month = String(referenceDate.getMonth() + 1).padStart(2, '0');
  const day = String(referenceDate.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

export function leaseIncludesDate(lease: LeasePeriod, date: string): boolean {
  return lease.startsOn <= date && (!lease.endsOn || lease.endsOn >= date);
}

export function proratedMonthlyAmountCents(
  amountCents: number,
  lease: LeasePeriod,
  referenceDate = new Date(),
): number {
  const year = referenceDate.getFullYear();
  const month = referenceDate.getMonth();
  const monthStart = Date.UTC(year, month, 1);
  const monthEnd = Date.UTC(year, month + 1, 0);
  const leaseStart = businessDateToUtc(lease.startsOn);
  const leaseEnd = lease.endsOn ? businessDateToUtc(lease.endsOn) : monthEnd;
  const occupiedFrom = Math.max(monthStart, leaseStart);
  const occupiedUntil = Math.min(monthEnd, leaseEnd);

  if (occupiedFrom > occupiedUntil) {
    return 0;
  }

  // Lease boundaries are inclusive business dates.
  const millisecondsPerDay = 86_400_000;
  const occupiedDays = Math.floor((occupiedUntil - occupiedFrom) / millisecondsPerDay) + 1;
  const daysInMonth = new Date(monthEnd).getUTCDate();
  return Math.round(amountCents * occupiedDays / daysInMonth);
}

export function expectedMonthlyReceiptsCents(
  leases: readonly LeaseMonthlyAmount[],
  referenceDate = new Date(),
): number {
  return leases.reduce(
    (total, lease) => total
      + proratedMonthlyAmountCents(lease.rentExcludingChargesCents, lease, referenceDate)
      + proratedMonthlyAmountCents(lease.chargesCents, lease, referenceDate),
    0,
  );
}

function businessDateToUtc(value: string): number {
  const [year, month, day] = value.split('-').map(Number);
  return Date.UTC(year, month - 1, day);
}
