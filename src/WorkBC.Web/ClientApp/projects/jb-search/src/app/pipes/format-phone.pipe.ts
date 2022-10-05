import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'formatphone' })
export class FormatPhonePipe implements PipeTransform {
  transform(value: string): string {
    if (value.length == 10) {
      return '(' + value.substr(0, 3) + ') ' + value.substr(3, 3) + '-' + value.substr(6);
    }
    else return value;
  }
}
