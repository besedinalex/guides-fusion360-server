import path from "path";
import fs from "fs-extra";
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

function createNewGuide(userId:number, body: GuideBody, file: Express.Multer.File, result: (code: number, json: object) => void) {
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
    changePartGuide,
    changePartGuideSortKeys
}