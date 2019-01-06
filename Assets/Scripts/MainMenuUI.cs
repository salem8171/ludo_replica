using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

    public ScreenFader fader;
    public GameObject mainMenu;
    public GameObject playMenu;

    public void Play()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
    }

    public void Exit()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }

    public void Play2P()
    {
        GameManager.NumberOfPlayers = 2;
        fader.FadeTo("MainScene");
    }

    public void Play3P()
    {
        GameManager.NumberOfPlayers = 3;
        fader.FadeTo("MainScene");
    }

    public void Play4P()
    {
        GameManager.NumberOfPlayers = 4;
        fader.FadeTo("MainScene");
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
    }

}
