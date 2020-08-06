import crypto from 'crypto';

const encryptionKey = 'F7O3bbsUFEeRylaPLpZFwbOYpR9bRGXr'; // Random 32 chars TODO: Update before deploy
const ivLength = 16; // Always 16 for AES
const algorithm = 'aes-256-ctr';

function encrypt(text: string): string {
    const iv = crypto.randomBytes(ivLength);
    const cipher = crypto.createCipheriv(algorithm, Buffer.from(encryptionKey), iv);
    const encrypted =  Buffer.concat([cipher.update(text), cipher.final()]);
    return iv.toString('hex') + ':' + encrypted.toString('hex');
}

function decrypt(text: string): string {
    const textParts = text.split(':');
    const iv = Buffer.from(textParts.shift() as string, 'hex');
    const encryptedText = Buffer.from(textParts.join(':'), 'hex');
    const decipher = crypto.createDecipheriv(algorithm, Buffer.from(encryptionKey), iv);
    const decrypted = Buffer.concat([decipher.update(encryptedText), decipher.final()]);
    return decrypted.toString();
}

export {encrypt, decrypt}
