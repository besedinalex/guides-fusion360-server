import {expect} from 'chai';
import fs from "fs-extra";
import {Database} from "sqlite3";
import db from "../../../src/db/api/sqlite3-connection";

describe('db/api/sqlite3-connection.ts', function () {
    it('should create data/ folder', function () {
        const databasePath = process.cwd() + '/data';
        expect(fs.pathExistsSync(databasePath)).to.be.true;
    });

    it('should create sqlite3 instance', function () {
        expect(db).to.be.instanceOf(Database);
    });
});