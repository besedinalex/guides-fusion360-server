import {selectGuides} from "../db/guides";
import Guide from "../../interfaces/guide";

export function getGuides(res) {
    selectGuides().then((data: Guide[]) => res.json(data));
}
