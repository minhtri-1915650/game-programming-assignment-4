"use strict";

const http = require("http");
const socket = require("socket.io");
const server = http.createServer();
const port = 11100;

var io = socket(server, {
  pingInterval: 10000,
  pingTimeout: 5000,
});

const users = {};

io.use((socket, next) => {
  if (socket.handshake.query.token === "UNITY") {
    next();
  } else {
    next(new Error("Authentication error"));
  }
});

io.on("connection", (socket) => {
  console.log("connection", socket.id);

  const ids = Object.keys(users);
  if (ids.length <= 0 || users[ids[0]] == "black") {
    users[socket.id] = "red";
  } else {
    users[socket.id] = "black";
  }

  socket.join(1);

  socket.on("move", (data) => {
    console.log(data);
    io.in(1).emit("move", data);
  });

  socket.on("next-turn", (data) => {
    console.log(data);
    io.in(1).emit("next-turn", data);
  });

  socket.emit("set-name", users[socket.id]);

  socket.on("disconnect", () => {
    delete users[socket.id];
  });

  //   socket.on('spin', (data) => {
  //     console.log('spin');
  //     socket.emit('spin', {date: new Date().getTime(), data: data});
  //   });

  //   socket.on('class', (data) => {
  //     console.log('class', data);
  //     socket.emit('class', {date: new Date().getTime(), data: data});
  //   });
});

server.listen(port, () => {
  console.log("listening on *:" + port);
});
