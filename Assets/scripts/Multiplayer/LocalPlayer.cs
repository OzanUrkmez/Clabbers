/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//This script is responsible for managing player info such as how many games the player has played, how many he has won etc.
//It has nothing to do with the current session and can be viewed more as Local Info.

public class LocalPlayer : MonoBehaviour {

    static PlayerProfileInfo CurrentPlayerInfo;

    //INITIAL BEHAVIOUR
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(ProfileExists())
        CurrentPlayerInfo = LoadPlayer();
    }

    //We must save the info when the application quits
    private void OnApplicationQuit()
    {
        SavePlayer();
    }

    //In case the OnApplicationQuit doesn't get called and the program gets terminated unnaturally, we should still call SavePlayer
    private void OnDestroy()
    {
        SavePlayer();
    }


    //SAVING AND LOADING
    private static void SavePlayer()
    {
        if (ProfileExists())
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fn = new FileStream(Application.persistentDataPath + "/PlayerInfo.lpsi", FileMode.Create);

            bf.Serialize(fn, CurrentPlayerInfo);

            fn.Close();
        }
    }

    private static PlayerProfileInfo LoadPlayer()
    {
        if (File.Exists(Application.persistentDataPath + "/PlayerInfo.lpsi"))
        {
           
        }
        return null;
    }

    //To check whether if the player has created his profile yet.
    public static bool ProfileExists()
    {
        if(PlayerPrefs.GetInt("PlayerProfileCreated",0) == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Registering The Player
    public static void RegisterPlayer(string s)
    {
        PlayerPrefs.SetInt("PlayerProfileCreated", 1);
        //TODO Complete this. Dont forget to set the current variable as well as the binary data.
    }

    //The Player Info class that will get saved and loaded.
    [Serializable]
    class PlayerProfileInfo
    {
        string PlayerName { get; set; }
        int GamesPlayed { get; set; }
        float AverageGameTime { get; set; }
        bool PlayedBefore { get; set; }
        int GamesWon { get; set; }
        int GamesLost { get; set; }
        int MiddleCaptured { get; set; }
        long RoyalDrawn { get; set; }
        long CardsPlayed { get; set; }

        PlayerProfileInfo(string Name)
        {
            PlayerName = Name;
            GamesLost = 0;
            GamesPlayed = 0;
            AverageGameTime = 0;
            PlayedBefore = false;
            GamesWon = 0;
            MiddleCaptured = 0;
            RoyalDrawn = 0;
            CardsPlayed = 0;
        }
    }
}*/


