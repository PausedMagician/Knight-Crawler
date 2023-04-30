using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public GameObject main, settings;

    public void Settings() {
        main.SetActive(false);
        settings.SetActive(true);
    }
    public void Return() {
        main.SetActive(true);
        settings.SetActive(false);
    }


    //Settings

    
    public void ApplySettings() {
        
    }

}
