import sqlite3 from 'sqlite3';
import fs from 'fs-extra';

sqlite3.verbose();

// Creates data folder
const databasePath = process.cwd() + '/data';
if (!fs.pathExistsSync(databasePath)) {
    fs.mkdirpSync(databasePath);
}

// Creates database connection
const db = new sqlite3.Database('./data/database.sqlite3', err => {
    if (err) {
        throw Error('cannot connect to database');
    }
});

export default db;
