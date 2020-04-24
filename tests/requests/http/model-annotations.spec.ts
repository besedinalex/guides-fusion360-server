import {expect} from 'chai';
import request from 'request';

describe('Model annotation requests: /annotation', function () {
    it('Successful GET /all ', function (done) {
        request('http://localhost:4004/annotation/all', function (err, res, body) {
            expect(res.statusCode).to.equal(200);
            done();
        });
    });
});

