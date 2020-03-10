import sqlite3 from 'sqlite3';
sqlite3.verbose();

const db = new sqlite3.Database('./data/database.sqlite3', err => {
    if (err) {
        return console.error(err.message);
    }
    console.log('Database is connected');
    createUsersTable();
    createGuidesTable();
    createPartGuidesTable();
    createModelAnnotationsTable();
});

function createUsersTable() {
    const sqlQuery =
        `CREATE TABLE IF NOT EXISTS 'Users' (
        'id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
        'email' TEXT NOT NULL UNIQUE,
        'firstName' TEXT NOT NULL,
        'lastName' TEXT NOT NULL,
        'group' TEXT NOT NULL,
        'password' TEXT NOT NULL
        );`;
    runQuery(sqlQuery, 'Users table have been created');
}

function createGuidesTable() {
    const sqlQuery =
        `CREATE TABLE IF NOT EXISTS 'Guides' (
        'id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
        'name' TEXT NOT NULL,
        'description' TEXT,
        'code' TEXT,
        'ownerId' INTEGER NOT NULL,
        FOREIGN KEY('ownerId') REFERENCES 'Users'('id')
        );`;
    runQuery(sqlQuery, 'Guides table have been created');
}

function createPartGuidesTable() {
    const sqlQuery =
        `CREATE TABLE IF NOT EXISTS 'PartGuides' (
        'guideId' INTEGER NOT NULL,
        'name' TEXT NOT NULL,
        'content' TEXT,
        'sortKey' INTEGER NOT NULL,
        FOREIGN KEY('guideId') REFERENCES 'Guides'('id')
        );`;
    runQuery(sqlQuery, 'PartGuides table have been created');
}

function createModelAnnotationsTable() {
    const sqlQuery =
        `CREATE TABLE IF NOT EXISTS 'ModelAnnotations' (
        'guideId' INTEGER NOT NULL,
        'x' INTEGER NOT NULL,
        'y' INTEGER NOT NULL,
        'z' INTEGER NOT NULL,
        'text' TEXT NOT NULL,
        FOREIGN KEY('guideId') REFERENCES 'Guides'('id')
        );`;
    runQuery(sqlQuery, 'ModelAnnotations table have been created');
}

function runQuery(sqlQuery: string, successMessage: string) {
    db.run(sqlQuery, err => {
        if (err) {
            console.error(err);
        } else {
            console.log(successMessage);
        }
    });
}

export default db;
