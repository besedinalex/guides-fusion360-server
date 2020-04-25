import path from "path";
import fs from "fs-extra";
import request from "request";
import {
    insertGuide,
    insertPartGuide,
    selectGuides,
    selectPartGuide,
    selectPartGuides,
    updatePartGuide,
    updatePartGuideSortKey
} from "../db/guides";
import {selectUserAccess} from "../db/user";
import Guide from "../interfaces/guide";
import PartGuide from "../interfaces/part-guide";

interface GuideBody {
    name: string;
    description: string;
}

interface PartGuideBody {
    id?: number
    guideId: number;
    name: string;
    sortKey: number;
    file?: string;
}

const storagePath = path.join(process.cwd(), 'data/storage');

function getAllGuides(result: (code: number, json: object) => void) {
    selectGuides()
        .then((data: Guide[]) => result(200, data))
        .catch(() => result(404, {message: 'Гайды не найдены'}));
}

function getAllPartGuides(guideId: number, result: (code: number, json: object) => void) {
    selectPartGuides(guideId)
        .then((data: PartGuide[]) => result(200, data))
        .catch(() => result(404, {message: 'Гайды не найден'}));
}

function createNewGuide(userId: number, body: GuideBody, file: Express.Multer.File, result: (code: number, json: object) => void) {
    const {name, description} = body;
    selectUserAccess(userId)
        .then(data => {
            insertGuide(name, description, userId)
                .then(id => {
                    const imgName = 'preview' + path.extname(file.originalname);
                    const imgPath = path.join(storagePath, String(id), imgName);
                    fs.outputFileSync(imgPath, file.buffer, {flag: 'wx'});
                    result(200, {id});
                })
                .catch(() => result(500, {message: 'Не удалось создать гайд'}));
        })
        .catch(() => result(404, {message: 'Пользователь не найден'}));
}

function createNewPartGuide(body: PartGuideBody, file: Express.Multer.File, result: (code: number, json: object) => void) {
    const noFile = body.file !== undefined; // It is intended to be that way
    if (!noFile) {
        const filePath = path.join(storagePath, String(body.guideId), file.originalname);
        if (fs.existsSync(filePath)) {
            result(409, {message: 'Файл с таким именен уже существует в этом гайде'});
            return;
        }
        fs.outputFileSync(filePath, file.buffer, {flag: 'wx'});
    }
    const content = noFile ? body.file : file.originalname;
    insertPartGuide(body.guideId, body.name, content as string, body.sortKey)
        .then(id => result(200, {id}))
        .catch(() => result(500, {message: 'Не удалось создать гайд'}));
}

function uploadModel(guideId: string, file: Express.Multer.File, result: (code: number, json: object) => void) {
    if (!file) {
        result(400, {message: 'Вы не выбрали файл для загрузки'});
        return;
    } else if (path.extname(file.originalname).toLowerCase() !== '.stp') {
        result(400, {message: 'Вы должны загрузить STP файл'});
        return;
    }
    const tempFile = path.join(storagePath, guideId, 'temp.stp');
    fs.removeSync(tempFile);
    fs.outputFileSync(tempFile, file.buffer, {flag: 'wx'});
    const token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MSwiaWF0IjoxNTY2NDg1NTkxLCJleHAiOjE1OTgwMjE1OTF9.ykeUjEi8nDvAkW2XaRehYW9w8IvsupLPG6dzCETVGr0';
    request({
        method: 'POST',
        url: `http://195.133.144.86:4001/model?token=${token}&exportFormat=glb`, // Converter from http://mpu-cloud.ru
        headers: {'Content-Type': 'multipart/form-data'},
        formData: {'model': fs.createReadStream(tempFile)}
    }, function (err, res) {
        if (err || res === undefined || res.statusCode === 500) {
            result(500, {message: 'Не удалось конвертировать модель'});
        } else {
            const glbFile = path.join(storagePath, guideId, 'model.glb');
            fs.removeSync(glbFile);
            fs.outputFileSync(glbFile, Buffer.from(JSON.parse(res.body).data), {flag: 'wx'});
            result(200, {});
        }
        fs.removeSync(tempFile);
    });
}

function changePartGuide(body: PartGuideBody, file: Express.Multer.File, result: (code: number, json: object) => void) {
    const noFile = body.file !== undefined; // It is intended to be that way
    if (!noFile) {
        selectPartGuide(body.id as number)
            .then(guide => {
                const filePath = path.join(storagePath, String(body.guideId), file.originalname);
                if (fs.existsSync(filePath)) {
                    if (file.originalname === guide.content) {
                        fs.removeSync(filePath);
                    } else {
                        result(409, {message: 'Файл с таким именен уже существует в этом гайде'});
                        return;
                    }
                }
                fs.outputFileSync(filePath, file.buffer, {flag: 'wx'});
            });
    }
    const content = noFile ? body.file : file.originalname;
    updatePartGuide(body.id as number, body.name, content as string)
        .then(() => result(200, {}))
        .catch(() => result(500, {message: 'Не удалось обновить гайд'}));
}

function changePartGuideSortKeys(id1: number, id2: number, result: (code: number, json) => void) {
    selectPartGuide(id1)
        .then(guide1 => {
            selectPartGuide(id2)
                .then(guide2 => {
                    updatePartGuideSortKey(id1, guide2.sortKey)
                        .then(() => {
                            updatePartGuideSortKey(id2, guide1.sortKey)
                                .then(() => result(200, {}))
                                .catch(() => result(500, {message: 'Не удалось изменить порядок гайдов'}));
                        })
                        .catch(() => result(500, {message: 'Не удалось изменить порядок гайдов'}));
                })
                .catch(() => result(404, {message: 'Не удалось найти гайды'}));
        })
        .catch(() => result(404, {message: 'Не удалось найти гайды'}));
}

export {
    getAllGuides,
    getAllPartGuides,
    createNewGuide,
    createNewPartGuide,
    uploadModel,
    changePartGuide,
    changePartGuideSortKeys
}
