using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    private Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource, walkSFXSource;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    private void Start()
    {
        AudioManager.instance.PlayMusic("MainMusic");
    }

    public void PlayMusic(string name)
    {
        Sound music = Array.Find(musicSounds, x => x.name == name);

        if (music == null)
        {
          Debug.Log("Sound Not Found");  
            return;
        }

        musicSource.clip = music.clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
        Debug.Log("Test");
    }

    public void PlaySFX(string name)
    {
        Sound sfx = Array.Find(sfxSounds, x => x.name == name);

        if (sfx == null)
        {
          Debug.Log("Sound Not Found");  
            return;
        }

        sfxSource.PlayOneShot(sfx.clip);
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void UpdateMusicVolume(float _volume)
    {
        musicSource.volume = _volume;
    }

    public void UpdateSFXVolume(float _volume)
    {
        sfxSource.volume = _volume;
        walkSFXSource.volume = _volume;
    }

    public void PlayWalkSFX()
    {
        walkSFXSource.enabled = true;
        Debug.Log("Play");
    }

    public void StopWalkSFX()
    {
        walkSFXSource.enabled = false;
        Debug.Log("Stop");
    }
}
