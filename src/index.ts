import express from 'express';
import cors from 'cors';
import path from 'path';
import guide from './routes/guides';
import user from './routes/user';
import annotations from "./routes/model-annotations";

const app = express();
const projectRoot = path.join(__dirname, '..');

// Some security stuff
app.use(cors());

// Requests
app.use('/guides', guide);
app.use('/user', user);
app.use('/annotations', annotations);

// Public access files
app.use('/', express.static(projectRoot + '/public'));
app.use('/storage', express.static(projectRoot + '/data/storage'));
app.get('*', (req, res) => res.sendFile(projectRoot + '/public/index.html'));

app.listen(4004, () => console.log('Server is started.'));
