
export type Score = {
  number: number,
  name: string,
  searchableName: string,

  pages: string[],
  region: string,
  typeOfDance: string,
  labels: Map<string, string>,

  folderStructure: string[],
};
