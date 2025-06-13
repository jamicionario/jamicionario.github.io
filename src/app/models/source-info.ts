
/**
 * Holds information about the origin of this site and its data.
 */
export interface SourceInfo {
    source: Source;
    data: Source;
}

export interface Source {
    url: string;
    license: string;
    /**
     * Any number of contributors.
     * 
     * Leave empty to attribute to the community.
     */
    contributors: string[];
}