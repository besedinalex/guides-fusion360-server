import db from './main';
import Guide from "../../interfaces/guide";

export function selectGuides(): Promise<Guide[]> {
    return new Promise((resolve, reject) => {
        const sql = `SELECT * FROM Guides`;
        db.all(sql, [], (err, rows: Guide[]) => {
            if (err) {
                reject(err);
            } else {
                resolve(rows);
            }
        });
    })
}
