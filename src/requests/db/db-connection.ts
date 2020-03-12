import sqlite3 from 'sqlite3';

sqlite3.verbose();

const db = new sqlite3.Database('./data/database.sqlite3', err => {
    if (err) {
        return console.error(err.message);
    }
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
        'studyGroup' TEXT NOT NULL,
        'password' TEXT NOT NULL
        );`;
    db.run(sqlQuery);
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
    db.run(sqlQuery);
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
    db.run(sqlQuery);
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
    db.run(sqlQuery);
}

export default db;
