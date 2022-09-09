using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameWinUI;
    public GameObject gameLoseUI;
    bool gameOver;
    // Start is called before the first frame update
    void Start()
    {
        Guard.onGuardHasSpottedPlayer += showGameLoseUI;
        Player.reachedFinish += showGameWinUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                SceneManager.LoadScene(0);
            }
        }
    }

    void showGameWinUI() {
        gameWinUI.SetActive(true);
        gameOver = true;
        Guard.onGuardHasSpottedPlayer -= showGameLoseUI;
        Player.reachedFinish -= showGameWinUI;
    }

    void showGameLoseUI() {
        gameLoseUI.SetActive(true);
        gameOver = true;
        Guard.onGuardHasSpottedPlayer -= showGameLoseUI;
        Player.reachedFinish -= showGameWinUI;
    }
}
