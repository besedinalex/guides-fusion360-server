import {expect} from 'chai';
import request from 'request';

const url = 'http://localhost:4004/user';
const email = 'besedinalexey@gmail.com';
const newUserRequest = `${url}/new?email=${email}&firstName=a&lastName=b&password=qwerty&access=user`;

describe('User requests: /user', function () {
    describe('Successful', function () {
        it('POST /new', function (done) {
            request.post(newUserRequest, function (err, res, body) {
                expect(res.statusCode).to.equal(200);
                done();
            });
        });

        it('GET /token', function (done) {
            request(`${url}/token?email=${email}&password=qwerty`, function (err, res, body) {
                expect(res.statusCode).to.equal(200);
                done();
            })
        });
    });

    describe('Unsuccessful', function () {
        it('POST /new (existing user)', function (done) {
            request.post(newUserRequest, function (err, res, body) {
                expect(res.statusCode).to.equal(401);
                done();
            })
        });

        it('GET /token (wrong password)', function (done) {
            request(`${url}/token?email=${email}&password=anything`, function (err, res, body) {
                expect(res.statusCode).to.equal(401);
                done();
            })
        });
    });
});
