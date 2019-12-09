import db from "./main";
import ModelAnnotation from "../../interfaces/model-annotation";

export function selectModelAnnotations(guideId: number): Promise<Array<ModelAnnotation>> {
    return new Promise((resolve, reject) => {
        const sql =
        `SELECT ModelAnnotations.x, ModelAnnotations.y, ModelAnnotations.z, ModelAnnotations.text
        FROM ModelAnnotations
        WHERE ModelAnnotations.guideId = ${guideId}`;
        db.all(sql, [], (err, rows: Array<ModelAnnotation>) => {
            if (err) {
                reject(err);
            } else {
                resolve(rows);
            }
        });
    });
}

export function insertNewAnnotation(guideId: number, x: number, y: number, z: number, text: string): Promise<string> {
    return new Promise((resolve, reject) => {
        const sql =
        `INSERT INTO ModelAnnotations (guideId, x, y, z, text)
        VALUES ('${guideId}', '${x}', '${y}', '${z}', '${text}')`;
        db.run(sql, [], err => {
            if (err) {
                reject(err);
            } else {
                resolve('success');
            }
        });
    });
}
