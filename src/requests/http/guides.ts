import {selectGuideImage, selectGuides, selectPartGuides} from "../db/guides";
import Guide from "../../interfaces/guide";
import PartGuide from "../../interfaces/part-guide";

export function getGuides(res) {
    selectGuides().then((data: Guide[]) => res.json(data));
}

export function getGuideImg(guideId: number, res) {
    selectGuideImage(guideId).then((data: string) => res.json(data));
}

export function getPartGuides(guideId: number, res) {
    selectPartGuides(guideId).then((data: PartGuide[]) => res.json(data));
}
