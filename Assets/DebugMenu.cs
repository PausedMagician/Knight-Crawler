using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DebugMenu : MonoBehaviour
{
    public GameObject toSpawn;

    public List<GameObject> spawned;

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

}
