import express from 'express';
import multer from 'multer';
import path from 'path';
import fs from 'fs-extra';
import {
    insertGuide,
    insertPartGuide,
    selectGuides,
    selectPartGuide,
    selectPartGuides,
    updatePartGuide, updatePartGuideSortKey
} from "../db/guides";
import Guide from "../interfaces/guide";
import PartGuide from "../interfaces/part-guide";
import {tokenToUserId} from "../access-check";
import {selectUserAccess} from "../db/user";

const guide = express.Router();
const upload = multer({storage: multer.memoryStorage()});
const storagePath = path.join(__dirname, '..', '..', 'data/storage'); // Temp

guide.get('/all', (req, res) => {
    selectGuides().then((data: Guide[]) => res.json(data));
});

guide.get('/parts', (req, res) => {
    selectPartGuides(req.query.guideId).then((data: PartGuide[]) => res.json(data));
});

guide.post('/guide', [tokenToUserId, upload.single('img')], (req, res) => {
    selectUserAccess(req.userId).then(access => {
        // if (access === 'student' || access === 'admin') {
            insertGuide(req.body.name, req.body.description, req.userId)
                .then(id => {

                    const imgName = 'preview' + path.extname(req.file.originalname);
                    const imgPath = path.join(storagePath, String(id), imgName);
                    fs.outputFileSync(imgPath, req.file.buffer, {flag: 'wx'});
                    res.sendStatus(200);
                }).catch(() => res.sendStatus(500));
        // } else {
        //     res.sendStatus(403);
        // }
    }).catch(() => res.sendStatus(401));

});

guide.post('/part-guide', [tokenToUserId, upload.single('file')], (req, res) => {
    // selectUserAccess(req.userId).then(access => {
    //     if (access === 'student' || access === 'admin') {

    //     } else {
    //         res.sendStatus(403);
    //     }
    // }).catch(() => res.sendStatus(401));

    const noFile = req.body.file !== undefined; // It is intended to be that way
    if (!noFile) {
        const filePath = path.join(storagePath, String(req.body.guideId), req.file.originalname);
        if (fs.existsSync(filePath)) {
            res.status(500).send('File exists already');
            return;
        }
        fs.outputFileSync(filePath, req.file.buffer, {flag: 'wx'});
    }
    const content = noFile ? req.body.file : req.file.originalname;
    insertPartGuide(req.body.guideId, req.body.name, content, req.body.sortKey)
        .then(() => res.sendStatus(200)).catch(() => res.sendStatus(500));
});

guide.put('/part-guide', [tokenToUserId, upload.single('file')], (req, res) => {
    const noFile = req.body.file !== undefined; // It is intended to be that way
    if (!noFile) {
        selectPartGuide(req.body.id).then(guide => {
            const filePath = path.join(storagePath, String(req.body.guideId), req.file.originalname);
            if (fs.existsSync(filePath)) {
                if (req.file.originalname === guide.content) {
                    fs.removeSync(filePath);
                } else {
                    res.status(500).send('Another guide file');
                    return;
                }
            }
            fs.outputFileSync(filePath, req.file.buffer, {flag: 'wx'});
        });
    }
    const content = noFile ? req.body.file : req.file.originalname;
    updatePartGuide(req.body.id, req.body.name, content)
        .then(() => res.sendStatus(200)).catch(() => res.sendStatus(500));
});

guide.put('/part-guides-sort-key', tokenToUserId, (req, res) => {
    updatePartGuideSortKey(req.query.id1, req.query.sortKey1)
        .then(() =>
            updatePartGuideSortKey(req.query.id2, req.query.sortKey2)
                .then(() => res.sendStatus(200)).catch(() => res.sendStatus(500)))
        .catch(() => res.sendStatus(500));
})

export default guide;
