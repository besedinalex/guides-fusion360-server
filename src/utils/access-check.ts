import jwt from "jsonwebtoken";

const jwtSecret = 'Tolstikoff'; // TODO: Update before deploy.

function tokenToUserId(req, res, next) {
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

export {jwtSecret, tokenToUserId}
