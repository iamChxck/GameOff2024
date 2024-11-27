using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.PlayMusic("MainMusic");
    }

    public void StartGame()
    {
        StartCoroutine(TransitionPanel.instance.StartTransitionPanelAnimation("GameScene"));
    }
}
