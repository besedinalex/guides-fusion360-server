import jwt from "jsonwebtoken";
import {decrypt, encrypt} from "../utils/crypto";
import {insertNewUser, selectUserLoginData} from "../db/user";
const {SECRET} = require(process.cwd() + '/config.json');

function getUserLoginData(email: string, password: string, result: (code: number, json: object) => void) {
    selectUserLoginData(email)
        .then(user => {
            if (decrypt(user.password) !== password) {
                result(401, {message: 'Неверный email или пароль'});
            } else {
                const payload = {id: user.id};
                const token = jwt.sign(payload, SECRET, {expiresIn: '30d'});
                result(200, {token});
            }
        })
        .catch(() => result(404, {message: 'Пользователь с таким email не найден'}));
}

function createNewUser(email: string, password: string, firstName: string, lastName: string, group: string, result: (code: number, json: object) => void) {
    insertNewUser(email.toLowerCase(), firstName, lastName, group, encrypt(password))
        .then(userId => {
            const payload = {id: userId};
            const token = jwt.sign(payload, SECRET, {expiresIn: '30d'});
            result(200, {token});
        })
        .catch(() => result(409, {message: 'Пользователь с таким email уже существует'}));
}

export {
    getUserLoginData,
    createNewUser
}