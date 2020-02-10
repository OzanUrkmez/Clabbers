using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public static void LoadLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public static void LoadMultiplayerGame()
    {
        SceneManager.LoadScene("onlineGame");
    }

    public static void LoadTitleMenu()
    {
        SceneManager.LoadScene("game");
    }

    public static void LoadSinglePlayer()
    {
        SceneManager.LoadScene("offline");
    }

    public void loadLobby()
    {
        LoadLobby();
    }

    public void loadMultiplayerGame()
    {
        LoadMultiplayerGame();
    }

    public void loadTitleMenu()
    {
        LoadTitleMenu();
    }
    
    public void loadSinglePlayer()
    {
        LoadSinglePlayer();
    }
}
