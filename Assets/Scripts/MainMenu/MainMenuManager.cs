using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject settingsPanelGO;

    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;

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

    public void AdjustMusicVolume()
    {
        AudioManager.instance.UpdateMusicVolume(musicSlider.value);
    }

    public void AdjustSFXVolume()
    {
        AudioManager.instance.UpdateSFXVolume(sfxSlider.value);
    }
}
