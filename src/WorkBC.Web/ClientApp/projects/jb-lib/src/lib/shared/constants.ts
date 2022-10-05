const locationWithin = 'Location: Within ';
const kmOf = ' km of ';

export function getLocationInfo(
  radius: number | string,
  cityOrPostal: string): string {
  return locationWithin + radius + kmOf + cityOrPostal;
}
