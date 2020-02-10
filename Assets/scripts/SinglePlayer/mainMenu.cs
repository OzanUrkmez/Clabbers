using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class mainMenu : MonoBehaviour {

    public string[] nameOfScene;
    public void OnSceneButton(int whichName)
    {
        SceneManager.LoadScene(nameOfScene[whichName]);
    }

    public static string nameOfPlayer;
    public Text nameField;
    public void ChanceName() {
        nameOfPlayer = nameField.text;
    }


    public GameObject tutorialObj;

    void Start () {
        if (PlayerPrefs.HasKey("Enter"))
        {

        }
        else
        {
            tutorialStart();
        }
	}
    public void tutorialStart() {
        tutorialObj.SetActive(true);
    } 
}
