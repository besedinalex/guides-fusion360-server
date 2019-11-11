import crypto from 'crypto';

const algorithm = 'aes-256-ctr';
const password = 'Polytech';

export function encrypt(decryptedText: string): string {
    const cipher = crypto.createCipher(algorithm, password);
    let crypted = cipher.update(decryptedText, 'utf8', 'hex');
    crypted += cipher.final('hex');
    return crypted;
}

export function decrypt(cryptedText: string): string {
    const decipher = crypto.createDecipher(algorithm, password);
    let dec = decipher.update(cryptedText, 'hex', 'utf8');
    dec += decipher.final('utf8');
    return dec;
}
