using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DynamicTextContainer : MonoBehaviour
{
    public TMP_Text content;

    private RectTransform rect;
    private float currentPreferredHeight;
    // Start is called before the first frame update

    private void Awake()
    {
        rect = (RectTransform)transform;
    }
    void Start()
    {
        SetHeight();
    }

    private void OnEnable()
    {
        SetHeight();
    }
    // Update is called once per frame
    void Update()
    {
        if (currentPreferredHeight != content.preferredHeight)
        {
            SetHeight();
        }
    }

    private void SetHeight()
    {
        if (content == null || (content.bounds.size.y < .8f * rect.rect.height && content.preferredHeight > currentPreferredHeight))
            return;

        rect = (RectTransform)transform;
        currentPreferredHeight = content.preferredHeight;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, content.preferredHeight);
    }
}
