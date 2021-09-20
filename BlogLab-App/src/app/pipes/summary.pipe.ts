import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'summary',
})
export class SummaryPipe implements PipeTransform {
  transform(content: string, limit: number): string {
    if (content.length <= limit) {
      return content;
    } else {
      return `${content.substring(0, limit)}...`;
    }
  }
}
