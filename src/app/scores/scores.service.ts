import { Injectable } from '@angular/core';
import metadata from '@public/scores/score-metadata.json';

export type Score = {
  Number: number,
  Name: string,
  Mscz: string,
  Pages: string[],
};

@Injectable({
  providedIn: 'root'
})
export class ScoreService {
  getScores() : Score[] {
    return metadata;
  }

  getScore(scoreId: number): Score | undefined {
    return metadata.find(score => score.Number === scoreId);
  }
}
