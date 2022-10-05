
export interface Region {
  locationId: number;
  locationName: string;
}

export const REGIONS: Region[] = [
  { locationId: 1, locationName: 'British Columbia' },
  { locationId: 2, locationName: 'Cariboo' },
  { locationId: 3, locationName: 'Kootenay' },
  { locationId: 4, locationName: 'Mainland / Southwest' },
  { locationId: 5, locationName: 'North Coast & Nechako' },
  { locationId: 6, locationName: 'Northeast' },
  { locationId: 7, locationName: 'Thompson-Okanagan' },
  { locationId: 8, locationName: 'Vancouver Island / Coast' }
];
