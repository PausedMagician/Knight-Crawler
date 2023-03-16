using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemUI itemUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.OnMouseHover(itemUI.item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.OnMouseExit();
    }

    // Start is called before the first frame update
    void Awake()
    {
        this.itemUI = gameObject.GetComponent<ItemUI>();
    }

}
