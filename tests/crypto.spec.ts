import {expect} from 'chai';
import {encrypt, decrypt} from "../src/utils/crypto";

describe('Crypto', function () {
    it('Encrypts text', function (done) {
        const textToEncrypt = 'Besedin';
        const encryptedText = encrypt(textToEncrypt);
        const regExp = /Besedin/;
        expect(encryptedText.search(regExp)).to.equal(-1);
        done();
    });

    it('Decrypts text', function (done) {
        expect(decrypt('147b8b55cb736cd303f2da029d47599c:4cd07e981df029')).to.equal('Besedin');
        done();
    });
});

