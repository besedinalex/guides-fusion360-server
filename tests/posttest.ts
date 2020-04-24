import fs from "fs-extra";

const cwd = process.cwd();

if (fs.existsSync(cwd + '/data_temp')) {
    fs.removeSync(cwd + '/data');
    fs.renameSync(cwd + '/data_temp', cwd + '/data');
}