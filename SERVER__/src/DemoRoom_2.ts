import { Room, Client, generateId } from "colyseus";
import { Schema, type, MapSchema, ArraySchema } from "@colyseus/schema";

class Entity extends Schema {
    @type("number")
    x: number = 0;

    @type("number")
    y: number = 0;
}

class Player extends Entity {
    @type("boolean")
    connected: boolean = true;
}

class Enemy extends Entity {
    @type("number")
    power: number = Math.random() * 10;
}

class State extends Schema {
    @type({ map: Entity })
    entities = new MapSchema<Entity>();
}

/**
 * Demonstrate sending schema data types as messages
 */
class Message extends Schema {
    @type("number") num;
    @type("string") str;
}

export class DemoRoom2 extends Room {



    onCreate(options: any) {

        this.setState(new State());


        this.setMetadata({
            str: "hello",
            number: 10
        });


        this.setPatchRate(1000 / 20);
        this.setSimulationInterval((dt) => this.update(dt));


        console.log("DemoRoom2 created!", options);

        this.onMessage("schema", (client) => {
            const message = new Message();
            message.num = Math.floor(Math.random() * 100);
            message.str = "sending to a single client--1111";
            client.send(message);
        })

    }

    onJoin(client: Client, options: any) {
        console.log("client joined!", client.sessionId);
        client.send("testConnectionToMaster", "[message from SERVER] : you joined to room");

    }

    update(dt?: number) {
        // console.log("num clients:", Object.keys(this.clients).length);
    }


}
