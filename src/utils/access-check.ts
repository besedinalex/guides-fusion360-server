import jwt from "jsonwebtoken";
import {selectUserAccess} from "../db/user";

export const jwtSecret = 'Tolstikoff';

export function tokenToUserId(req, res, next) {
    const token = req.query.token;
    if (!token) {
        res.status(401).send('Unauthorized: No token provided');
    } else {
        jwt.verify(token, jwtSecret, function (err, decoded) {
            if (err) {
                res.status(401).send('Unauthorized: Invalid token');
            } else if (Date.now() >= decoded.exp * 1000) {
                res.status(403).send('Unauthorized: Token expired');
            } else {
                req.userId = decoded.id;
                next();
            }
        });
    }
}

export function userEditAccess(userId: number, access: string, result: (hasAccess: boolean, code?: number, message?: string) => void) {
    selectUserAccess(userId)
        .then(data => {
            switch (access) {
                case 'admin':
                    result(data.access === 'admin');
                    break;
                case 'editor':
                    result(data.access === 'admin' || data.access === 'editor');
                    break;
                default:
                    result(false, 403, 'Недостаточно прав доступа');
                    break
            }
        })
        .catch(() => result(false, 404, 'Пользователь не найден'));
}
