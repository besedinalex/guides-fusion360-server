import express from 'express';
import multer from 'multer';
import {tokenToUserId} from "../utils/access-check";
import {
    changeGuideVisibility,
    changePartGuide,
    changePartGuideSortKeys,
    createNewGuide,
    createNewPartGuide,
    getAllGuides,
    getAllPartGuides, getHiddenGuides, getHiddenPartGuides, uploadModel
} from "../services/guides";

const guides = express.Router();
const upload = multer({storage: multer.memoryStorage()});

guides.get('/all', (req, res) => {
    getAllGuides((code, json) => res.status(code).send(json));
});

guides.get('/all-hidden', tokenToUserId, (req, res) => {
    // @ts-ignore
    const {userId} = req;
    getHiddenGuides(userId, (code, json) => res.status(code).send(json));
});

guides.get('/parts', (req, res) => {
    const {guideId} = req.query;
    getAllPartGuides(guideId, (code, json) => res.status(code).send(json));
});

guides.get('/parts-hidden', tokenToUserId, (req, res) => {
    // @ts-ignore
    const {userId} = req;
    const {guideId} = req.query;
    getHiddenPartGuides(userId, guideId, (code, json) => res.status(code).send(json));
});

guides.post('/guide', [tokenToUserId, upload.single('img')], (req, res) => {
    const {userId, body, file} = req;
    createNewGuide(userId, body, file, (code, json) => res.status(code).send(json));
});

guides.post('/part-guide', [tokenToUserId, upload.single('file')], (req, res) => {
    const {userId, body, file} = req;
    createNewPartGuide(userId, body, file, (code, json) => res.status(code).send(json));
});

guides.post('/model', [tokenToUserId, upload.single('file')], (req, res) => {
    const {userId, body, file} = req;
    uploadModel(userId, body.guideId, file, (code, json) => res.status(code).send(json));
})

guides.put('/hidden', tokenToUserId, (req, res) => {
    // @ts-ignore
    const {userId} = req;
    const {guideId, hidden} = req.query;
    changeGuideVisibility(userId, guideId, hidden, (code, json) => res.status(code).send(json));
});

guides.put('/part-guide', [tokenToUserId, upload.single('file')], (req, res) => {
    const {userId, body, file} = req;
    changePartGuide(userId, body, file, (code, json) => res.status(code).send(json));
});

guides.put('/switch', tokenToUserId, (req, res) => {
    // @ts-ignore
    const {userId} = req;
    const {id1, id2} = req.query;
    changePartGuideSortKeys(userId, id1, id2, (code, json) => res.status(code).send(json));
})

export default guides;
