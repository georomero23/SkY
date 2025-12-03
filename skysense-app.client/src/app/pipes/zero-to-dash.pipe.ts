import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'zeroToDash'
})
export class ZeroToDashPipe implements PipeTransform {
  transform(value: number | string | null | undefined): string | number {
     if (value === 0 || value === '0' || value === null || value === undefined) {
    return '-';
  }
  // Usa el pipe number internamente si quieres formato
  return new Intl.NumberFormat('es-MX', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(+value);
  }
}