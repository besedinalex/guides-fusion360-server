import {selectModelAnnotations} from "../db/model-annotations";
import ModelAnnotation from "../../interfaces/model-annotation";

import express from 'express';

const annotations = express.Router();

annotations.get('/all', (req, res) => {
    selectModelAnnotations(req.query.guideId).then((data: ModelAnnotation[]) => res.json(data));
});

export default annotations;
