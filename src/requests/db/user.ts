import db from './main';
import {encrypt} from '../../crypto';

export function signUp(email: string, firstName: string, lastName: string, group: string, password: string): Promise<number> {
    return new Promise((resolve, reject) => {
        const cryptedPassword = encrypt(password);
        const sql =
        `INSERT INTO Users (firstName, lastName, email, password, group_)
        VALUES ('${firstName}', '${lastName}', '${email}', '${cryptedPassword}', '${group}')`;
        db.run(sql, [], function(err) {
            if (err) {
                reject(err);
            } else {
                resolve(this.lastID);
            }
        });
    });
}

export function signIn(email: string, password: string): Promise<number> {
    return new Promise((resolve, reject) => {
        const cryptedPassword = encrypt(password);
        const sql =
        `SELECT Users.id
        FROM Users 
        WHERE email = '${email}' AND password = '${cryptedPassword}'`;
        db.all(sql, [], (err, rows) => {
            if (err) {
                reject(err);
            } else if (rows.length === 0) {
                const err = new Error('Неверный логин или пароль.');
                reject(err);
            } else {
                resolve(rows[0].id);
            }
        });
    });
}
