export interface SalaryRanges {
  range1: string;
  range2: string;
  range3: string;
  range4: string;
  range5: string;
  intervalText: string;
}

export class SalaryRange {
  public static hourly: SalaryRanges = {
    range1: 'Under $20.00 per hour',
    range2: '$20.00 to $29.99 per hour',
    range3: '$30.00 to $39.99 per hour',
    range4: '$40.00 to $49.99 per hour',
    range5: '$50.00+ per hour',
    intervalText: 'per hour'
  }

  public static weekly: SalaryRanges = {
    range1: 'Under $800 per week',
    range2: '$800 to $1,199 per week',
    range3: '$1,200 to $1,599 per week',
    range4: '$1,600 to $1,999 per week',
    range5: '$2,000+ per week',
    intervalText: 'per week'
  }

  public static biweekly: SalaryRanges = {
    range1: 'Under $1,600 biweekly',
    range2: '$1,600 to $2,399 biweekly',
    range3: '$2,400 to $3,199 biweekly',
    range4: '$3,200 to $3,999 biweekly',
    range5: '$4,000+ biweekly',
    intervalText: 'biweekly'
  }

  public static monthly: SalaryRanges = {
    range1: 'Under $4,000 per month',
    range2: '$4,000 to $5,999 per month',
    range3: '$6,000 to $7,999 per month',
    range4: '$8,000 to $9,999 per month',
    range5: '$10,000+ per month',
    intervalText: 'per month'
  }

  public static annually: SalaryRanges = {
    range1: 'Under $40,000 per year',
    range2: '$40,000 to $59,999 per year',
    range3: '$60,000 to $79,999 per year',
    range4: '$80,000 to $99,999 per year',
    range5: '$100,000+ per year',
    intervalText: 'per year'
  }

  public static options = []
}
