using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zilliqa.Core;
using UnityEngine.UI;

public class RPCBenchmarkingManager : MonoBehaviour
{
    public delegate void BenchmarkingEvent(BenchmarkInfo info);
    public delegate void GroupBenchmarkingEvent(params BenchmarkInfo[] info);
    public delegate void BatchBenchmarkingEvent();

    public static RPCBenchmarkingManager Instance;

    public BenchmarkingEvent OnBenchmarkingStarted;
    public BenchmarkingEvent OnBenchmarkingComplete;
    public BatchBenchmarkingEvent OnBatchBenchmarking;
    [SerializeField]
    private GameObject rpcPrefab;
    [SerializeField]
    private Button batchBenchmarkingButton;
    //[SerializeField]
    //private Button IncludeAllToBatchButton;
    //[SerializeField]
    //private Button ExcludeAllFromBatchButton;
    [SerializeField]
    private List<TabPanelController> tabs;

    private Wallet sender;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Init()
    {
        sender = new Wallet(TestWallets.WalletPK1);

        batchBenchmarkingButton.onClick.AddListener(() => OnBatchBenchmarking?.Invoke());
        //IncludeAllToBatchButton.onClick.AddListener(() => IncludeAllToBatch());
        //ExcludeAllFromBatchButton.onClick.AddListener(() => ExcludeAllFomBatch());

        tabs[0].Init("Acount", GenerateAccountUI(tabs[0].contentPane));
        tabs[1].Init("Blockchain", GenerateBlockchainUI(tabs[1].contentPane));
        tabs[2].Init("Transaction", GenerateTransactionUI(tabs[2].contentPane));

        
    }

    public void IncludeAllToBatch()
    {
        foreach (var tab in tabs)
        {
            tab.ToggleAll(true);
        }
    }

    public void ExcludeAllFomBatch()
    {
        foreach (var tab in tabs)
        {
            tab.ToggleAll(false);
        }
    }

    private RPCPrefabController[] GenerateAccountUI(RectTransform _parent)
    {
        List<RPCPrefabController> elements = new List<RPCPrefabController>();

        RPCPrefabController rpcUI = AddRPC("GetBalance",
            (onComplete) =>
            {
                var info = new BenchmarkInfo("GetBalance"); OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetBalance(sender, (response) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        return elements.ToArray();
    }




    private RPCPrefabController[] GenerateBlockchainUI(RectTransform _parent)
    {
        List<RPCPrefabController> elements = new List<RPCPrefabController>();

        RPCPrefabController rpcUI = AddRPC("GetLatestTxBlock",
            (onComplete) =>
            {
                var info = new BenchmarkInfo("GetLatestTxBlock");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetLatestTxBlock((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetDSBlockListing", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetDSBlockListing");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetDSBlockListing(1, (response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetBlockchainInfo", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetBlockchainInfo");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetBlockchainInfo((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetCurrentDSEpoch", (onComplete) => 
        {
            var info = new BenchmarkInfo("GetCurrentDSEpoch");
            OnBenchmarkingStarted?.Invoke(info);
            _ = ZilliqaRPC.GetCurrentDSEpoch((response, error) => onComplete?.Invoke(info));
        }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetCurrentMiniEpoch", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetCurrentMiniEpoch"); OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetCurrentMiniEpoch((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetDsBlock", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetDsBlock");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetDsBlock("5000", (response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetDSBlockRate", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetDSBlockRate");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetDSBlockRate((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetLatestDsBlock", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetLatestDsBlock");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetLatestDsBlock((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetMinerInfo", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetMinerInfo");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetMinerInfo("5500", (response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetNetworkId", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetNetworkId");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetNetworkId((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetNumDSBlocks", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetNumDSBlocks");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetNumDSBlocks((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetNumTransactions", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetNumTransactions");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetNumTransactions((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetNumTxBlocks", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetNumTxBlocks");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetNumTxBlocks((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetPrevDifficulty", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetPrevDifficulty");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetPrevDifficulty((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetPrevDSDifficulty", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetPrevDSDifficulty");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetPrevDSDifficulty((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetTotalCoinSupply", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetTotalCoinSupply");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetTotalCoinSupply((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetTransactionRate", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetTransactionRate"); OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetTransactionRate((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetTxBlock", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetTxBlock");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetTxBlock("1002353", (response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("TxBlockListing", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("TxBlockListing");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.TxBlockListing(1, (response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetTxBlockRate", 
            (onComplete) => 
            {
                var info = new BenchmarkInfo("GetTxBlockRate");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetTxBlockRate((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        return elements.ToArray();
    }

    private RPCPrefabController[] GenerateTransactionUI(RectTransform _parent)
    {
        List<RPCPrefabController> elements = new List<RPCPrefabController>();

      RPCPrefabController rpcUI = AddRPC("GetMinimumGasPrice",
            (onComplete) =>{
                var info = new BenchmarkInfo("GetMinimumGasPrice");OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetMinimumGasPrice((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetNumTxnsDSEpoch", 
            (onComplete) =>
            {
                var info = new BenchmarkInfo("GetNumTxnsDSEpoch");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetNumTxnsDSEpoch((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetNumTxnsTxEpoch", 
            (onComplete) =>
            {
                var info = new BenchmarkInfo("GetNumTxnsTxEpoch");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetNumTxnsTxEpoch((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetRecentTransactions", 
            (onComplete) =>
            {
                var info = new BenchmarkInfo("GetRecentTransactions");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetRecentTransactions((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetTransaction", 
            (onComplete) =>
            {
                var info = new BenchmarkInfo("GetTransaction");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetTransaction((response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetTransactionsForTxBlock", 
            (onComplete) =>
            {
                var info = new BenchmarkInfo("GetTransactionsForTxBlock");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetTransactionsForTxBlock("5000", (response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetTransactionsForTxBlockEx", 
            (onComplete) =>
            {
                var info = new BenchmarkInfo("GetTransactionsForTxBlockEx");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetTransactionsForTxBlockEx("1002353", "1", (response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetTxnBodiesForTxBlock", 
            (onComplete) =>
            {
                var info = new BenchmarkInfo("GetTxnBodiesForTxBlock");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetTxnBodiesForTxBlock("1", (response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        rpcUI = AddRPC("GetTxnBodiesForTxBlockEx", 
            (onComplete) =>
            {
                var info = new BenchmarkInfo("GetTxnBodiesForTxBlockEx");
                OnBenchmarkingStarted?.Invoke(info);
                _ = ZilliqaRPC.GetTxnBodiesForTxBlockEx("1002353", "1", (response, error) => onComplete?.Invoke(info));
            }, _parent);
        elements.Add(rpcUI);

        return elements.ToArray();
    }

    private RPCPrefabController AddRPC(string buttonLabel, Action<Action<BenchmarkInfo>> rpc, RectTransform parent)
    {
        var rpcObject = Instantiate(rpcPrefab, parent).GetComponent<RPCPrefabController>();
        rpcObject.Init(buttonLabel, rpc);

        return rpcObject;
    }


}
