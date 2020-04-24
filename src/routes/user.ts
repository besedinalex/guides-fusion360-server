import express from 'express';
import {createNewUser, getUserLoginData} from "../services/user";

const user = express.Router();

user.get('/token', (req, res) => {
    const {email, password} = req.query;
    getUserLoginData(email, password, (code, json) => res.status(code).send(json));
});

user.post('/new', (req, res) => {
    const {email, password, firstName, lastName, group} = req.query;
    createNewUser(email, password, firstName, lastName, group,
        (code, json) => res.status(code).send(json));
});

// user.get('/data', tokenToUserId, (req, res) => {
//     // @ts-ignore
//     selectUserAccess(req.userId)
//         .then(access => {
//             if (access === 'admin') {
//                 selectUserData(req.query.userId)
//                     .then(data => res.json(data))
//                     .catch(() => res.sendStatus(404));
//             } else {
//                 res.sendStatus(403);
//             }
//         })
//         .catch(() => res.sendStatus(500));
// });

// user.get('/data-self', tokenToUserId, (req, res) => {
//     // @ts-ignore
//     const id = req.userId;
//     selectUserData(id)
//         .then(data => {
//             if (data.id === id) {
//                 res.json(data);
//             } else {
//                 res.sendStatus(401);
//             }
//         })
//         .catch(() => res.sendStatus(404));
// });

// user.delete('/self', tokenToUserId, (req, res) => {
//     selectUserLoginData(req.query.email).then(user => {
//         // @ts-ignore
//         if (user.id === req.userId) {
//             deleteUser(req.query.email)
//                 .then(() => res.sendStatus(200))
//                 .catch(() => res.sendStatus(500));
//         } else {
//             res.sendStatus(403);
//         }
//     }).catch(() => res.sendStatus(404));
// });

export default user;
