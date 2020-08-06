import path from "path";
import fs from "fs-extra";
import request from "request";
import {
    insertGuide,
    insertPartGuide, selectGuideAccess,
    selectGuides,
    selectPartGuide,
    selectPartGuides, updateGuideHidden,
    updatePartGuide,
    updatePartGuideSortKey
} from "../db/guides";
import {userEditAccess} from "../utils/access-check";
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

const storagePath = path.join(process.cwd(), 'data', 'storage');

export function getAllGuides(result: (code: number, json: object) => void) {
    selectGuides(false)
        .then((data: Guide[]) => result(200, data))
        .catch(() => result(404, {message: 'Гайды не найдены'}));
}

export function getHiddenGuides(userId: number, result: (code: number, json: object) => void) {
    userEditAccess(userId, 'editor', (hasAccess, code, message) => {
        if (hasAccess) {
            selectGuides(true)
                .then((guides: Guide[]) => result(200, guides))
                .catch(() => result(404, {message: 'Гайды не найдены'}));
        } else {
            result(code as number, {message});
        }
    });
}

export function getAllPartGuides(guideId: number, result: (code: number, json: object) => void) {
    selectPartGuides(guideId)
        .then((data: PartGuide[]) => result(200, data))
        .catch(() => result(404, {message: 'Гайды не найдены'}));
}

export function createNewGuide(userId: number, body: GuideBody, file: Express.Multer.File, result: (code: number, json: object) => void) {
    const {name, description} = body;
    userEditAccess(userId, 'editor', hasAccess => {
        if (hasAccess) {
            insertGuide(name, description, userId)
                .then(id => {
                    const imgName = 'preview' + path.extname(file.originalname);
                    const imgPath = path.join(storagePath, String(id), imgName);
                    fs.outputFileSync(imgPath, file.buffer, {flag: 'wx'});
                    result(200, {id});
                })
                .catch(() => result(500, {message: 'Не удалось создать гайд'}));
        } else {
            result(403, {message: 'Недостаточно прав доступа'});
        }
    })
}

export function createNewPartGuide(userId: number, body: PartGuideBody, file: Express.Multer.File, result: (code: number, json: object) => void) {
    userEditAccess(userId, 'editor', (hasAccess, code, message) => {
        if (hasAccess) {
            selectGuideAccess(body.guideId)
                .then(data => {
                    if (data.hidden === 'true') {
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
                    } else {
                        result(403, {message: 'Нельзя редактировать публичный гайд, сначала администратор должен его скрыть'});
                    }
                })
                .catch(() => result(404, {message: 'Гайд не найден'}));
        } else {
            result(code as number, {message});
        }
    });
}

export function uploadModel(userId: number, guideId: string, file: Express.Multer.File, result: (code: number, json: object) => void) {
    userEditAccess(userId, 'editor', (hasAccess, code, message) => {
        if (hasAccess) {
            selectGuideAccess(+guideId)
                .then(data => {
                    if (data.hidden === 'true') {
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
                    } else {
                        result(403, {message: 'Нельзя редактировать публичный гайд, сначала администратор должен его скрыть'});
                    }
                })
                .catch(() => result(404, {message: 'Гайд не найден'}));
        } else {
            result(code as number, {message});
        }
    });
}

export function changeGuideVisibility(userId: number, guideId: number, hidden: string, result: (code: number, json: object) => void) {
    userEditAccess(userId, 'admin', (hasAccess, code, message) => {
        if (hasAccess) {
            updateGuideHidden(guideId, hidden)
                .then(() => result(200, {}))
                .catch(() => result(500, {message: 'Не удалось поменять видимость гайда'}));
        } else {
            result(code as number, {message});
        }
    });
}

export function changePartGuide(userId: number, body: PartGuideBody, file: Express.Multer.File, result: (code: number, json: object) => void) {
    userEditAccess(userId, 'editor', (hasAccess, code, message) => {
        if (hasAccess) {
            selectGuideAccess(body.guideId)
                .then(data => {
                    if (data.hidden === 'true') {
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
                    } else {
                        result(403, {message: 'Нельзя редактировать публичный гайд, сначала администратор должен его скрыть'});
                    }
                })
                .catch(() => result(404, {message: 'Гайд не найден'}));
        } else {
            result(code as number, {message});
        }
    })
}

export function changePartGuideSortKeys(userId: number, id1: number, id2: number, result: (code: number, json) => void) {
    userEditAccess(userId, 'editor', (hasAccess, code, message) => {
        if (hasAccess) {
            selectPartGuide(id1)
                .then(guide1 => {
                    selectPartGuide(id2)
                        .then(guide2 => {
                            if (guide1.guideId === guide2.guideId) {
                                selectGuideAccess(guide1.guideId as number)
                                    .then(data => {
                                        if (data.hidden === 'true') {
                                            updatePartGuideSortKey(id1, guide2.sortKey)
                                                .then(() => {
                                                    updatePartGuideSortKey(id2, guide1.sortKey)
                                                        .then(() => result(200, {}))
                                                        .catch(() => result(500, {message: 'Не удалось изменить порядок гайдов'}));
                                                })
                                                .catch(() => result(500, {message: 'Не удалось изменить порядок гайдов'}));
                                        } else {
                                            result(403, {message: 'Нельзя редактировать публичный гайд, сначала администратор должен его скрыть'});
                                        }
                                    })
                                    .catch(() => result(404, {message: 'Гайд не найден'}));

                            } else {
                                result(409, {message: 'Данный гайды не связаны между собой'});
                            }
                        })
                        .catch(() => result(404, {message: 'Не удалось найти гайды'}));
                })
                .catch(() => result(404, {message: 'Не удалось найти гайды'}));
        } else {
            result(code as number, {message});
        }
    });
}
