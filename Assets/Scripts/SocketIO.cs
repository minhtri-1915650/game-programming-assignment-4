using System;
using System.Collections.Generic;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

// using Debug = System.Diagnostics.Debug;
public class SocketIO : MonoBehaviour
{
    public SocketIOUnity socket;
    public Game controller;
    public string name;


    // Start is called before the first frame update
    void Start()
    {

        //TODO: check the Uri if Valid.
        var uri = new Uri("http://localhost:11100");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "UNITY" }
                }
            ,
            EIO = 4
            ,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        ///// reserved socketio events
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("socket.OnConnected");
        };
        socket.OnPing += (sender, e) =>
        {
            Debug.Log("Ping");
        };
        socket.OnPong += (sender, e) =>
        {
            Debug.Log("Pong: " + e.TotalMilliseconds);
        };
        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("disconnect: " + e);
        };
        socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Log($"{DateTime.Now} Reconnecting: attempt = {e}");
        };
        ////

        Debug.Log("Connecting...");
        socket.Connect();

        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();


        this.On();
    }

    public void NextTurn(string player)
    {
        socket.Emit("next-turn", player);
    }

    public void EmitMove(int x, int y, int endX, int endY, string currentPlayer)
    {
        PositionOnBoard p = new PositionOnBoard(x, y, endX, endY, currentPlayer);
        socket.Emit("move", p);
    }

    public void On()
    {
        socket.OnUnityThread("set-name", (res) =>
        {
            string name = res.GetValue<string>();
            this.name = name;
            if (name == "red")
            {
                controller.DrawMovingPlate();
            }
            Debug.Log("name: " + name);
        });

        socket.OnUnityThread("next-turn", (res) =>
        {
            string player = res.GetValue<string>();

            controller.SetCurrentPlayer(player);
            Debug.Log("turn: " + player);
        });

        socket.OnUnityThread("move", (res) =>
        {

            PositionOnBoard data = res.GetValue<PositionOnBoard>();

            if (data.currentPlayer == this.name)
            {
                return;
            }

            Debug.Log("move " + data.currentPlayer);

            GameObject cp = controller.GetPosition(
                data.x, data.y);

            if (cp != null)
            {
                Chessman cm = cp.GetComponent<Chessman>();

                controller.SetPositionEmpty(
                data.x, data.y);

                cm.SetXBoard(data.endX);
                cm.SetYBoard(data.endY);
                cm.SetCoords();

                if (cm.GetYBoard() == 7 && cm.GetPlayer() == "red")
                {
                    cm.name = "red_king_chess";
                    cm.Activate();
                }
                else if (cm.GetYBoard() == 0 && cm.GetPlayer() == "black")
                {
                    cm.name = "black_king_chess";
                    cm.Activate();
                }

                controller.SetPosition(cp);
            }

        });
    }

    [System.Serializable]
    public class PositionOnBoard
    {
        public int x, endX;
        public int y, endY;

        public string currentPlayer;

        public PositionOnBoard(int x, int y, int endX, int endY, string currentPlayer)
        {
            this.x = x;
            this.y = y;
            this.endX = endX;
            this.endY = endY;
            this.currentPlayer = currentPlayer;
        }
    }
}
