import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class JamModeService {
  readonly isJamMode$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  set jamMode(value: boolean) {
    this.isJamMode$.next(value);
  }
}