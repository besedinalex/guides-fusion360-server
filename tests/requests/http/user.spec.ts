import {expect} from 'chai';
import request from 'request';
import {removeUser} from "../../../src/requests/db/user";

const url = 'http://localhost:4004/user';
const email = 'besedinalexey@gmail.com';
const newUserRequest = `${url}/new?email=${email}&firstName=a&lastName=b&group=f&password=qwerty`;

describe('User requests: /user', function () {
    it('Request successful: POST /new', function (done) {
        request.post(newUserRequest, function (err, res, body) {
            expect(res.statusCode).to.equal(200);
            done();
        });
    });

    it('Request unsuccessful: POST /new (same email)', function (done) {
        request.post(newUserRequest, function (err, res, body) {
            expect(res.statusCode).to.equal(401);
            done();
        })
    });

    it('Request successful: GET /token', function (done) {
        request(`${url}/token?email=${email}&password=qwerty`, function (err, res, body) {
            expect(res.statusCode).to.equal(200);
            done();
        })
    });

    it('Request unsuccessful: GET /token (wrong password)', function (done) {
        request(`${url}/token?email=${email}&password=anything`, function (err, res, body) {
            expect(res.statusCode).to.equal(401);
            done();
        })
    });

    // TODO: Remove this temp decision
    it('NOT A TEST: removing test data from db', function (done) {
        removeUser(email);
        expect(true).to.be.true;
        done();
    });
});
