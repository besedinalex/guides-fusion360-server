import sqlite3 from 'sqlite3';
import fs from 'fs-extra';
import {changeData} from "./run-query";
import {sqlCreateQueries} from "./db-tables";

sqlite3.verbose();

// Creates data folder
const databasePath = process.cwd() + '/data';
if (!fs.pathExistsSync(databasePath)) {
    fs.mkdirpSync(databasePath);
}

// Creates database connection
const db = new sqlite3.Database('./data/database.sqlite3', err => {
    if (err) {
        return console.error('\nERROR: Cannot connect to database\n');
    }
    console.log('Database is connected');
    for (const query of sqlCreateQueries) {
        changeData(query);
    }
});

export default db;
