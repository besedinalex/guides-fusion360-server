import express from 'express';
import {selectModelAnnotations} from "../db/model-annotations";
import ModelAnnotation from "../interfaces/model-annotation";

const annotations = express.Router();

annotations.get('/all', (req, res) => {
    selectModelAnnotations(req.query.guideId).then((data: ModelAnnotation[]) => res.json(data));
});

annotations.post('/new', (req, res) => {
    // TODO: Finish this.
});

export default annotations;
