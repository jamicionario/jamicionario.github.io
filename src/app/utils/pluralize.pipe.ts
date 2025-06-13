import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'pluralize',
})
export class PluralizePipe implements PipeTransform {
    transform(value: number, name: string, pluralName?: string): string {
        return pluralize(value, name, pluralName);
    }
}

export function pluralize(value: number, name: string, pluralName?: string): string {
    if (value === 1) {
        return `1 ${name}`;
    }
    if (pluralName == null) {
        pluralName = `${name}s`;
    }
    if (!value) {
        return `no ${pluralName}`;
    }
    return `${value} ${pluralName}`;
}
