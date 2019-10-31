import express from 'express';
import cors from 'cors';

import {getGuides, getPartGuides} from "./requests/http/guides";

const app = express();

app.use(cors());
app.use('/images', express.static(projectFolderPath() + '/assets/images'));

function projectFolderPath(): string {
    const folders = __dirname.split('/');
    folders.splice(folders.length - 1, 1);
    folders.splice(0, 1);
    let path = '';
    for (const folder of folders) {
        path += '/' + folder;
    }
    return path;
}

app.listen(4000, () => console.log('Server is started.'));

app.get('/home', function (req, res) {
    getGuides(res);
});

app.get('/guide', function (req, res) {
    getPartGuides(req.query.guideId, res);
});
