import express from 'express';
import multer from 'multer';
import {tokenToUserId} from "../utils/access-check";
import {
    changePartGuide,
    changePartGuideSortKeys,
    createNewGuide,
    createNewPartGuide,
    getAllGuides,
    getAllPartGuides
} from "../services/guides";

const guides = express.Router();
const upload = multer({storage: multer.memoryStorage()});

guides.get('/all', (req, res) => {
    getAllGuides((code, json) => res.status(code).send(json));
});

guides.get('/parts', (req, res) => {
    const {guideId} = req.query;
    getAllPartGuides(guideId, (code, json) => res.status(code).send(json));
});

guides.post('/guide', [tokenToUserId, upload.single('img')], (req, res) => {
    const {userId, body, file} = req;
    createNewGuide(userId, body, file, (code, json) => res.status(code).send(json));
});

guides.post('/part-guide', [tokenToUserId, upload.single('file')], (req, res) => {
    const {body, file} = req;
    createNewPartGuide(body, file, (code, json) => res.status(code).send(json));
});

guides.put('/part-guide', [tokenToUserId, upload.single('file')], (req, res) => {
    const {body, file} = req;
    changePartGuide(body, file, (code, json) => res.status(code).send(json));
});

guides.put('/switch', tokenToUserId, (req, res) => {
    const {id1, id2} = req.query;
    changePartGuideSortKeys(id1, id2, (code, json) => res.status(code).send(json));
})

export default guides;
