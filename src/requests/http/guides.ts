import express from 'express';
import {selectGuideCode, selectGuides, selectPartGuides} from "../db/guides";
import Guide from "../../interfaces/guide";
import PartGuide from "../../interfaces/part-guide";

const guide = express.Router();

guide.get('/all', (req, res) => {
    selectGuides().then((data: Guide[]) => res.json(data));
});

guide.get('/img', (req, res) => {
    selectGuideCode(req.query.guideId).then((data: string) => res.json(data));
});

guide.get('/parts', (req, res) => {
    selectPartGuides(req.query.guideId).then((data: PartGuide[]) => res.json(data));
});

export default guide;
