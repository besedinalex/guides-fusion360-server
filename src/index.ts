import express from 'express';
import cors from 'cors';

export const app = express();

app.use(cors());
app.listen(4000, () => console.log('Server is successfully started!'));
