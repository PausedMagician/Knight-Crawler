using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public TextMeshProUGUI musicVolumeText, sfxVolumeText, masterVolumeText;
    public Slider musicVolumeSlider, sfxVolumeSlider, masterVolumeSlider;
    public Toggle fullscreenToggle, vsyncToggle;
    public TMP_Dropdown qualityDropdown;
    public TextMeshProUGUI resolutionText;
    public Resolution[] resolutions;
    public int currentResolutionIndex;

    public void Start() {
        musicVolumeText.text = $"Music: {(int)(AudioController.GetInstance().musicVolume * 100)}%";
        sfxVolumeText.text = $"SFX: {(int)(AudioController.GetInstance().sfxVolume * 100)}%";
        masterVolumeText.text = $"Master: {(int)(AudioController.GetInstance().masterVolume * 100)}%";

        musicVolumeSlider.value = AudioController.GetInstance().musicVolume;
        sfxVolumeSlider.value = AudioController.GetInstance().sfxVolume;
        masterVolumeSlider.value = AudioController.GetInstance().masterVolume;

        qualityDropdown.value = QualitySettings.GetQualityLevel();

        fullscreenToggle.isOn = Screen.fullScreen;

        if(QualitySettings.vSyncCount == 0) {
            vsyncToggle.isOn = false;
        } else {
            vsyncToggle.isOn = true;
        }

        resolutionText.text = $"{Screen.width}x{Screen.height}";
    }

    public void SetMusicVolume(float volume) {
        AudioController.GetInstance().SetMusicVolume(volume);
        musicVolumeText.text = $"Music: {(int)(volume * 100)}%";
    }

    public void SetSFXVolume(float volume) {
        AudioController.GetInstance().SetSFXVolume(volume);
        sfxVolumeText.text = $"SFX: {(int)(volume * 100)}%";
    }

    public void SetMasterVolume(float volume) {
        AudioController.GetInstance().SetMasterVolume(volume);
        masterVolumeText.text = $"Master: {(int)(volume * 100)}%";
    }

    public void ResolutionLeft() {
        if(currentResolutionIndex > 0) {
            currentResolutionIndex--;
        } else {
            currentResolutionIndex = resolutions.Length - 1;
        }
        resolutionText.text = $"{resolutions[currentResolutionIndex].width}x{resolutions[currentResolutionIndex].height}";
    }
    
    public void ResolutionRight() {
        if(currentResolutionIndex < resolutions.Length - 1) {
            currentResolutionIndex++;
        } else {
            currentResolutionIndex = 0;
        }
        resolutionText.text = $"{resolutions[currentResolutionIndex].width}x{resolutions[currentResolutionIndex].height}";
    }

    public void ApplyGraphics() {
        Screen.fullScreen = fullscreenToggle.isOn;
        if(vsyncToggle.isOn) {
            QualitySettings.vSyncCount = 1;
        } else {
            QualitySettings.vSyncCount = 0;
        }
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        FullScreenMode fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        if(!fullscreenToggle.isOn) {
            fullScreenMode = FullScreenMode.Windowed;
        }
        Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, fullScreenMode);
        resolutionText.text = $"{resolutions[currentResolutionIndex].width}x{resolutions[currentResolutionIndex].height}";
    }

}

[System.Serializable]
public struct Resolution {
    public int width;
    public int height;
    public Resolution(int width, int height) {
        this.width = width;
        this.height = height;
    }
}
