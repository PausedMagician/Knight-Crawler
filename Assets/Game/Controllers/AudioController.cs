using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    #region Singleton
    private static AudioController instance;

    private void Awake() {
        if(instance != null) {
            Debug.LogWarning("More than one instance of AudioController found!");
            return;
        }
        instance = this;
    }

    public static AudioController GetInstance() {
        return instance;
    }
    #endregion

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public float masterVolume = 1f;

    public void SetMusicVolume(float volume) {
        musicVolume = volume;
        musicSource.volume = musicVolume * masterVolume;
    }

    public void SetSFXVolume(float volume) {
        sfxVolume = volume;
        sfxSource.volume = sfxVolume * masterVolume;
    }

    public void SetMasterVolume(float volume) {
        masterVolume = volume;
        musicSource.volume = musicVolume * masterVolume;
        sfxSource.volume = sfxVolume * masterVolume;
    }

    public void PlaySFX(AudioClip clip) {
        sfxSource.PlayOneShot(clip);
    }
    public void StopSFX() {
        sfxSource.Stop();
    }
    public void PauseSFX() {
        sfxSource.Pause();
    }
    public void UnpauseSFX() {
        sfxSource.UnPause();
    }

    public void PlayMusic(AudioClip clip) {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void StopMusic() {
        musicSource.Stop();
    }
    public void PauseMusic() {
        musicSource.Pause();
    }
    public void UnpauseMusic() {
        musicSource.UnPause();
    }

}
