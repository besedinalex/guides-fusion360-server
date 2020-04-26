import {changeData, selectData} from "sqlite3-simple-api";
import Guide from "../interfaces/guide";
import PartGuide from "../interfaces/part-guide";

const guideTable =
    `CREATE TABLE IF NOT EXISTS 'Guides' (
    'id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    'name' TEXT NOT NULL,
    'description' TEXT,
    'ownerId' INTEGER NOT NULL,
    'hidden' TEXT NOT NULL
    );`
const partGuideTable =
    `CREATE TABLE IF NOT EXISTS 'PartGuides' (
    'id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    'guideId' INTEGER NOT NULL,
    'name' TEXT NOT NULL,
    'content' TEXT,
    'sortKey' INTEGER NOT NULL,
    FOREIGN KEY('guideId') REFERENCES 'Guides'('id')
    );`;
changeData(guideTable);
changeData(partGuideTable);

function selectGuides(): Promise<Guide[]> {
    const sql = `SELECT G.id, G.name, G.description FROM Guides AS G`;
    return selectData(sql) as Promise<Guide[]>;
}

function selectPartGuides(guideId: number): Promise<PartGuide[]> {
    const sql =
        `SELECT PG.id, PG.name, PG.content, PG.sortKey FROM PartGuides as PG
        WHERE PG.guideId = ${guideId} ORDER BY PG.sortKey ASC`;
    return selectData(sql) as Promise<PartGuide[]>;
}

function selectPartGuide(id: number): Promise<PartGuide> {
    const sql = `SELECT PG.name, PG.content, PG.sortKey FROM PartGuides as PG WHERE PG.id = ${id}`;
    return selectData(sql, true) as Promise<PartGuide>;
}

function insertGuide(name: string, description: string, ownerId: number): Promise<number> {
    const sql =
        `INSERT INTO Guides (name, description, ownerId, hidden)
        VALUES ('${name}', '${description}', '${ownerId}', 'true')`;
    return changeData(sql) as Promise<number>;
}

function insertPartGuide(guideId: number, name: string, content: string, sortKey: number): Promise<number> {
    const sql =
        `INSERT INTO PartGuides (guideId, name, content, sortKey)
        VALUES ('${guideId}', '${name}', '${content}', '${sortKey}')`;
    return changeData(sql) as Promise<number>;
}

function updatePartGuide(id: number, name: string, content: string): Promise<number> {
    const sql = `UPDATE PartGuides SET name='${name}', content='${content}' WHERE id=${id}`;
    return changeData(sql) as Promise<number>;
}

function updatePartGuideSortKey(id: number, sortKey: number): Promise<number> {
    const sql = `UPDATE PartGuides SET sortKey='${sortKey}' WHERE id=${id}`;
    return changeData(sql) as Promise<number>;
}

export {
    selectGuides,
    selectPartGuides,
    selectPartGuide,
    insertGuide,
    insertPartGuide,
    updatePartGuide,
    updatePartGuideSortKey
}
