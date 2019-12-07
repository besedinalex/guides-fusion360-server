import express from 'express';
import cors from 'cors';
import guide from './requests/http/guides';
import user from './requests/http/user';
import annotations from "./requests/http/model-annotations";

const app = express();

app.use(cors());
app.use('/guide', guide);
app.use('/user', user);
app.use('/annotations', annotations);
app.use('/images', express.static(projectFolderPath() + '/assets/images'));
app.use('/models', express.static(projectFolderPath() + '/assets/models'));

app.listen(4000, () => console.log('Server is started.'));

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
