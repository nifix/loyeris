import {
  expectedMonthlyReceiptsCents,
  proratedMonthlyAmountCents,
} from './lease-period';

describe('lease period utilities', () => {
  const july2026 = new Date(2026, 6, 15);

  it('should prorate an amount between inclusive lease boundaries', () => {
    const amount = proratedMonthlyAmountCents(
      310000,
      { startsOn: '2026-07-12', endsOn: '2026-07-20' },
      july2026,
    );

    expect(amount).toBe(90000);
  });

  it('should cumulate all leases intersecting the selected month', () => {
    const amount = expectedMonthlyReceiptsCents(
      [
        {
          startsOn: '2026-07-01',
          endsOn: '2026-07-10',
          rentExcludingChargesCents: 310000,
          chargesCents: 31000,
        },
        {
          startsOn: '2026-07-20',
          endsOn: null,
          rentExcludingChargesCents: 620000,
          chargesCents: 62000,
        },
        {
          startsOn: '2026-08-01',
          endsOn: null,
          rentExcludingChargesCents: 100000,
          chargesCents: 10000,
        },
      ],
      july2026,
    );

    expect(amount).toBe(374000);
  });
});
