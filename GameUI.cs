using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameUI : MonoBehaviour {

    public GameObject GameLose;
    public GameObject GameWin;
    bool GameIsOver;
    //bool GameIsOver_Lose;
    //bool GameIsOver_Win;

    public GameObject playerObject;
    public player playerScript;

    bool spotted;

    // Use this for initialization
    void Start () {
        Guard.OnSpotted += GameLoseUi;

        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerScript = playerObject.GetComponent<player>();

        FindObjectOfType<player>().ReachedEnd += GameWinUi;

        spotted = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (GameIsOver)
        {
            if (Input.GetKeyDown("x") && spotted) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            if (Input.GetKeyDown("x") && !spotted) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
	}

    void GameWinUi()
    {
        OnGameOver(GameWin);
    }

    public void GameLoseUi()
    {
        OnGameOver(GameLose);
        spotted = true;
    }

    void OnGameOver(GameObject gameOverUI)
    {

        gameOverUI.SetActive(true);
        GameIsOver = true;
        Guard.OnSpotted -= GameLoseUi;
        FindObjectOfType<player>().ReachedEnd -= GameWinUi;

    }
}
