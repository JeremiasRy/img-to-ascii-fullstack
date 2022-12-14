export default class Picture {
    rows: number;
    columns: number;
    picture: string[];

    constructor(rows:number, columns:number, picture:string[]) {
      this.rows = rows;
      this.columns = columns;
      this.picture = picture.map(row => row.replace(/ /g, "\u00A0"));
    }
}