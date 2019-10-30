import express from 'express';
import cors from 'cors';

import {getGuides} from "./requests/http/guides";

const app = express();

app.use(cors());
app.listen(4000, () => console.log('Server is successfully started!'));

app.get('/home', function (req, res) {
    getGuides(res);
});
