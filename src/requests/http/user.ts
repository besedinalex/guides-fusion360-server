import express from 'express';
import jwt from 'jsonwebtoken';
import {signUp, signIn} from '../db/user';
import {encrypt, decrypt} from "../../crypto";

const user = express.Router();
const secret = 'Tolstikoff'; // TODO: Update before deploy.

user.get('/token', (req, res) => {
    signIn(req.query.email)
        .then(user => {
            if (decrypt(user.password) !== req.query.password) {
                res.sendStatus(401);
            } else {
                const payload = {id: user.id};
                const token = jwt.sign(payload, secret, {expiresIn: '30d'});
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
    signUp(email.toLowerCase(), firstName, lastName, group, encrypt(password))
        .then(userId => {
            const payload = {id: userId};
            const token = jwt.sign(payload, secret, {expiresIn: '30d'});
            let expiresAt = Date.now() + +30 * 24 * 60 * 60 * 1000;
            res.json({token, expiresAt: expiresAt});
        }).catch(() => res.sendStatus(401));
});

export default user;

export function checkToken(req, res, callback) {
    const token = req.query.token;
    if (!token) {
        res.status(401).send('Unauthorized: No token provided');
    } else {
        jwt.verify(token, secret, function (err, decoded) {
            if (err) {
                res.status(401).send('Unauthorized: Invalid token');
            } else if (Date.now() >= decoded.exp * 1000) {
                res.status(403).send('Unauthorized: Token expired');
            } else {
                const userId = decoded.id;
                callback(userId);
            }
        });
    }
}
