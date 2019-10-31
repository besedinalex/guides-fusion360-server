import express from 'express';
import cors from 'cors';

import {getGuides, getPartGuides} from "./requests/http/guides";

const app = express();

// Server settings

app.use(cors());
app.use('/images', express.static(projectFolderPath() + '/assets/images'));
app.use('/models', express.static(projectFolderPath() + '/assets/models'));

app.listen(4000, () => console.log('Server is started.'));

// HTTP requests

app.get('/home', function (req, res) {
    getGuides(res);
});

app.get('/guide', function (req, res) {
    getPartGuides(req.query.guideId, res);
});

// Additional functions

function projectFolderPath(): string {
    const path = process.argv[1].split('/');
    const file = path[path.length - 1];
    const extension = file.split('.')[1];
    if (extension === 'ts') {
        return __dirname.slice(0, __dirname.length - 4);
    } else {
        return __dirname.slice(0, __dirname.length - 5);
    }
}
