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
                break;
            case "black_king_chess":
                this.GetComponent<SpriteRenderer>().sprite = black_king_chess;
                player = "black";
                break;
            case "red_chess":
                this.GetComponent<SpriteRenderer>().sprite = red_chess;
                player = "red";
                break;
            case "red_king_chess":
                this.GetComponent<SpriteRenderer>().sprite = red_king_chess;
                player = "red";
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

    private void OnMouseUp()
    {
        if (!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().GetCurrentPlayer() == player)
        {
            DestroyMovePlates();
            InitiateMovePlates();
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
        switch (this.name)
        {
            case "red_chess":
                PointMovePlate(1, 1);
                PointMovePlate(-1, 1);
                break;
            case "black_chess":
                PointMovePlate(1, -1);
                PointMovePlate(-1, -1);
                break;
            case "red_king_chess":
            case "black_king_chess":
                PointMovePlate(1, 1);
                PointMovePlate(-1, 1);
                PointMovePlate(1, -1);
                PointMovePlate(-1, -1);
                break;
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
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                if (sc.PositionOnBoard(x + xIncrement, y + yIncrement) &&
                    sc.GetPosition(x + xIncrement, y + yIncrement) == null)
                    MovePlateSpawn(x + xIncrement, y + yIncrement, true);
            }
        }
    }

    public void MovePlateSpawn(int matrixX, int matrixY, bool attack = false)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 0.6f;
        y *= 0.6f;

        x += -2.1f;
        y += -2.1f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -1.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = attack;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public string GetPlayer()
    {
        return player;
    }
}