export default interface Guide {
    id: number;
    name: string;
    description: string;
    ownerId?: number;
    hidden?: string;
}
