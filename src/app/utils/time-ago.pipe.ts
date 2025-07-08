import { Pipe, PipeTransform } from "@angular/core";
import { pluralize } from "./pluralize.pipe";

/**
 * Generates a "time ago" from a date.
 * 
 * Examples: "2 years ago", "1 month ago", "12 days ago", "3 hours ago", "just now".
 *
 * Note:
 *      It is IMPRECISE on months calculation.
 *      It assumes a month is always 30 days, which is not great but is good enough for our use case.
 *      It also assumes a year is 365 days, which is even less impactful.
 */
@Pipe({
    name: 'timeAgo',
})
export class TimeAgoPipe implements PipeTransform {
    transform(value: Date): string {
        return this.timeAgo(value);
    }

    private timeAgo(date: Date): string {
        // 30 days per month is imprecise, but it works well enough for what we want.
        const daysPerMonth = 30;

        const timeAgoMilliseconds: number = new Date().valueOf() - date.valueOf();

        const timeAgoHours: number = Math.floor(timeAgoMilliseconds / 1000 / 60 / 60);
        if (timeAgoHours === 0) {
            return 'just now';
        }
        if (timeAgoHours < 24) {
            return pluralize(timeAgoHours, 'hour') + " ago";
        }

        const timeAgoDays = Math.floor(timeAgoHours / 24);
        if (timeAgoDays < daysPerMonth) {
            return pluralize(timeAgoDays, 'day') + " ago";
        }

        const timeAgoMonths = Math.floor(timeAgoDays / daysPerMonth);
        if (timeAgoMonths < 12) {
            return pluralize(timeAgoMonths, 'month') + " ago";
        }

        const timeAgoYears = Math.floor(timeAgoDays / 365);
        return pluralize(timeAgoYears, 'year') + " ago";
    }
}
