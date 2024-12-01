using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public void GoToMenu() {
        StartCoroutine(TransitionPanel.instance.StartTransitionPanelAnimation("MainMenu"));
    }

    public void RestartGame() {
        StartCoroutine(TransitionPanel.instance.StartTransitionPanelAnimation("GameScene"));
    }

    public void ClickSound() {
        Time.timeScale = 1;
        AudioManager.instance.PlaySFX("ButtonClick");
    }
}
