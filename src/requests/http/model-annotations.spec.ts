import {expect} from 'chai';
import request from 'request';

it('Request successful: GET /annotation/all ', function (done) {
    request('http://localhost:4004/annotation/all', function (err, res, body) {
        expect(res.statusCode).to.equal(200);
        done();
    });
});
