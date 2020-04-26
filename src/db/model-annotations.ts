import {changeData, selectData} from "sqlite3-simple-api";
import ModelAnnotation from "../interfaces/model-annotation";

const modelAnnotationsTable =
    `CREATE TABLE IF NOT EXISTS 'ModelAnnotations' (
    'guideId' INTEGER NOT NULL,
    'x' INTEGER NOT NULL,
    'y' INTEGER NOT NULL,
    'z' INTEGER NOT NULL,
    'text' TEXT NOT NULL,
    FOREIGN KEY('guideId') REFERENCES 'Guides'('id')
    );`;
changeData(modelAnnotationsTable);

function selectModelAnnotations(guideId: number): Promise<ModelAnnotation[]> {
    const sql = `SELECT MA.x, MA.y, MA.z, MA.text FROM ModelAnnotations as MA WHERE MA.guideId = ${guideId}`;
    return selectData(sql) as Promise<ModelAnnotation[]>;
}

function insertNewAnnotation(guideId: number, x: number, y: number, z: number, text: string): Promise<number> {
    const sql =
        `INSERT INTO ModelAnnotations (guideId, x, y, z, text)
        VALUES ('${guideId}', '${x}', '${y}', '${z}', '${text}')`;
    return changeData(sql) as Promise<number>;
}

export {
    selectModelAnnotations,
    insertNewAnnotation
}
