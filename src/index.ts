import express from 'express';
import cors from 'cors';
import path from 'path';
import guides from './routes/guides';
import user from './routes/user';
import annotations from "./routes/model-annotations";
const {PORT} = require(process.cwd() + '/config.json');

const app = express();

// Some security stuff
app.use(cors());

// Requests
app.use('/guides', guides);
app.use('/user', user);
app.use('/annotations', annotations);

// Public access files
app.use('/', express.static(path.join(__dirname, '..', 'public')));
app.use('/storage', express.static(process.cwd() + '/data/storage'));
app.get('*', (req, res) => res.sendFile(path.join(__dirname, '..', 'public', 'index.html')));

app.listen(PORT, () => console.log(`Server is listening port ${PORT}`));
