using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Game : MonoBehaviour
{
    public GameObject chesspiece;
    public GameObject network;

    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[12];
    private GameObject[] playerRed = new GameObject[12];
    private int countBlack;
    private int countRed;

    private string currentPlayer = "red";
    private bool gameOver = false;

    public void Start()
    {
        this.network = GameObject.FindGameObjectWithTag("Network");

        playerRed = new GameObject[] {
            Create("red_chess", 0, 0), Create("red_chess", 2, 0), Create("red_chess", 4, 0), Create("red_chess", 6, 0),
            Create("red_chess", 1, 1), Create("red_chess", 3, 1), Create("red_chess", 5, 1), Create("red_chess", 7, 1),
            Create("red_chess", 0, 2), Create("red_chess", 2, 2), Create("red_chess", 4, 2), Create("red_chess", 6, 2)};
        playerBlack = new GameObject[] {
            Create("black_chess", 1, 5), Create("black_chess", 3, 5), Create("black_chess", 5, 5), Create("black_chess", 7, 5),
            Create("black_chess", 0, 6), Create("black_chess", 2, 6), Create("black_chess", 4, 6), Create("black_chess", 6, 6),
            Create("black_chess", 1, 7), Create("black_chess", 3, 7), Create("black_chess", 5, 7), Create("black_chess", 7, 7)};

        // playerRed = new GameObject[] { 
        //     Create("red_chess", 0, 0)};

        // playerBlack = new GameObject[] { 
        //     Create("black_chess", 1, 1)};
        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
        }
        for (int i = 0; i < playerRed.Length; i++)
        {
            SetPosition(playerRed[i]);
        }
        countBlack = playerBlack.Length;
        countRed = playerRed.Length;

        // this.SetCurrentPlayer("red");
    }

    public void decreaseBlack()
    {
        countBlack -= 1;
    }
    public void decreaseRed()
    {
        countRed -= 1;
    }



    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public void SetCurrentPlayer(string player)
    {
        currentPlayer = player;
        string username = network.GetComponent<SocketIO>().name;
        if (username == player)
        {
            this.DrawMovingPlate();
        }
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        if (currentPlayer == "red")
        {
            currentPlayer = "black";
        }
        else
        {
            currentPlayer = "red";
        }

        network.GetComponent<SocketIO>().NextTurn(currentPlayer);
    }

    public void DrawMovingPlate()
    {
        printPlayer();

        GameObject[] player = (currentPlayer == "red") ? playerRed : playerBlack;
        foreach (var chess in player)
        {
            if (chess != null)
            {
                Chessman cm = chess.GetComponent<Chessman>();
                if (GetPosition(cm.GetXBoard(), cm.GetYBoard()) != null)
                    cm.DrawWaitingPlate();

            }
        }

    }

    public void printPlayer()
    {
        string log = "";
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (positions[j, 7 - i] == null) log = log + " null";
                else log = log + " " + positions[j, 7 - i].GetComponent<Chessman>().getPlayer();
            }
            log = log + "\n";
        }
        Debug.Log(log);
    }

    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene("Game");
        }
    }

    public void CheckWinner()
    {
        if (countBlack == 0 || countRed == 0)
        {
            gameOver = true;

            string winner = (countBlack == 0) ? "Red" : "Black";

            //Using UnityEngine.UI is needed here
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = winner + " is the winner";

            GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;

        }
    }
}