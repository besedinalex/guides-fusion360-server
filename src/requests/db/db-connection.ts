import sqlite3 from 'sqlite3';
sqlite3.verbose();

const db = new sqlite3.Database('./data/database.sqlite3', err => {
    if (err) {
        return console.error(err.message);
    }
    console.log('Database is connected.');
});

export default db;
