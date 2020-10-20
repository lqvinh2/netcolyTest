import { Client, Room } from "colyseus";

// LV : code file state.ts
import { State, Player } from './state';


// global variables for the server
var enemies = [];
var playerSpawnPoints = [];
var clientsServer = [];

var SERVER_NEED_SETUP_INFO_PLAYER = 1;
var SERVER_FINISH_SETUP_INFO = 2;

console.log(`11111111111`)
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




        this.onMessage(SERVER_NEED_SETUP_INFO_PLAYER, (client, dataPlayer) => {
            clientsServer[dataPlayer.sessionId] = dataPlayer;

            client.send(SERVER_FINISH_SETUP_INFO, 1);

        });

  




        this.onMessage("from_cl_test_get_playerInfo", (client) => {
            const p = this.state.players[client.sessionId];
            client.send(p);
        });

        this.onMessage("cl_move_right", (client) => {
            const p = this.state.players[client.sessionId];
            this.broadcast("sv_move_right", p );

           

        });

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
       
        });

        this.onMessage("player move", (client, pos) => {

            this.broadcast("player move", pos );
        });






        console.log("GameRoom created!", options);


     

        this.resetValue();
    }


 


    onJoin (client:Client) {

      

        console.log("client joined room :[GameRoom], has sessionId: ", client.sessionId);

        // let player: Player = new Player();
        // player.sessionId = client.sessionId;
        // player.seat = this.playerCount + 1;

        // this.state.players[client.sessionId] = player;
        // this.playerCount++;
        // console.log(this.state.players);
        // console.log(`START TEST NET 1:`);
        //client.send("svTestNet", `[message from SERVER] : you joined to room, client has ID :${client.id}, sessionId:${client.sessionId}`);

        // client.send("svTestNet", this.state.players[client.sessionId]);
        // this.broadcast("svTestNet", this.state.players[client.sessionId]);
        client.send(SERVER_NEED_SETUP_INFO_PLAYER, 11111);
    }




    async onLeave (client:Client) {

        console.log("client onLeave", client.sessionId);

        let ss="NONE";
        if (clientsServer.length > 0) {
            ss = "";
            for (var cl of clientsServer) {
                ss = ss  + cl.sessionId + "__";
              }
        }

        console.log("con` lai trong room :" + ss);

        delete clientsServer[client.sessionId];

        for (var cl of clientsServer) {
            ss = ss  + cl.sessionId + "__";
          }

        this.playerCount--;
        this.state.phase = 'waiting';
    }


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

    guid() {
        function s4() {
            return Math.floor((1+Math.random()) * 0x10000).toString(16).substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
    }


}