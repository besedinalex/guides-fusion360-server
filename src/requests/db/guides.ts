import db from './main';
import Guide from "../../interfaces/guide";
import PartGuide from "../../interfaces/part-guide";

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
    });
}

export function selectPartGuides(guideId: number): Promise<PartGuide[]> {
    return new Promise((resolve, reject) => {
        const sql =
        `SELECT PartGuide.name, PartGuide.content, PartGuide.sortKey
        FROM PartGuide
        WHERE PartGuide.guideId = ${guideId}`;
        db.all(sql, [], (err, rows: PartGuide[]) => {
            if (err) {
                reject(err);
            } else {
                resolve(rows);
            }
        });
    });
}
