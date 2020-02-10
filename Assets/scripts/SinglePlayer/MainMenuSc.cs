using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuSc : MonoBehaviour {

    public string sceneOfflineName;
    public float[] pagesX;
    public int WeAreOnPage = 3;
    public GameObject camenra;
    public GameObject MainLobby;
    public void OfflineChange(int input)
    {
        offlineGameController.peopleCount = input;
        SceneManager.LoadScene(sceneOfflineName);

    }

    void Update() {

    }

    public void MultiplayerSlide()
    {
        StartCoroutine(moveLobby(20));
    }

    IEnumerator moveLobby(int amount)
    {
        yield return new WaitForSeconds(0.01f);
        if (amount > 0)
        {
            MainLobby.transform.Translate(0,-0.5f,0);
            StartCoroutine(moveLobby(amount-1));
        }
    }

    IEnumerator moveCamera(float moveAmount, int count)
    {
        yield return new WaitForSeconds(0.01f);

        if (count > 0)
        {
            camenra.transform.Translate(moveAmount / 50, 0, 0);
            StartCoroutine(moveCamera(moveAmount, count - 1));
        }
    }
    public void menuButtons(int amount) {
        Debug.LogError("eeee " + amount);
        StartCoroutine(moveCamera((float)amount, 50));
    }
}
