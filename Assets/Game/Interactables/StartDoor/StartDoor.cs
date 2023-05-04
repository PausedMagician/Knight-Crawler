using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartDoor : Interactable
{
    override public void Interact(Player player) {
        base.Interact(player);
        SceneManager.UnloadSceneAsync(sceneName:"Start");
        SceneManager.LoadSceneAsync(sceneName:"Main Game");
    }
}
