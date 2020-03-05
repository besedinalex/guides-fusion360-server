import express from 'express';
import cors from 'cors';
import guide from './requests/http/guides';
import user from './requests/http/user';
import annotations from "./requests/http/model-annotations";

const app = express();
const projectFolderPath = __dirname.replace('src', '');

// Some security stuff
app.use(cors());

// Requests
app.use('/guides', guide);
app.use('/user', user);
app.use('/annotations', annotations);

// Public access files
app.use('/', express.static(projectFolderPath + '/public'));
app.use('/images', express.static(projectFolderPath + '/data/images'));
app.use('/models', express.static(projectFolderPath + '/data/models'));
app.get('*', (req, res) => res.sendFile(projectFolderPath + '/public/index.html'));

app.listen(4004, () => console.log('Server is started.'));
