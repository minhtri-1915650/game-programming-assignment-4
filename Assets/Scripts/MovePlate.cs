using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Net.Mime;

public class MovementInfo
{
    public int[,] state = new int[8, 8];
    public int player;
    public int num_searches;
    
    public MovementInfo(int[,] stateBoard)
    {
        state = stateBoard;
        player = -1;
        num_searches = 100;
    }
}

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    GameObject reference = null;

    int matrixX;
    int matrixY;

    public string color;

    public void Start()
    {
        switch (color)
        {
            case "red":
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.7f);
                break;
            case "yellow":
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 0.7f);
                break;
            case "green":
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.7f);
                break;
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Chessman cm = reference.GetComponent<Chessman>();
        // cm.DestroyMovePlates();
        if (color == "yellow")
        {
            // controller.GetComponent<Game>().DrawMovingPlate();
            // cm.InitiateMovePlates();
            return;
        }
        if (color == "red")
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(
                (matrixX + cm.GetXBoard()) / 2,
                (matrixY + cm.GetYBoard()) / 2);

            // if (cp.name == "white_king") controller.GetComponent<Game>().Winner("black");
            // if (cp.name == "black_king") controller.GetComponent<Game>().Winner("red");
            controller.GetComponent<Game>().SetPositionEmpty(
                (matrixX + cm.GetXBoard()) / 2,
                (matrixY + cm.GetYBoard()) / 2);
            Destroy(cp);
            if (cm.getPlayer() == "red") controller.GetComponent<Game>().decreaseBlack();
            else controller.GetComponent<Game>().decreaseRed();
        }

        if (!controller.GetComponent<Game>().PlayVsAI()) {
            GameObject network = controller.GetComponent<Game>().network;

            network.GetComponent<SocketIO>().EmitMove(cm.GetXBoard(), cm.GetYBoard(), matrixX, matrixY, cm.getPlayer());
        }
        
        controller.GetComponent<Game>().SetPositionEmpty(
            cm.GetXBoard(),
            cm.GetYBoard());

        cm.SetXBoard(matrixX);
        cm.SetYBoard(matrixY);
        cm.SetCoords();

        // Debug.Log(cm.GetYBoard() + " " + cm.GetPlayer());

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
        controller.GetComponent<Game>().SetPosition(reference);

        if (color != "red" || cm.RecursionPlay() == 1)
        {   
            if (controller.GetComponent<Game>().PlayVsAI())
            {
                if (color == "red")
                {
                    MovementInfo mv = new MovementInfo(controller.GetComponent<Game>().GetStateBoard());
                    HttpClient _httpClient = new HttpClient();
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri("http://localhost:5000/mcts-move"),
                        Content = new StringContent(JsonConvert.SerializeObject(mv), Encoding.UTF8, MediaTypeNames.Application.Json)
                    };
                    var response = _httpClient.SendAsync(request);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        string apiResponse = response.Result.Content.ReadAsStringAsync().Result;
                        int[] movement = JsonConvert.DeserializeObject<int[]>(apiResponse);
                        int x1 = movement[0];
                        int y1 = movement[1];
                        int x2 = movement[2];
                        int y2 = movement[3];
                        if (Math.Abs(x2 - x1) == 2 && Math.Abs(y2 - y1) == 2)
                        {
                            controller.GetComponent<Game>().SetPositionEmpty(x1 + 1, y1 + 1);
                        }
                        controller.GetComponent<Game>().SetPositionEmpty(x1, y1);
                        cm.SetXBoard(x2);
                        cm.SetYBoard(y2);
                        cm.SetCoords();    
                    }
                    else
                    {   
                        Console.WriteLine(response.Result.StatusCode);
                    }                    
                }
            }
            else
            {
                controller.GetComponent<Game>().NextTurn();
                cm.DestroyMovePlates();
                // controller.GetComponent<Game>().DrawMovingPlate();
            }
        }
        controller.GetComponent<Game>().CheckWinner();
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }
}