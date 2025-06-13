import { Pipe, PipeTransform } from "@angular/core";
import { pluralize } from "./pluralize.pipe";

/**
 * Generates a "time ago" from a date.
 * 
 * Examples: "2 years ago", "1 month ago", "12 days ago", "3 hours ago", "just now".
 */
@Pipe({
    name: 'timeAgo',
})
export class TimeAgoPipe implements PipeTransform {
    transform(value: Date): string {
        return this.timeAgo(value);
    }

    private timeAgo(date: Date): string {
        const today = new Date();

        const years = today.getUTCFullYear() - date.getUTCFullYear();
        const inYears = this.compare(years, 'year');
        if (inYears != null) {
            return inYears;
        }

        const months = today.getUTCMonth() - date.getUTCMonth();
        const inMonths = this.compare(months, 'month');
        if (inMonths != null) {
            return inMonths;
        }

        const days = today.getUTCDate() - date.getUTCDate();
        const inDays = this.compare(days, 'day');
        if (inDays != null) {
            return inDays;
        }

        const hours = today.getUTCHours() - date.getUTCHours();
        const inHours = this.compare(hours, 'hour');
        if (inHours != null) {
            return inHours;
        }

        return 'just now';
    }

    private compare(count: number, datePartName: string): string | null {
        switch (count) {
            case 0:
                return null;
            default:
                return pluralize(count, datePartName) + " ago";
        }
    }
}
