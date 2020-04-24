import db from "./db-connection";
import ModelAnnotation from "../interfaces/model-annotation";

export function selectModelAnnotations(guideId: number): Promise<ModelAnnotation[]> {
    return new Promise((resolve, reject) => {
        const sql = `SELECT MA.x, MA.y, MA.z, MA.text FROM ModelAnnotations as MA WHERE MA.guideId = ${guideId}`;
        db.all(sql, [], (err, rows: ModelAnnotation[]) => {
            if (err) {
                reject(err);
            } else {
                resolve(rows);
            }
        });
    });
}

export function insertNewAnnotation(guideId: number, x: number, y: number, z: number, text: string): Promise<number> {
    return new Promise((resolve, reject) => {
        const sql =
            `INSERT INTO ModelAnnotations (guideId, x, y, z, text)
            VALUES ('${guideId}', '${x}', '${y}', '${z}', '${text}')`;
        db.run(sql, [], function (err) {
            if (err) {
                reject(err);
            } else {
                resolve(this.changes);
            }
        });
    });
}
