
export class Score {
  number!: number;
  name!: string;
  searchableName!: string;
  // mscz: string,
  pages!: string[];
  isPortuguese!: boolean;
  category!: string;
  subcategories!: string[];

  labels!: Map<string, string>;

  constructor(init: Required<Score>) {
    Object.assign(this, init);
  }
};
