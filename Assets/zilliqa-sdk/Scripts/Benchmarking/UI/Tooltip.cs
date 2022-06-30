using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject toolTip;
    public Transform tooltipPos;
    public string Content;

    
    void Start()
    {
        // I added this in case I forgot to set the tooltip object
        if (toolTip != null)
        {
            toolTip.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Same here
        if (toolTip != null)
        {
            var txt = toolTip.GetComponentInChildren<TMP_Text>();
            txt.text = Content;
            var recttrans = (RectTransform)toolTip.transform;
            recttrans.position = new Vector3(tooltipPos.position.x, tooltipPos.position.y, recttrans.position.z);
            toolTip.SetActive(true);
           
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // and same here
        if (toolTip != null)
        {
            toolTip.SetActive(false);
        }
    }
}
