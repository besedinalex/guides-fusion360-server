import db from './db-connection';
import Guide from "../interfaces/guide";
import PartGuide from "../interfaces/part-guide";

export function selectGuides(): Promise<Guide[]> {
    return new Promise((resolve, reject) => {
        const sql = `SELECT G.id, G.name, G.description FROM Guides AS G`;
        db.all(sql, [], (err, rows: Guide[]) => {
            if (err) {
                reject(err);
            } else {
                resolve(rows);
            }
        });
    });
}

export function selectPartGuide(id: number): Promise<PartGuide> {
    return new Promise((resolve, reject) => {
        const sql = `SELECT PG.name, PG.content, PG.sortKey FROM PartGuides as PG WHERE PG.id = ${id}`;
        db.all(sql, [], (err, rows: PartGuide[]) => {
            if (err || rows.length === 0) {
                reject(err);
            } else {
                resolve(rows[0]);
            }
        });
    });
}

export function selectPartGuides(guideId: number): Promise<PartGuide[]> {
    return new Promise((resolve, reject) => {
        const sql =
        `SELECT PG.id, PG.name, PG.content, PG.sortKey
        FROM PartGuides as PG
        WHERE PG.guideId = ${guideId}
        ORDER BY PG.sortKey ASC`;
        db.all(sql, [], (err, rows: PartGuide[]) => {
            if (err || rows.length === 0) {
                reject(err);
            } else {
                resolve(rows);
            }
        });
    });
}

export function insertGuide(name: string, description: string, ownerId: number): Promise<number> {
    return new Promise((resolve, reject) => {
        const sql =
            `INSERT INTO Guides (name, description, ownerId, hidden)
            VALUES ('${name}', '${description}', '${ownerId}', 'true')`;
        db.run(sql, [], function (err) {
            if (err) {
                reject(err);
            } else {
                resolve(this.lastID);
            }
        });
    });
}

export function insertPartGuide(guideId: number, name: string, content: string, sortKey: number): Promise<number> {
    return new Promise((resolve, reject) => {
        const sql =
            `INSERT INTO PartGuides (guideId, name, content, sortKey)
            VALUES ('${guideId}', '${name}', '${content}', '${sortKey}')`;
        db.run(sql, [], function (err) {
            if (err) {
                reject(err);
            } else {
                resolve(this.lastID);
            }
        });
    });
}

export function updatePartGuide(id: number, name: string, content: string): Promise<number> {
    return new Promise((resolve, reject) => {
        const sql = `UPDATE PartGuides SET name='${name}', content='${content}' WHERE id=${id}`;
        db.run(sql, [], function (err) {
            if (err) {
                reject(err);
            } else {
                resolve(this.changes);
            }
        });
    });
}

export function updatePartGuideSortKey(id: number, sortKey: number): Promise<number> {
    return new Promise((resolve, reject) => {
        const sql = `UPDATE PartGuides SET sortKey='${sortKey}' WHERE id=${id}`;
        db.run(sql, [], function (err) {
            if (err) {
                reject(err);
            } else {
                resolve(this.changes);
            }
        });
    });
}