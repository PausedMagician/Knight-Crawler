using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{

    public List<string> NpcDialogue;
    public List<string> NpcQuestion;
    int i = 0;
    int j = 0;

    void OnTriggerStay2D(Collider2D coll)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (NpcDialogue[i] == "q"){Debug.LogAssertion(NpcQuestion[j]);j++;i++;}
            Debug.LogWarning(NpcDialogue[i]);
            if(i == NpcDialogue.Count - 1) {i = NpcDialogue.Count - 1; return;}
            i++;

        }
    }

    public float Speed;
    public float mini; 
    public Transform target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, target.position) > mini)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, Speed * Time.deltaTime);
        }
    }
}
