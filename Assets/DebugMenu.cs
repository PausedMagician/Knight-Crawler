using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DebugMenu : MonoBehaviour
{
    public static DebugMenu instance;
    private void Awake() {
        instance = this;
    }
    public static DebugMenu GetInstance() {
        if(instance) {
            return instance;
        } else {
            instance = GameObject.FindObjectOfType<DebugMenu>();
            return instance;
        }
    }

    public SelectMenu selectMenu;

    public GameObject toSpawn;

    public List<GameObject> spawned;

    public static bool selecting = false;
    public Button selectingButton;

    public void SpawnGameObject() {
        GameObject obj = Instantiate(toSpawn, Player.GetInstance().transform.position, Quaternion.identity);
        spawned.Add(obj);
        AI2 ai2;
        if(ai2 = obj.GetComponent<AI2>()) {
            ai2.defaultState = AI2.AIState.Patrol;
            ai2.reset();
        }
    }

    public void DeleteLatestGameObject() {
        GameObject obj = spawned[spawned.Count - 1];
        spawned.RemoveAt(spawned.Count - 1);
        Destroy(obj, 0.1f);
    }

    public void DeleteEarliestGameObject() {
        GameObject obj = spawned[0];
        spawned.RemoveAt(0);
        spawned = spawned.ToArray().ToList();
        Destroy(obj, 0.1f);
    }

    public void Selecting() {
        if(selecting) {
            selectingButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
            selecting = false;
        } else {
            selectingButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Selecting...";
            Invoke("TurnOnSelect", 0.1f);
        }
    }
    void TurnOnSelect() {
        selecting = true;
    }

    public void OpenSelectMenu(Humanoid selected) {
        selectMenu.gameObject.SetActive(true);
        selectMenu.selected = selected;
        selectMenu.SetUp();
    }

}
