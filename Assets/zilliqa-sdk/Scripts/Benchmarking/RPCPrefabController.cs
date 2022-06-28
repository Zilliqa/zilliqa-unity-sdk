using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RPCPrefabController : MonoBehaviour
{
    [SerializeField]
    private Button rpcButton;
    [SerializeField]
    private TMP_InputField triesIF;
    [SerializeField]
    private TMP_InputField delayIF;
    [SerializeField]
    private Button detailsButton;
    [SerializeField]
    private Toggle IncludeInBatchRecordingToggle;

    public string rpcName;
    private Action<Action<BenchmarkInfo>> rpcTask;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(string buttonLabel, Action<Action<BenchmarkInfo>> onButtonClick)
    {
        rpcTask = onButtonClick;

        rpcButton.GetComponentInChildren<TMP_Text>().text = buttonLabel;
        rpcName = buttonLabel;
        rpcButton.onClick.AddListener(() => StartCoroutine(ExecuteRPC((info) => { RPCBenchmarkingManager.Instance.OnBenchmarkingComplete(info); })));
        detailsButton.onClick.AddListener(() => BenchmarkDataRecorder.Instance.DisplayLogs(rpcName));
        IncludeInBatchRecordingToggle.onValueChanged.AddListener(
            (toggled) =>
            {
                if (toggled)
                    BenchmarkDataRecorder.Instance.AddGroupToBatch(this);
                else
                    BenchmarkDataRecorder.Instance.RemoveGroupFromBatch(this);
            });
    }

    public IEnumerator ExecuteRPC(Action<BenchmarkInfo> onBenchmarkingComplete = null)
    {
        int totalTries = Convert.ToInt32(triesIF.text);
        float delay = (float)Convert.ToDouble(delayIF.text);
        int triesCount = 0;

        while (triesCount < totalTries)
        {
            triesCount++;
            rpcTask?.Invoke(onBenchmarkingComplete);

            yield return triesCount == totalTries ? null : new WaitForSecondsRealtime(delay);
        }


    }

    public void IncludeInBatchRecording(bool include)
    {
        IncludeInBatchRecordingToggle.isOn = include;
    }
}

public class BenchmarkInfo
{
    public float startTime;
    public float finishTime;
    public string FunctionName;
    public float ExecutionTime => finishTime - startTime;

    public BenchmarkInfo()
    {

    }
    public BenchmarkInfo(string functionName)
    {
        FunctionName = functionName;
    }

    public BenchmarkInfo(string functionName, float _startTime, float _finishTime)
    {
        FunctionName = functionName;
        startTime = _startTime;
        finishTime = _finishTime;
    }

}

public class BenchmarkGroupInfo
{
    public List<BenchmarkInfo> benchmarks;
    public string FunctionName;

    public float AverageExecutionTime => benchmarks.Select(info => info.ExecutionTime).Average();
    public float MaxExecutionTime => benchmarks.Select(info => info.ExecutionTime).Max();
    public float MinExecutionTime => benchmarks.Select(info => info.ExecutionTime).Min();

    public BenchmarkGroupInfo(string functionName, params BenchmarkInfo[] infos)
    {
        FunctionName = functionName;
        benchmarks = new List<BenchmarkInfo>(infos);
    }

    public static BenchmarkGroupInfo LongestExecutionTime
        => new BenchmarkGroupInfo(
            "GROUP_LONGEST_EXEC_TIME",
            new BenchmarkInfo("LONGEST_EXEC_TIME", 0, float.MaxValue));

    public StringBuilder GetDetailedLogs()
    {
        var sb = GetLogs();

        sb.AppendLine("");
        sb.AppendLine("Details");
        sb.AppendLine("----------------");
        for (int i = 0; i < benchmarks.Count; i++)
        {
            sb.AppendLine("Try " + (i + 1) + " " + benchmarks[i].ExecutionTime + "s");
        }
        return sb;
    }

    public StringBuilder GetLogs()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Stats for " + FunctionName + " over " + benchmarks.Count + " tries");
        sb.AppendLine("----------------");
        sb.AppendLine("Average time " + AverageExecutionTime);
        sb.AppendLine("Min time " + MinExecutionTime);
        sb.AppendLine("Max time " + MaxExecutionTime);

        return sb;
    }
    public override string ToString()
    {
        return base.ToString();
    }

}

public class BenchmarkBatchInfo
{
    public Dictionary<string, BenchmarkGroupInfo> benchmarkGroups;

    public BenchmarkGroupInfo FastestRPC
        => benchmarkGroups.Values.Aggregate(
            BenchmarkGroupInfo.LongestExecutionTime,
            (fastest, next) => fastest.MinExecutionTime <= next.MinExecutionTime ? fastest : next);
    public BenchmarkBatchInfo()
    {
        benchmarkGroups = new Dictionary<string, BenchmarkGroupInfo>();
    }

    public StringBuilder GetLogs()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Fastest RPC " + FastestRPC.FunctionName + " over " + FastestRPC.MinExecutionTime + "s");
        sb.AppendLine("");
        sb.AppendLine("Details");
        sb.AppendLine("----------------");
        foreach (var item in benchmarkGroups.Keys)
        {
            sb.AppendLine(benchmarkGroups[item].FunctionName + " => " + benchmarkGroups[item].MinExecutionTime);
        }

        return sb;
    }
}