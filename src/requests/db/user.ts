import db from './db-connection';

interface UserLoginData {
    id: number;
    password: string;
}

export function signUp(email: string, firstName: string, lastName: string, group: string, password: string): Promise<number> {
    return new Promise((resolve, reject) => {
        const sql =
            `INSERT INTO Users (firstName, lastName, email, password, group)
            VALUES ('${firstName}', '${lastName}', '${email}', '${password}', '${group}')`;
        db.run(sql, [], function(err) {
            if (err) {
                reject(err);
            } else {
                resolve(this.lastID);
            }
        });
    });
}

export function signIn(email: string): Promise<UserLoginData> {
    return new Promise((resolve, reject) => {
        const sql = `SELECT Users.id, Users.password FROM Users WHERE email = '${email}'`;
        db.all(sql, [], (err, rows: UserLoginData[]) => {
            if (err) {
                reject(err);
            } else if (rows.length === 0) {
                reject(err);
            } else {
                resolve(rows[0]);
            }
        });
    });
}
