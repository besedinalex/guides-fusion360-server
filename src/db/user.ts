import {changeData, selectData} from "sqlite3-simple-api";

interface UserLoginData {
    id: number;
    password: string;
}

interface UserData {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
    studyGroup: string;
    access: string;
}

const userTable =
    `CREATE TABLE IF NOT EXISTS 'Users' (
    'id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    'email' TEXT NOT NULL UNIQUE,
    'password' TEXT NOT NULL,
    'firstName' TEXT NOT NULL,
    'lastName' TEXT NOT NULL,
    'access' TEXT NOT NULL,
    'studyGroup' TEXT
    );`;
changeData(userTable);

function selectUserLoginData(email: string): Promise<UserLoginData> {
    const sql = `SELECT U.id, U.password FROM Users AS U WHERE email = '${email}'`;
    return selectData(sql, true) as Promise<UserLoginData>;
}

function selectUserData(id: number): Promise<UserData> {
    const sql = `SELECT U.id, U.email, U.firstName, U.lastName, U.access, U.studyGroup FROM Users AS U WHERE id=${id}`;
    return selectData(sql, true) as Promise<UserData>;
}

function selectUserAccess(id: number): Promise<{access: string}> {
    const sql = `SELECT U.access FROM Users AS U WHERE id=${id}`;
    return selectData(sql, true) as Promise<{access: string}>;
}

function insertNewUser(email: string, firstName: string, lastName: string, group: string, password: string): Promise<number> {
    const sql =
        `INSERT INTO Users (firstName, lastName, email, password, studyGroup, access)
        VALUES ('${firstName}', '${lastName}', '${email}', '${password}', '${group}', 'unknown')`;
    return changeData(sql) as Promise<number>;
}

function updatePassword(email: string, password: string): Promise<number> {
    const sql = `UPDATE Users SET password='${password}' WHERE email='${email}'`;
    return changeData(sql) as Promise<number>;
}

function updateUserAccess(email: string, access: string): Promise<number> {
    const sql = `UPDATE Users SET access='${access}' WHERE email='${email}'`;
    return changeData(sql) as Promise<number>;
}

function updateUserData(email: string, firstName: string, lastName: string, studyGroup: string): Promise<number> {
    const sql =
        `UPDATE Users SET firstName='${firstName}', lastName='${lastName}', studyGroup='${studyGroup}'
        WHERE email='${email}'`;
    return changeData(sql) as Promise<number>;
}

function deleteUser(email: string): Promise<number> {
    const sql = `DELETE FROM Users WHERE email='${email}'`;
    return changeData(sql) as Promise<number>;
}

export {
    selectUserLoginData,
    selectUserData,
    selectUserAccess,
    insertNewUser,
    updatePassword,
    updateUserAccess,
    updateUserData,
    deleteUser
}
