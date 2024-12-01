using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameAnimation : MonoBehaviour
{
    public GameObject endGameCamera;
    public GameObject endGamePanel;

    public void StartEndScene() {
        StartCoroutine("StartTransition");
    }

    IEnumerator StartTransition() {
        endGameCamera.SetActive(true);

        yield return new WaitForSeconds(35);

        endGamePanel.SetActive(true );
        SceneManager.LoadScene("MainMenu");
    }
}