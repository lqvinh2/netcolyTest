import {Schema, type, MapSchema, ArraySchema}  from "@colyseus/schema";

//  npx schema-codegen ./src/state.ts --csharp --output ../Assets/C#
export class UserJSON extends Schema {

    @type('string')
    name: string;

    @type(['float32'])
    position: ArraySchema<number> = new ArraySchema<number>();

    @type(['float32'])
    rotation: ArraySchema<number> = new ArraySchema<number>();

    @type('int32')
    health: number;
}

export class PositionJSON extends Schema {

    @type(['float32'])
    position: ArraySchema<number> = new ArraySchema<number>();
}


export class RotationJSON extends Schema {

    @type(['float32'])
    rotation: ArraySchema<number> = new ArraySchema<number>();
}


export class Player extends Schema {

    @type("boolean")
    isRoomMater: boolean = false;

    @type("boolean")
    isLocalPlayer: boolean = false;

    @type('string')
    room_name: string;

    @type('string')
    sessionId: string;

    @type('string')
    name: string;

    @type('int16')
    seat: number;

    @type('int16')
    health: number;

    @type(['float32'])
    position: ArraySchema<number> = new ArraySchema<number>();

    @type(['float32'])
    rotation: ArraySchema<number> = new ArraySchema<number>();
}

export class State extends Schema {

    @type({ map: PositionJSON })
    position: MapSchema<PositionJSON> = new MapSchema<PositionJSON>();

    @type({ map: PositionJSON })
    rotation: MapSchema<PositionJSON> = new MapSchema<PositionJSON>();

    @type('string')
    phase: string = "waiting";

    @type('int16')
    playerTurn: number = 1;

    @type('int16')
    winningPlayer: number = -1;

    @type({ map: Player })
    players: MapSchema<Player> = new MapSchema<Player>();

    @type(['int16'])
    player1Shots: ArraySchema<number> = new ArraySchema<number>();

    @type(['int16'])
    player2Shots: ArraySchema<number> = new ArraySchema<number>();
}

