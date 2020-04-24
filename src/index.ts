import express from 'express';
import cors from 'cors';
import guide from './routes/guides';
import user from './routes/user';
import annotations from "./routes/model-annotations";

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
app.use('/storage', express.static(projectFolderPath + '/data/storage'));
app.get('*', (req, res) => res.sendFile(projectFolderPath + '/public/index.html'));

app.listen(4004, () => console.log('Server is started.'));
