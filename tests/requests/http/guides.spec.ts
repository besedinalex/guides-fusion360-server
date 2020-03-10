import {expect} from 'chai';
import request from 'request';

describe('Guide requests: /guides', function () {
    it('Request successful: GET /all ', function (done) {
        request('http://localhost:4004/guides/all', function (err, res, body) {
            expect(res.statusCode).to.equal(200);
            done();
        });
    });

    it('Request successful: GET /img?guideId=1 ', function (done) {
        request('http://localhost:4004/guides/img?guideId=1', function (err, res, body) {
            expect(res.statusCode).to.equal(200);
            done();
        });
    });

    it('Request successful: GET /parts?guideId=1 ', function (done) {
        request('http://localhost:4004/guides/parts?guideId=1', function (err, res, body) {
            expect(res.statusCode).to.equal(200);
            done();
        });
    });
});
