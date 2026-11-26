
export interface StateRecord {
    id:string;
    name:string;
    emoji:string;
    walkingDistance:number;
    totalPoints:number;
    places:places[];
}

export interface places {
    id:string;
    name:string;
    icon:string;
    description:string;
    visited:boolean;
    distance:number;
    points:number;
    stateId:string;
}
