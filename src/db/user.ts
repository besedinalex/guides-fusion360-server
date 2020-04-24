import db from './db-connection';

interface UserData {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
    studyGroup: string;
    access: string;
    hidden: string;
}

interface UserLoginData {
    id: number;
    password: string;
    hidden: string;
}

export function selectUserAccess(id: number): Promise<string> {
    return new Promise((resolve, reject) => {
        const sql = `SELECT Users.access FROM Users WHERE id='${id}'`;
        db.all(sql, [], (err, rows) => {
            if (err) {
                reject(err);
            } else {
                resolve(rows[0].access);
            }
        });
    });
}

export function selectUserData(id: number): Promise<UserData> {
    return new Promise((resolve, reject) => {
        const sql = `SELECT * FROM Users WHERE id=${id}`;
        db.all(sql, [], (err, rows: UserData[]) => {
            if (err || rows.length === 0) {
                reject(err);
            } else {
                resolve(rows[0]);
            }
        });
    });
}

export function selectUserLoginData(email: string): Promise<UserLoginData> {
    return new Promise((resolve, reject) => {
        const sql = `SELECT U.id, U.password, U.hidden FROM Users AS U WHERE email = '${email}'`;
        db.all(sql, [], (err, rows: UserLoginData[]) => {
            if (err) {
                reject(err);
            } else {
                resolve(rows[0]);
            }
        });
    });
}

export function insertNewUser(email: string, firstName: string, lastName: string, group: string, password: string, access: string): Promise<number> {
    return new Promise((resolve, reject) => {
        const sql =
            `INSERT INTO Users (firstName, lastName, email, password, studyGroup, access, hidden)
            VALUES ('${firstName}', '${lastName}', '${email}', '${password}', '${group}', '${access}', 'false')`;
        db.run(sql, [], function (err) {
            if (err) {
                reject(err);
            } else {
                resolve(this.lastID);
            }
        });
    });
}

export function updatePassword(email: string, password: string): Promise<number> {
    return new Promise((resolve, reject) => {
        const sql = `UPDATE Users SET password='${password}' WHERE email='${email}'`;
        db.run(sql, [], function (err) {
            if (err) {
                reject(err);
            } else {
                resolve(this.changes);
            }
        });
    });
}

export function updateUserAccess(email: string, access: string): Promise<number> {
    return new Promise((resolve, reject) => {
        const sql = `UPDATE Users SET access='${access}' WHERE email='${email}'`;
        db.run(sql, [], function (err) {
            if (err) {
                reject(err);
            } else {
                resolve(this.changes);
            }
        });
    });
}

export function updateUserData(email: string, firstName: string, lastName: string, studyGroup: string): Promise<number> {
    return new Promise((resolve, reject) => {
        const sql =
            `UPDATE Users
            SET firstName='${firstName}', lastName='${lastName}', studyGroup='${studyGroup}'
            WHERE email='${email}'`;
        db.run(sql, [], function (err) {
            if (err) {
                reject(err);
            } else {
                resolve(this.changes);
            }
        });
    });
}

export function deleteUser(email: string): Promise<number> {
    return new Promise((resolve, reject) => {
        const sql = `UPDATE Users SET hidden='true' WHERE email='${email}'`;
        db.run(sql, [], function (err) {
            if (err) {
                reject(err);
            } else {
                resolve(this.changes);
            }
        });
    });
}
