using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabPanelController : MonoBehaviour
{
    public RectTransform contentPane;
    public RectTransform tab;

    [SerializeField]
    private ScrollRect scrollRect;

    private TMP_Text tabLabel;
    private Button tabButton;

    private List<RPCPrefabController> uiElements;
    private void Awake()
    {
        tabLabel = tab.GetComponentInChildren<TMP_Text>();
        tabButton = tab.GetComponentInChildren<Button>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(string _tabName, params RPCPrefabController[] _uiElements)
    {
        SetTabName(_tabName);
        uiElements = new List<RPCPrefabController>(_uiElements);

        scrollRect.Rebuild(CanvasUpdate.MaxUpdateValue);
    }

    public void SetTabName(string name)
    {
        tabLabel.text = name;
    }

    public void ToggleAll(bool toggled)
    {
        foreach (var element in uiElements)
        {
            element.IncludeInBatchRecording(toggled);
        }
    }
}
