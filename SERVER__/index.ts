import express from 'express';
import serveIndex from 'serve-index';
import path from 'path';
import cors from 'cors';
import { createServer } from 'http';
import { Server, LobbyRoom, RelayRoom } from 'colyseus';

import {GameRoom } from './src/game-room';

import { monitor } from '@colyseus/monitor';


const port = Number(process.env.PORT || 2569) + Number(process.env.NODE_APP_INSTANCE || 0);
const app = express();

app.use(cors());
app.use(express.json());

// Attach WebSocket Server on HTTP Server.
const gameServer = new Server({
  server: createServer(app),
  express: app,
  pingInterval: 0,
});

// // Define "lobby" room
// gameServer.define("lobby", LobbyRoom);

// // Define "relay" room
// gameServer.define("relay", RelayRoom, { maxClients: 4 })
//     .enableRealtimeListing();

let gameRoom1_match_from_client_when_create = "gameVinh";
gameServer.define(gameRoom1_match_from_client_when_create, GameRoom, { maxClients: 4 }).enableRealtimeListing();

app.use('/', serveIndex(path.join(__dirname, "static"), {'icons': true}))
app.use('/', express.static(path.join(__dirname, "static")));

// (optional) attach web monitoring panel
app.use('/colyseus', monitor());

gameServer.onShutdown(function(){
  console.log(`game server is going down.`);
});

gameServer.listen(port);

// process.on("uncaughtException", (e) => {
//   console.log(e.stack);
//   process.exit(1);
// });

console.log(`Listening on [ws://localhost:${port}], 1.copy this link to your code, 2.copy [${gameRoom1_match_from_client_when_create}] to your code client [var roomName]`);
