import sqlite3 from 'sqlite3';

sqlite3.verbose();

const db = new sqlite3.Database('./data/database.sqlite3', err => {
    if (err) {
        return console.error(err.message);
    }
    console.log('Database is connected.');
    for (const query of sqlCreateQueries) {
        db.run(query, err => {
            if (err) {
                console.error(err);
            }
        });
    }
});

const sqlCreateQueries = [
    `CREATE TABLE IF NOT EXISTS 'Users' (
    'id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    'email' TEXT NOT NULL UNIQUE,
    'password' TEXT NOT NULL,
    'firstName' TEXT NOT NULL,
    'lastName' TEXT NOT NULL,
    'access' TEXT NOT NULL,
    'studyGroup' TEXT,
    'hidden' TEXT NOT NULL
    );`,

    `CREATE TABLE IF NOT EXISTS 'Guides' (
    'id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    'name' TEXT NOT NULL,
    'description' TEXT,
    'ownerId' INTEGER NOT NULL,
    'hidden' TEXT NOT NULL,
    FOREIGN KEY('ownerId') REFERENCES 'Users'('id')
    );`,

    `CREATE TABLE IF NOT EXISTS 'PartGuides' (
    'id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    'guideId' INTEGER NOT NULL,
    'name' TEXT NOT NULL,
    'content' TEXT,
    'sortKey' INTEGER NOT NULL,
    FOREIGN KEY('guideId') REFERENCES 'Guides'('id')
    );`,

    `CREATE TABLE IF NOT EXISTS 'ModelAnnotations' (
    'guideId' INTEGER NOT NULL,
    'x' INTEGER NOT NULL,
    'y' INTEGER NOT NULL,
    'z' INTEGER NOT NULL,
    'text' TEXT NOT NULL,
    FOREIGN KEY('guideId') REFERENCES 'Guides'('id')
    );`
];

export default db;
