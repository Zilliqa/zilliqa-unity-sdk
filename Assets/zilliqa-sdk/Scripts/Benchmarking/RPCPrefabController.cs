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

public abstract class BaseBenchmarkInfo : ILoggable
{
    public string FunctionName;

    public abstract StringBuilder GetLogs(bool richText = false);
}

public class BenchmarkInfo : BaseBenchmarkInfo
{
    public float startTime;
    public float finishTime;
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

    public override StringBuilder GetLogs(bool richText = false)
    {
        return new StringBuilder();
    }
}

public interface ILoggable
{
    public StringBuilder GetLogs(bool richText = false);
}

public class BenchmarkGroupInfo : BaseBenchmarkInfo
{
    public List<BenchmarkInfo> benchmarks;

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

    public static BenchmarkGroupInfo ShortestExecutionTime
    => new BenchmarkGroupInfo(
        "GROUP_SHORTEST_EXEC_TIME",
        new BenchmarkInfo("SHORTEST_EXEC_TIME", 0, 0));

    public override StringBuilder GetLogs(bool richText = false)
    {
        var average = AverageExecutionTime;
        var min = MinExecutionTime;
        var max = MaxExecutionTime;

        var avColorHex = LoggerTextColors.GetColorHex(average, MaxExecutionTime, MinExecutionTime, higherIsBetter: false);
        var minColorHex = LoggerTextColors.GetColorHex(min, MaxExecutionTime, MinExecutionTime, higherIsBetter: false);
        var maxColorHex = LoggerTextColors.GetColorHex(max, MaxExecutionTime, MinExecutionTime, higherIsBetter: false);

        var avStr = "Average time " + average + "s";
        var minStr = "Min time " + min + "s";
        var maxStr = "Max time " + max + "s";

        var sb = new StringBuilder();
        sb.AppendLine("Stats for " + FunctionName + " over " + benchmarks.Count + " calls");
        sb.AppendLine("----------------");
        sb.AppendLine(richText ? LoggerTextColors.SetColor(avStr, avColorHex) : avStr);
        sb.AppendLine(richText ? LoggerTextColors.SetColor(minStr, minColorHex) : minStr);
        sb.AppendLine(richText ? LoggerTextColors.SetColor(maxStr, maxColorHex) : maxStr);

        sb.AppendLine("");
        sb.AppendLine("Details");
        sb.AppendLine("----------------");

        for (int i = 0; i < benchmarks.Count; i++)
        {
            var colorHex = LoggerTextColors.GetColorHex(benchmarks[i].ExecutionTime, max, min, higherIsBetter: false);

            var coloredFunctionName = "#" + (i + 1);

            if (richText &&
                (benchmarks[i].ExecutionTime == min || benchmarks[i].ExecutionTime == max))
            {
                coloredFunctionName = LoggerTextColors.SetColor(coloredFunctionName, colorHex);
            }

            var timeStr = " => " + benchmarks[i].ExecutionTime + "s";

            sb.AppendLine(coloredFunctionName +
                (richText ? LoggerTextColors.SetColor(timeStr, colorHex) : timeStr));
        }

        return sb;
    }
    public override string ToString()
    {
        return base.ToString();
    }

}

public class BenchmarkBatchInfo : BaseBenchmarkInfo
{
    public Dictionary<string, BenchmarkGroupInfo> benchmarkGroups;

    public BenchmarkGroupInfo FastestRPC
        => benchmarkGroups.Values.Aggregate(
            BenchmarkGroupInfo.LongestExecutionTime,
            (fastest, next) => fastest.MinExecutionTime <= next.MinExecutionTime ? fastest : next);

    public BenchmarkGroupInfo SlowestRPC
        => benchmarkGroups.Values.Aggregate(
            BenchmarkGroupInfo.ShortestExecutionTime,
            (slowest, next) => slowest.MinExecutionTime > next.MinExecutionTime ? slowest : next);
    public BenchmarkBatchInfo()
    {
        benchmarkGroups = new Dictionary<string, BenchmarkGroupInfo>();
        FunctionName = "BatchRecord";
    }

    public override StringBuilder GetLogs(bool richText = false)
    {
        var fastest = FastestRPC;
        var slowest = SlowestRPC;
        var sb = new StringBuilder();

        sb.AppendLine("Fastest RPC " + fastest.FunctionName + " over " + fastest.MinExecutionTime + "s");
        sb.AppendLine("");
        sb.AppendLine("Details");
        sb.AppendLine("----------------");
        foreach (var item in benchmarkGroups.Keys)
        {
            var colorHex = LoggerTextColors.GetColorHex(benchmarkGroups[item].MinExecutionTime, slowest.MinExecutionTime, fastest.MinExecutionTime, higherIsBetter: false);

            var coloredFunctionName = benchmarkGroups[item].FunctionName;

            if (richText &&
                (benchmarkGroups[item].FunctionName == fastest.FunctionName || benchmarkGroups[item].FunctionName == slowest.FunctionName))
            {
                coloredFunctionName = LoggerTextColors.SetColor(coloredFunctionName, colorHex);
            }
            var timeStr = " => " + benchmarkGroups[item].MinExecutionTime + "s";

            sb.AppendLine(coloredFunctionName +
                (richText ? LoggerTextColors.SetColor(timeStr, colorHex) : timeStr));
        }

        return sb;
    }
}