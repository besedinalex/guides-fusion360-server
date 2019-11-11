const jwt = require('jsonwebtoken');
const userData = require('../db/user');

const secret = 'Tolstikoff';

// exports.checkToken = function (req, res, next) {
//     const token = req.query.token;
//     if (!token) {
//         res.status(401).send('Unauthorized: No token provided');
//     } else {
//         jwt.verify(token, secret, function (err, decoded) {
//             if (err) {
//                 res.status(401).send('Unauthorized: Invalid token');
//             } else if (Date.now() >= decoded.exp * 1000) {
//                 res.status(401).send('Unauthorized: Token expired');
//             } else {
//                 req.user_id = decoded.id;
//                 next();
//             }
//         });
//     }
// };

exports.signInUser = function (email, password, res) {
    userData.signIn(email, password)
        .then(data => {
            const payload = {id: data.user_id};
            const token = jwt.sign(payload, secret, {expiresIn: '365d'});
            let expiresAt = Date.now() + +365 * 24 * 60 * 60 * 1000;
            res.json({token, expiresAt: expiresAt, userId: data.user_id});
            // TODO: Хранить токены в БД.
            // TODO: Дать юзеру возможность дропнуть токены.
            // TODO: Выдавать токен на несколько дней/месяцев в зависимости от галочки в поле "запомнить меня".
        })
        .catch(err => res.status(401).send(err.message));
};

exports.signUpUser = function (firstName, lastName, email, password, res) {
    userData.signUp(firstName, lastName, email.toLowerCase(), password)
        .then(userId => {
            const payload = {id: userId};
            const token = jwt.sign(payload, secret, {expiresIn: '365d'});
            let expiresAt = Date.now() + +365 * 24 * 60 * 60 * 1000;
            res.json({token, expiresAt: expiresAt, userId: userId});
        })
    // TODO: Выдавать ошибку в Front-End, если юзер уже существует.
};

exports.getUserData = function (userId, res) {
    userData.getUser(userId).then(data => res.json(data))
};

exports.getUserModels = function (userId, res) {
    userData.getUserModels(userId).then(data => res.json(data));
};
