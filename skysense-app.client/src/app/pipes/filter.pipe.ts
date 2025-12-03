import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'filtrar'
})
export class FilterPipe implements PipeTransform {
  transform(items: any[], searchTerms: any[], key?: string[]): any[] {
    if (!items || !searchTerms || searchTerms.length === 0) {
      return items;
    }

    //searchTerms = searchTerms.map(term => term.toLowerCase());

    return items.filter(item => {
      if(key && key.length > 0){
        return key.every((k, i) => {
          return item[k] !== undefined && item[k] === searchTerms[i];
        });
      }
      const searchTerm = searchTerms[0].toLowerCase();
      // If no key is provided, search across all string properties
      return Object.values(item).some(val => 
        (typeof val === 'string' && val.toLowerCase().includes(searchTerm))
      );
    });
  }
}