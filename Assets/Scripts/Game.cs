using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

public class Game : MonoBehaviour
{
    public GameObject chesspiece;
    public GameObject network;

    public GameObject button;

    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[12];
    private GameObject[] playerRed = new GameObject[12];
    private int countBlack;
    private int countRed;

    private string currentPlayer = "red";
    private bool gameOver = false;
    private bool nextTurn = false;

    public void Start()
    {
        if (!AIMode.aimode)
        {
            button.SetActive(false);
            this.network = GameObject.FindGameObjectWithTag("Network");
        }

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

    public int[,] GetStateBoard()
    {
        int[,] state = new int[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (positions[i, j] == null)
                {
                    state[j, i] = 0;
                }
                else
                {
                    GameObject obj = positions[i, j];
                    switch (obj.name)
                    {
                        case "black_chess":
                            state[j, i] = -1;
                            break;
                        case "black_king_chess":
                            state[j, i] = -2;
                            break;
                        case "red_chess":
                            state[j, i] = 1;
                            break;
                        case "red_king_chess":
                            state[j, i] = 2;
                            break;
                    }
                }
            }
        }
        return state;
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
        nextTurn = true;
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
        if (currentPlayer == "black" && AIMode.aimode)
        {
            MovementInfo mv = new MovementInfo(GetStateBoard());
            HttpClient _httpClient = new HttpClient();
            HttpRequestMessage request;
            if (AIMode.easyLevel)
            {
                request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri("http://localhost:5000/checker-agent/random-move"),
                    Content = new StringContent(JsonConvert.SerializeObject(mv), Encoding.UTF8, MediaTypeNames.Application.Json)
                };
            }
            else
            {
                request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri("http://localhost:5000/checker-agent/mcts-move"),
                    Content = new StringContent(JsonConvert.SerializeObject(mv), Encoding.UTF8, MediaTypeNames.Application.Json)
                };

            }
            var response = _httpClient.SendAsync(request);
            if (response.Result.IsSuccessStatusCode)
            {
                string apiResponse = response.Result.Content.ReadAsStringAsync().Result;
                int[] movement = JsonConvert.DeserializeObject<int[]>(apiResponse);
                int y1 = movement[0];
                int x1 = movement[1];
                int y2 = movement[2];
                int x2 = movement[3];
                GameObject obj = GetPosition(x1, y1);
                Chessman cm_AI = obj.GetComponent<Chessman>();
                cm_AI.SetXBoard(x2);
                cm_AI.SetYBoard(y2);
                if (y2 == 0)
                {
                    cm_AI.name = "black_king_chess";
                    cm_AI.Activate();
                } else
                {
                    cm_AI.SetCoords();
                }                
                SetPosition(obj);
                SetPositionEmpty(x1, y1);
                if (Math.Abs(x2 - x1) == 2 && Math.Abs(y2 - y1) == 2)
                {
                    GameObject obj2 = GetPosition((x1 + x2) / 2, (y1 + y2) / 2);
                    SetPositionEmpty((x1 + x2) / 2, (y1 + y2) / 2);
                    Destroy(obj2);
                    decreaseRed();
                    CheckEatMore(x2, y2);
                }
                NextTurn();
                CheckWinner();
            }
            else
            {
                Debug.Log(mv.state.ToString());
                Debug.Log("Error when calling API: " + response.Result.StatusCode);
                Debug.Log("Try to calling...");
            }
        }
        if (nextTurn)
        {
            if (currentPlayer == "red")
            {
                currentPlayer = "black";
            }
            else
            {
                currentPlayer = "red";
            }
            if (!AIMode.aimode)
            {
                network.GetComponent<SocketIO>().NextTurn(currentPlayer);
            }
            nextTurn = false;
        }
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene("Begin");
        }
    }

    private void CheckEatMore(int x2, int y2)
    {
        if (PositionOnBoard(x2 + 2, y2 + 2) && GetPosition(x2 + 2, y2 + 2) == null)
        {
            CheckEatMorePosition(x2, y2, x2 + 2, y2 + 2);
        }
        else if (PositionOnBoard(x2 + 2, y2 - 2) && GetPosition(x2 + 2, y2 - 2) == null)
        {
            CheckEatMorePosition(x2, y2, x2 + 2, y2 - 2);
        }
        else if (PositionOnBoard(x2 - 2, y2 - 2) && GetPosition(x2 - 2, y2 - 2) == null)
        {
            CheckEatMorePosition(x2, y2, x2 - 2, y2 - 2);
        }
        else if (PositionOnBoard(x2 - 2, y2 + 2) && GetPosition(x2 - 2, y2 + 2) == null)
        {
            CheckEatMorePosition(x2, y2, x2 - 2, y2 + 2);
        }
    }

    private void CheckEatMorePosition(int x1, int y1, int x2, int y2)
    {
        GameObject obj = GetPosition((x1 + x2) / 2, (y1 + y2) / 2);
        Chessman cm = obj.GetComponent<Chessman>();
        if (cm.getPlayer() == "red")
        {
            GameObject obj2 = GetPosition(x1, y1);
            Chessman cm_AI = obj2.GetComponent<Chessman>();
            cm_AI.SetXBoard(x2);
            cm_AI.SetYBoard(y2);
            if (y2 == 0)
            {
                cm_AI.name = "black_king_chess";
                cm_AI.Activate();
            }
            else
            {
                cm_AI.SetCoords();
            }
            SetPosition(obj2);
            SetPositionEmpty(x2, y2);
            SetPositionEmpty((x1 + x2) / 2, (y1 + y2) / 2);
            Destroy(obj);
            decreaseRed();
            CheckEatMore(x2, y2);
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