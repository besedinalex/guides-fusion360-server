import fs from "fs-extra";

const cwd = process.cwd();

if (fs.existsSync(cwd + '/data')) {
    fs.renameSync(cwd + '/data', cwd + '/data_temp');
}
