import {expect} from 'chai';
import request from 'request';

it('Request successful: GET /* (React page)', function (done) {
    request('http://localhost:4004', function (err, res, body) {
        expect(res.statusCode).to.equal(200);
        done();
    });
});
