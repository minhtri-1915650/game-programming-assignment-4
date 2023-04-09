using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    GameObject reference = null;

    int matrixX;
    int matrixY;

    public bool attack = false;

    public void Start()
    {
        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Chessman cm = reference.GetComponent<Chessman>();
        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(
                (matrixX+cm.GetXBoard())/2, 
                (matrixY+cm.GetYBoard())/2);

            // if (cp.name == "white_king") controller.GetComponent<Game>().Winner("black");
            // if (cp.name == "black_king") controller.GetComponent<Game>().Winner("red");

            Destroy(cp);
        }
        else controller.GetComponent<Game>().NextTurn();

        controller.GetComponent<Game>().SetPositionEmpty(cm.GetXBoard(), 
            cm.GetYBoard());

        cm.SetXBoard(matrixX);
        cm.SetYBoard(matrixY);
        cm.SetCoords();

        // Debug.Log(cm.GetYBoard() + " " + cm.GetPlayer());

        if (cm.GetYBoard()==7 && cm.GetPlayer() == "red") {
            cm.name = "red_king_chess";
            cm.Activate();
        }
        else if (cm.GetYBoard()==0 && cm.GetPlayer() == "black") {
            cm.name = "black_king_chess";
            cm.Activate();
        }

        controller.GetComponent<Game>().SetPosition(reference);

        cm.DestroyMovePlates();
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