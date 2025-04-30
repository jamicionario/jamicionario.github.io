
export type Score = {
  number: number,
  name: string,
  searchableName: string,
  pages: string[],
  isPortuguese: boolean,
  category: string,
  subcategories: string[],

  labels: Map<string, string>,
};
