using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public GameObject startMenu;
    public GameObject pauseMenu;
    public MapGenerator mapGenerator;
    public MapGenerator mapGeneratorPreview;

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
        if(mapGenerator) {
            if(mapGenerator.GenerateMap()) {
                mapGeneratorPreview.ClearMap();
                mapGeneratorPreview.gameController.EndGame();
                mapGenerator.gameController.StartGame();
            }
        }
    }

    public void ReturnToMenu() {
        mapGeneratorPreview.Start();
        startMenu.SetActive(true);
        mapGenerator.ClearMap();
        mapGenerator.gameController.EndGame();
    }

    public void QuitGame() {
        Application.Quit();
    }
}
