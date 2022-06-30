using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogsExporter : MonoBehaviour
{
    public static LogsExporter Instance;

    [SerializeField]
    private Button Exportbutton;
    [SerializeField]
    private Button ShowInExplorerbutton;

    private ILoggable displayedLogs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        BenchmarkDataRecorder.Instance.OnDisplayLogs += FetchDisplayedLogs;
        Exportbutton.onClick.AddListener(() => { ExportLogs(); });
        ShowInExplorerbutton.onClick.AddListener(() => { ShowExplorer(); });
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FetchDisplayedLogs(ILoggable logs)
    {
        Exportbutton.interactable = logs != null;
        displayedLogs = logs;
    }

    public void ExportLogs()
    {
        var asBenchInfo = displayedLogs as BaseBenchmarkInfo;
        if (asBenchInfo != null)
        {
            FileExporter.WriteLogFile(displayedLogs.GetLogs().ToString(), asBenchInfo.FunctionName);

        }

    }

    public void ShowExplorer()
    {
        string itemPath = (Application.persistentDataPath + @"\").Replace(@"/", @"\");
        System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
    }
}
