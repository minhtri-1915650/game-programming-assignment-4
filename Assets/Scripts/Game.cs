using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject chesspiece;

    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[12];
    private GameObject[] playerRed = new GameObject[12];

    private string currentPlayer = "red";
    private bool gameOver = false;

    public void Start()
    {
        playerRed = new GameObject[] {
            Create("red_chess", 0, 0), Create("red_chess", 2, 0), Create("red_chess", 4, 0), Create("red_chess", 6, 0),
            Create("red_chess", 1, 1), Create("red_chess", 3, 1), Create("red_chess", 5, 1), Create("red_chess", 7, 1),
            Create("red_chess", 0, 2), Create("red_chess", 2, 2), Create("red_chess", 4, 2), Create("red_chess", 6, 2)};

        playerBlack = new GameObject[] {
            Create("black_chess", 1, 5), Create("black_chess", 3, 5), Create("black_chess", 5, 5), Create("black_chess", 7, 5),
            Create("black_chess", 0, 6), Create("black_chess", 2, 6), Create("black_chess", 4, 6), Create("black_chess", 6, 6),
            Create("black_chess", 1, 7), Create("black_chess", 3, 7), Create("black_chess", 5, 7), Create("black_chess", 7, 7)};

        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerRed[i]);
        }
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
    }

    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene("Game");
        }
    }

    // public void Winner(string playerWinner)
    // {
    //     gameOver = true;

    //     //Using UnityEngine.UI is needed here
    //     GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
    //     GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner";

    //     GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    // }
}