
export class ScoreLabel {
  name!: string;
  values!: string[];

  constructor(init: Required<ScoreLabel>) {
    Object.assign(this, init);
  }
}
