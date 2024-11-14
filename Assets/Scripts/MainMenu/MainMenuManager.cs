using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(StartTransitionPanelAnimation());
    }

    IEnumerator StartTransitionPanelAnimation() {
        if (TransitionPanel.instance != null) {
            TransitionPanel.instance.FadeOutTransition();
        }

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("GrayscaleTestScene");
    }
}
