
export type Score = {
  number: number,
  name: string,
  searchableName: string,

  pages: string[],
  region: string | null,
  typeOfDance: string | null,
  labels: Map<string, string>,

  files: {
    pdf: string,
    mscz: string,
  },

  folderStructure: string[],
};
