using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    public GameObject controller;
    public GameObject movePlate;

    private int xBoard = -1;
    private int yBoard = -1;

    private string player;

    private (int x, int y)[] movePoints;

    public Sprite red_chess, red_king_chess;
    public Sprite black_chess, black_king_chess;

    public void Activate()
    {

        controller = GameObject.FindGameObjectWithTag("GameController");
        SetCoords();

        switch (this.name)
        {
            case "black_chess":
                this.GetComponent<SpriteRenderer>().sprite = black_chess;
                player = "black";
                this.movePoints = new[] { (1, -1), (-1, -1) };
                break;
            case "black_king_chess":
                this.GetComponent<SpriteRenderer>().sprite = black_king_chess;
                player = "black";
                this.movePoints = new[] { (1, 1), (-1, 1), (1, -1), (-1, -1) };
                break;
            case "red_chess":
                this.GetComponent<SpriteRenderer>().sprite = red_chess;
                player = "red";
                this.movePoints = new[] { (1, 1), (-1, 1) };
                break;
            case "red_king_chess":
                this.GetComponent<SpriteRenderer>().sprite = red_king_chess;
                player = "red";
                this.movePoints = new[] { (1, 1), (-1, 1), (1, -1), (-1, -1) };
                break;
        }
    }

    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;

        x *= 0.6f;
        y *= 0.6f;

        x += -2.1f;
        y += -2.1f;

        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    public string getPlayer()
    {
        return player;
    }

    public (int x, int y)[] getMovePoints()
    {
        return movePoints;
    }

    private void OnMouseUp()
    {
        string playerName = "red";
        if (!AIMode.aimode)
        {
            GameObject network = controller.GetComponent<Game>().network;
            playerName = network.GetComponent<SocketIO>().name;
        }

        if (!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().GetCurrentPlayer() == player && controller.GetComponent<Game>().GetCurrentPlayer() == playerName)
        {
            DestroyMovePlates();
            InitiateMovePlates();
            // for (int i = 0 ; i < this.movePoints.Length; i++) {
            //     Debug.Log(this.movePoints[i].x + " " + this.movePoints[i].y);
            // }
        }
    }

    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    public void InitiateMovePlates()
    {
        controller.GetComponent<Game>().DrawMovingPlate();
        MovePlateSpawn(xBoard, yBoard, "yellow");
        for (int i = 0; i < this.movePoints.Length; i++)
        {
            PointMovePlate(movePoints[i].x, movePoints[i].y);
        }
    }

    public void PointMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y, "green");
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                if (sc.PositionOnBoard(x + xIncrement, y + yIncrement) &&
                    sc.GetPosition(x + xIncrement, y + yIncrement) == null)
                    MovePlateSpawn(x + xIncrement, y + yIncrement, "red");
            }
        }
    }

    public void DrawWaitingPlate()
    {
        Game sc = controller.GetComponent<Game>();
        bool isDraw = false;
        foreach (var move in getMovePoints())
        {
            int x = GetXBoard() + move.x;
            int y = GetYBoard() + move.y;
            if (sc.PositionOnBoard(x, y))
            {
                GameObject cp = sc.GetPosition(x, y);

                if (cp == null)
                {
                    isDraw = true;
                }
                else if (cp.GetComponent<Chessman>().player != player)
                {
                    if (sc.PositionOnBoard(x + move.x, y + move.y) &&
                        sc.GetPosition(x + move.x, y + move.y) == null)
                        isDraw = true;
                }
            }
        }
        if (isDraw)
        {
            MovePlateSpawn(GetXBoard(), GetYBoard(), "yellow");
        }
    }

    public int RecursionPlay()
    {
        Game sc = controller.GetComponent<Game>();
        bool isMove = false;
        List<(int x, int y)> moveSteps = new List<(int x, int y)>();
        int xdis = 0, ydis = 0;
        foreach (var move in getMovePoints())
        {
            int x = GetXBoard() + move.x;
            int y = GetYBoard() + move.y;
            if (sc.PositionOnBoard(x, y))
            {
                GameObject cp = sc.GetPosition(x, y);
                if (cp != null && cp.GetComponent<Chessman>().player != player)
                {
                    if (sc.PositionOnBoard(x + move.x, y + move.y) &&
                        sc.GetPosition(x + move.x, y + move.y) == null)
                    {
                        isMove = true;
                        xdis = x + move.x;
                        ydis = y + move.y;
                        moveSteps.Add((x + move.x, y + move.y));
                    }
                }
            }
        }
        if (isMove)
        {
            if (moveSteps.Count > 1)
            {
                DestroyMovePlates();
                foreach (var move in moveSteps)
                {
                    MovePlateSpawn(move.x, move.y, "red");
                    // controller.GetComponent<Game>().NextTurn();
                }
                return 0;
            }
            GameObject cp = controller.GetComponent<Game>().GetPosition(
                (xdis + GetXBoard()) / 2,
                (ydis + GetYBoard()) / 2);

            // if (cp.name == "white_king") controller.GetComponent<Game>().Winner("black");
            // if (cp.name == "black_king") controller.GetComponent<Game>().Winner("red");
            controller.GetComponent<Game>().SetPositionEmpty(
                (xdis + GetXBoard()) / 2,
                (ydis + GetYBoard()) / 2);
            Destroy(cp);
            if (player == "red") controller.GetComponent<Game>().decreaseBlack();
            else controller.GetComponent<Game>().decreaseRed();

            controller.GetComponent<Game>().SetPositionEmpty(
                GetXBoard(),
                GetYBoard());

            SetXBoard(xdis);
            SetYBoard(ydis);
            SetCoords();

            if (!AIMode.aimode)
            {
                GameObject network = controller.GetComponent<Game>().network;
                network.GetComponent<SocketIO>().EmitMove((xdis + GetXBoard()) / 2,
                    (ydis + GetYBoard()) / 2, xdis, ydis, this.player);
            }

            // Debug.Log(cm.GetYBoard() + " " + cm.GetPlayer());

            if (GetYBoard() == 7 && GetPlayer() == "red")
            {
                this.name = "red_king_chess";
                Activate();
            }
            else if (GetYBoard() == 0 && GetPlayer() == "black")
            {
                this.name = "black_king_chess";
                Activate();
            }
            controller.GetComponent<Game>().SetPosition(gameObject);
            // StartCoroutine(WaitFor(1));
            // Debug.Log("Start");
            return RecursionPlay();
        }
        return 1;
    }

    public void MovePlateSpawn(int matrixX, int matrixY, string color)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 0.6f;
        y *= 0.6f;

        x += -2.1f;
        y += -2.1f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -0.5f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.color = color;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public string GetPlayer()
    {
        return player;
    }
}