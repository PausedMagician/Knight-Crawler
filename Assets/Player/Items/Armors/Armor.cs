using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item
{
    public List<Effect> effects = new List<Effect>();
    public Sprite sprite;
    new public string name;
    public int cost;
    public int level;
    public int rarity;
}
