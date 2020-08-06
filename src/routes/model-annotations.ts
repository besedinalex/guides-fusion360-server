import express from 'express';
import {getAllAnnotations} from "../services/model-annotations";

const annotations = express.Router();

annotations.get('/all', (req, res) => {
    const {guideId} = req.query;
    getAllAnnotations(guideId, (code, json) => res.status(200).send(json));
});

export default annotations;
