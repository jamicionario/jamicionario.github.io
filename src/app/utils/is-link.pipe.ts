import { Pipe, PipeTransform } from "@angular/core";

const linkFormat = /http(s)?:\/\//;

@Pipe({
    name: 'isLink',
})
export class IsLinkPipe implements PipeTransform {
    transform(value: string): boolean {
        return linkFormat.test(value);
    }
}
