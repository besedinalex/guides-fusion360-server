import {selectGuideImage, selectGuides, selectPartGuides} from "../db/guides";
import Guide from "../../interfaces/guide";
import PartGuide from "../../interfaces/part-guide";

import express from 'express';

const guide = express.Router();

guide.get('/all', function (req, res) {
    selectGuides().then((data: Guide[]) => res.json(data));
});

guide.get('/img', function (req, res) {
    selectGuideImage(req.query.guideId).then((data: string) => res.json(data));
});

guide.get('/parts', function (req, res) {
    selectPartGuides(req.query.guideId).then((data: PartGuide[]) => res.json(data));
});

export default guide;
