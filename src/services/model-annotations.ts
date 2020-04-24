import {selectModelAnnotations} from "../db/model-annotations";
import ModelAnnotation from "../interfaces/model-annotation";

function getAllAnnotations(guideId: number, result: (code: number, json: object) => void) {
    selectModelAnnotations(guideId)
        .then((data: ModelAnnotation[]) => result(200, data))
        .catch(() => result(404, {message: 'Аннотации не найдены'}));
}

export {
    getAllAnnotations
}