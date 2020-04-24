import express from 'express';
import jwt from 'jsonwebtoken';
import {deleteUser, insertNewUser, selectUserAccess, selectUserData, selectUserLoginData} from '../db/user';
import {encrypt, decrypt} from "../utils/crypto";
import {jwtSecret, tokenToUserId} from "../utils/access-check";

const user = express.Router();

user.get('/data', tokenToUserId, (req, res) => {
    // @ts-ignore
    selectUserAccess(req.userId)
        .then(access => {
            if (access === 'admin') {
                selectUserData(req.query.userId)
                    .then(data => res.json(data))
                    .catch(() => res.sendStatus(404));
            } else {
                res.sendStatus(403);
            }
        })
        .catch(() => res.sendStatus(500));
});

user.get('/data-self', tokenToUserId, (req, res) => {
    // @ts-ignore
    const id = req.userId;
    selectUserData(id)
        .then(data => {
            if (data.id === id) {
                res.json(data);
            } else {
                res.sendStatus(401);
            }
        })
        .catch(() => res.sendStatus(404));
});

user.get('/token', (req, res) => {
    selectUserLoginData(req.query.email)
        .then(user => {
            if (decrypt(user.password) !== req.query.password) {
                res.sendStatus(401);
            } else {
                const payload = {id: user.id};
                const token = jwt.sign(payload, jwtSecret, {expiresIn: '30d'});
                let expiresAt = Date.now() + +30 * 24 * 60 * 60 * 1000;
                res.json({token: token, expiresAt: expiresAt});
            }
        })
        .catch(() => res.sendStatus(401));
});

user.post('/new', (req, res) => {
    const email = req.query.email;
    const firstName = req.query.firstName;
    const lastName = req.query.lastName;
    const group = req.query.group;
    const password = req.query.password;
    insertNewUser(email.toLowerCase(), firstName, lastName, group, encrypt(password))
        .then(userId => {
            const payload = {id: userId};
            const token = jwt.sign(payload, jwtSecret, {expiresIn: '30d'});
            let expiresAt = Date.now() + +30 * 24 * 60 * 60 * 1000;
            res.json({token, expiresAt: expiresAt});
        }).catch(() => res.sendStatus(401));
});

user.delete('/self', tokenToUserId, (req, res) => {
    selectUserLoginData(req.query.email).then(user => {
        // @ts-ignore
        if (user.id === req.userId) {
            deleteUser(req.query.email)
                .then(() => res.sendStatus(200))
                .catch(() => res.sendStatus(500));
        } else {
            res.sendStatus(403);
        }
    }).catch(() => res.sendStatus(404));
});

export default user;

