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
