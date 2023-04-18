using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }
    public GameObject chooseState;
    public GameObject pausePanel;

    public void openChooseState() {
        chooseState.SetActive(true);
    }

    public void chooseStateVSMachine() {
        AIMode.aimode = true;
        changeScence();
    }

    public void chooseStateVSPlayer() {
        AIMode.aimode = false;
        changeScence();
    }

    private void changeScence() {
        SceneManager.LoadScene("Game"); 
    }

    public void pauseGame() {
        pausePanel.SetActive(true);
    }

    public void resumeGame() {
        pausePanel.SetActive(false);

    }
}
