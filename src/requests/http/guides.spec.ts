import {expect} from 'chai';
import request from 'request';

it('Request successful: GET /guides/all ', function (done) {
    request('http://localhost:4004/guides/all', function (err, res, body) {
        expect(res.statusCode).to.equal(200);
        done();
    });
});

it('Request successful: GET /guides/img?guideId=1 ', function (done) {
    request('http://localhost:4004/guides/img?guideId=1', function (err, res, body) {
        expect(res.statusCode).to.equal(200);
        done();
    });
});

it('Request successful: GET /guides/parts?guideId=1 ', function (done) {
    request('http://localhost:4004/guides/parts?guideId=1', function (err, res, body) {
        expect(res.statusCode).to.equal(200);
        done();
    });
});
