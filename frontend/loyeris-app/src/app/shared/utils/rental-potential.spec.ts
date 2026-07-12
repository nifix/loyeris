import {
  monthlyRentalPotential,
  portfolioMonthlyRentalPotentialCents,
  type RentalPotentialLot,
} from './rental-potential';

describe('rental potential utilities', () => {
  const currentDate = '2026-07-15';

  it('should prefer the complete current lease amount over the configured lot potential', () => {
    const potential = monthlyRentalPotential(
      createLot(),
      [
        {
          lotId: 'lot-1',
          startsOn: '2026-07-12',
          endsOn: null,
          rentExcludingChargesCents: 81000,
          chargesCents: 9000,
        },
      ],
      currentDate,
    );

    expect(potential).toEqual({
      rentExcludingChargesCents: 81000,
      chargesCents: 9000,
      totalCents: 90000,
    });
  });

  it('should use configured amounts for vacant lots and exclude archived lots', () => {
    const total = portfolioMonthlyRentalPotentialCents(
      [
        createLot(),
        createLot({ id: 'lot-2', potentialRentExcludingChargesCents: 75000 }),
        createLot({ id: 'lot-3', status: 'Archived' }),
      ],
      [],
      currentDate,
    );

    expect(total).toBe(150000);
  });
});

function createLot(overrides: Partial<RentalPotentialLot> = {}): RentalPotentialLot {
  return { ...baseLot(), ...overrides };
}

function baseLot(): RentalPotentialLot {
  return {
    id: 'lot-1',
    status: 'Active',
    potentialRentExcludingChargesCents: 65000,
    potentialChargesCents: 5000,
  };
}
