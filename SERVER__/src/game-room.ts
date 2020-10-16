import { Client, Room } from "colyseus";

// LV : code file state.ts
import { State, Player } from './state';



export class GameRoom extends Room<State> {

    maxClients = 4;
    playerCount: number = 0;

    onCreate (options: any) {

        this.setState(new State());

        this.setMetadata({
            str: "hello",
            number: 10
        });

        this.setPatchRate(1000 / 20);
        this.setSimulationInterval((dt) => this.update(dt));

        this.onMessage("from_cl_test_get_playerInfo", (client) => {
            const p = this.state.players[client.sessionId];
            client.send(p);
        })

        this.onMessage("cl_move_right", (client) => {
            const p = this.state.players[client.sessionId];
            this.broadcast("sv_move_right", p );

            //client.send("sv_move_right", p)

        })

        this.onMessage("cl_check_before_join", (client) => {

            console.log(`cl_check_before_join`);
            let ss="NONE";
            if (this.clients.length > 0) {
                ss = "";
                for (var cl of this.clients) {
                    ss = ss  + cl.sessionId + "__";
                  }
            }
            client.send("sv_check_before_join", ss)
       
        })


        console.log("GameRoom created!", options);

        this.resetValue();
    }



    onJoin (client:Client) {

      

        console.log("client joined room :[GameRoom], has sessionId: ", client.sessionId);

        let player: Player = new Player();
        player.sessionId = client.sessionId;
        player.seat = this.playerCount + 1;

        this.state.players[client.sessionId] = player;
        this.playerCount++;
        console.log(this.state.players);
        console.log(`START TEST NET 1:`);
        //client.send("svTestNet", `[message from SERVER] : you joined to room, client has ID :${client.id}, sessionId:${client.sessionId}`);

        // client.send("svTestNet", this.state.players[client.sessionId]);
        this.broadcast("svTestNet", this.state.players[client.sessionId]);
        // client.send("testConnectionToMaster", "[message from SERVER] : you joined to room");
    }




    async onLeave (client:Client) {

        console.log("client onLeave", client.sessionId);

        let ss="NONE";
        if (this.clients.length > 0) {
            ss = "";
            for (var cl of this.clients) {
                ss = ss  + cl.sessionId + "__";
              }
        }

        console.log("con` lai trong room :" + ss);

        // const newClient = await this.allowReconnection(client, 10);
        // console.log("reconnected!", newClient.sessionId);

        delete this.state.players[client.sessionId];
        this.playerCount--;
        this.state.phase = 'waiting';
    }


    // onMessage (client, message) {
    //     console.log("message onMessage", message);

    //     if (!message) return;

    //     let player: Player = this.state.players[client.sessionId];

    //     if (!player) return;

    //     let command: string = message['command'];

    //     switch(command)
    //     {
    //         case"1":
    //             break;


    //         case"2":
            
    //             break;
    //         default:
    //                 console.log('unknown command');
    //     }
        

        
    // }

    onDispose () {
        console.log("onDispose() room destroyed!");
    }

    resetValue()
    {
        this.setState(new State());
    }

    update(dt?: number) {
        // console.log("num clients:", Object.keys(this.clients).length);
    }

}