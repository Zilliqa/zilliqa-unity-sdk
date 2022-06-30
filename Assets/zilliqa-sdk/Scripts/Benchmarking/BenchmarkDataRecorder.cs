using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RPCBenchmarkingManager))]
public class BenchmarkDataRecorder : MonoBehaviour
{
    public delegate void DisplayLogsEvent(ILoggable logs);

    public static BenchmarkDataRecorder Instance;
    public TMP_Text LoggerTextArea;

    public DisplayLogsEvent OnDisplayLogs;

    private Dictionary<string, BenchmarkGroupInfo> benchmarkHistory;
    private List<RPCPrefabController> benchmarkBatch;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        benchmarkBatch = new List<RPCPrefabController>();
        benchmarkHistory = new Dictionary<string, BenchmarkGroupInfo>();
        RPCBenchmarkingManager.Instance.OnBenchmarkingStarted += BenchStart;
        RPCBenchmarkingManager.Instance.OnBenchmarkingComplete += BenchComplete;
        RPCBenchmarkingManager.Instance.OnBatchBenchmarking += () => StartCoroutine(RecordBatch());
        FileExporter.OnFileSaved += DisplayFileSaveFeedback;
    }

    private void BenchStart(BenchmarkInfo info)
    {
        info.startTime = Time.realtimeSinceStartup;
    }

    private void BenchComplete(BenchmarkInfo info)
    {
        info.finishTime = Time.realtimeSinceStartup;

        AddToBenchmarkGroup(benchmarkHistory, info);

        DisplayLogs(benchmarkHistory[info.FunctionName]);
    }

    public void DisplayLogs(BenchmarkGroupInfo groupInfo)
    {
        var blankStr = "There is no recorded data for this RPC, Please click on the button with the RPC name to start recording";

        LoggerTextArea.text = groupInfo == null ? blankStr : groupInfo.GetLogs(true).ToString();

        OnDisplayLogs?.Invoke(groupInfo);
    }

    public void DisplayLogs(string groupInfoName)
    {
        var groupInfo = benchmarkHistory.ContainsKey(groupInfoName) ?
            benchmarkHistory[groupInfoName] :
            null;

        DisplayLogs(groupInfo);
    }

    public void DisplayBatchLogs(BenchmarkBatchInfo batchInfo)
    {
        var blankStr = "There is no recorded data for this RPC, Please click on the button with the RPC name to start recording";

        LoggerTextArea.text = batchInfo == null ? blankStr : batchInfo.GetLogs(true).ToString();

        OnDisplayLogs?.Invoke(batchInfo);
    }

    private void AddToBenchmarkGroup(Dictionary<string, BenchmarkGroupInfo> groupsInfo, BenchmarkInfo info)
    {
        if (!groupsInfo.ContainsKey(info.FunctionName))
        {
            groupsInfo.Add(info.FunctionName, new BenchmarkGroupInfo(info.FunctionName));
        }
        groupsInfo[info.FunctionName].benchmarks.Add(info);
    }

    public void AddGroupToBatch(RPCPrefabController rpcTask)
    {
        if (!benchmarkBatch.Contains(rpcTask))
            benchmarkBatch.Add(rpcTask);
    }

    public void RemoveGroupFromBatch(RPCPrefabController rpcTask)
    {
        benchmarkBatch.Remove(rpcTask);
    }

    public IEnumerator RecordBatch()
    {
        LoggerTextArea.text = "";
        var benchmarkBatchInfo = new BenchmarkBatchInfo();
        foreach (var item in benchmarkBatch)
        {
            StartCoroutine(item.ExecuteRPC((info) =>
            {
                info.finishTime = Time.realtimeSinceStartup;
                AddToBenchmarkGroup(benchmarkBatchInfo.benchmarkGroups, info);
            }));
        }

        while (benchmarkBatchInfo.benchmarkGroups.Count < benchmarkBatch.Count)
        {
            yield return new WaitForEndOfFrame();
        }

        DisplayBatchLogs(benchmarkBatchInfo);
    }

    public void DisplayFileSaveFeedback(string path)
    {
        var savePath = LoggerTextColors.SetColor("Logs saved successfully at: " + path, "#49C0BE");
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(savePath);
        sb.AppendLine(" ");
        sb.AppendLine(LoggerTextArea.text);

        LoggerTextArea.text = sb.ToString();
    }
}
