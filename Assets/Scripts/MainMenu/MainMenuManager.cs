using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject settingsPanelGO;

    private void Start()
    {
        AudioManager.instance.PlayMusic("MainMusic");
    }

    public void StartGame()
    {
        StartCoroutine(TransitionPanel.instance.StartTransitionPanelAnimation("GameScene"));
    }

    public void ToggleSettings()
    {
        settingsPanelGO.SetActive(!settingsPanelGO.activeSelf);
    }
}
