import { Searchable } from "./searchable";

export interface Score extends Searchable {
  number: number,
  name: string,

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
