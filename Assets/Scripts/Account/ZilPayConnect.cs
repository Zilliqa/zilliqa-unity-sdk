using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.IO;
using Zilliqa.Utils;
using Newtonsoft.Json;

public class ZilPayConnect : MonoBehaviour
{


    ZilPayWalletBrowserExtension wallet;
    Transaction transactionParam;
    private string publicKey;
    // Start is called before the first frame update
    void Start()
    {
         transactionParam = new Transaction()
        {
            version = this.version,
            nonce = this.nonce,
            // the contract address needs to be checksummed
            toAddr = AddressUtils.ToCheckSumAddress(this.toAddress),
            amount = this.amount,
            pubKey = publicKey,
            gasPrice = this.gasPrice,
            gasLimit = this.gasLimit,
            code = this.code,
            priority = this.priority,
            data = this.data
        };

        wallet = new ZilPayWalletBrowserExtension();
        wallet.ZilPayCreateTransaction(JsonConvert.SerializeObject(transactionParam));

    }

    private void OnApplicationQuit()
    {
        wallet.driver.Quit();
    }

    [SerializeField] private int version = 21823489;
    [SerializeField] private long nonce = 0;
    [SerializeField] private string toAddress = "0x638a429b1f0dc1dd206c3030295255d7dbf45501";
    [SerializeField] private string amount = "1000000000000";
    [SerializeField] private string gasPrice = "2000000000";
    [SerializeField] private string gasLimit = "50";
    [SerializeField] private string code = "";
    [SerializeField] private string data = "";
    [SerializeField] private bool priority = false;
}
