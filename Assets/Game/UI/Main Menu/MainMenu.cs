using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public GameObject startMenu;
    public GameObject pauseMenu;

    public void ToggleMenu() {
        if(pauseMenu.activeSelf) {
            Time.timeScale = 1;
        } else {
            Time.timeScale = 0;
        }
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    public void StartGame() {
        startMenu.SetActive(false);
    }

    public void ReturnToMenu() {
        startMenu.SetActive(true);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
