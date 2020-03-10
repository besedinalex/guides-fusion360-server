import {expect} from 'chai';
import {encrypt, decrypt} from "./crypto";

it('Encrypts text', function (done) {
    const textToEncrypt = 'Besedin';
    const encryptedText = encrypt(textToEncrypt);
    const regExp = /Besedin/;
    expect(encryptedText.search(regExp)).to.equal(-1);
    done();
});

it('Encrypts and decrypts text', function (done) {
    const textToEncrypt = 'Besedin';
    const encryptedText = encrypt(textToEncrypt);
    const decryptedText = decrypt(encryptedText);
    expect(decryptedText).to.equal(textToEncrypt);
    done();
});
