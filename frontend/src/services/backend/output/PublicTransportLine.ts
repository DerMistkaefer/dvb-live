export interface PublicTransportLine {
    /**
     * Local regional title of the Line.
     */
    title: string;

    /**
     * Description of the start point from this line.
     */
    from: string;

    /**
     * Description of the end point from this line.
     */
    to: string;

    /**
     * Geo Json Data of this Line.
     */
    line: GeoJSON.Feature<GeoJSON.MultiLineString>;

//#region Additional Data

    /**
     * Url of Line Change Data for this line.
     */
    urlLineChange: string | null;

    /**
     * Information about the clocking of this line.
     */
    clocking: string | null;

//#endregion
}
