using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : Interactable
{
    public MapGenerator mapGenerator;
    override public void Interact(Player player) {
        base.Interact(player);
        player.targetedBy.Clear();
        mapGenerator.level++;
        mapGenerator.GenerateMap();
        mapGenerator.gameController.SendPlayerToBonfire();
    }
}
