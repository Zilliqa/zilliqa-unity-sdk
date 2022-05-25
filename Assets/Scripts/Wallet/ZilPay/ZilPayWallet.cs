using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.IO;
using Zilliqa.Utils;
using Newtonsoft.Json;

public class ZilPayWalletBrowserExtension
{
    public IWebDriver driver;
    string profileName = "*.WorkZilliqa";
    public ZilPayWalletBrowserExtension()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (driver != null)
            return;

        string pathToCurrentUserProfiles = Environment.ExpandEnvironmentVariables("%APPDATA%") + @"\Mozilla\Firefox\Profiles"; // Path to profile
        string[] pathsToProfiles = Directory.GetDirectories(pathToCurrentUserProfiles, profileName, SearchOption.TopDirectoryOnly);

        if (pathsToProfiles.Length != 0)
        {
            FirefoxProfile profile = new FirefoxProfile(pathsToProfiles[0]);
            profile.SetPreference("browser.tabs.loadInBackground", false); // set preferences you need

            var options = new FirefoxOptions();
            options.Profile = profile;
            driver = new FirefoxDriver(options);
        }
        else
        {
            driver = new FirefoxDriver();
        }
    }

    public string ZilPaySign(string message = null)
    {
        driver.Url = "https://simpay.cma-tn.com/zilliqa/";
        var result = ((IJavaScriptExecutor)driver).ExecuteScript(Sign(message));
        Debug.Log("code result " + result);
        return (string)result;
    }

    public string ZilPayCreateTransaction(string message = null)
    {
        driver.Url = "https://simpay.cma-tn.com/zilliqa/";
        var result = ((IJavaScriptExecutor)driver).ExecuteScript(CreateTransaction(message));
        Debug.Log("code result " + result);
        return (string)result;
    }

    private string Sign(string message)
    {
        var signJs = Resources.Load<TextAsset>("ZilPaySign");

        var signCode = signJs.ToString();
        signCode = signCode.Replace("$$message$$", message);
        Debug.Log(signCode);
        return signCode;
    }

    private string CreateTransaction(string message)
    {
        var signJs = Resources.Load<TextAsset>("CreateTransaction");

        var signCode = signJs.ToString();
        signCode = signCode.Replace("$$contractAddr$$", AddressUtils.ToCheckSumAddress("0xe52cb846a86ffe28b3c40e99f3c842e6ad55b594"));
        Debug.Log("Create transaction code \n" + signCode);
        return signCode;
    }

    string ConnectionJS()
    {
        var connectJs = Resources.Load<TextAsset>("ZilConnect");
        Debug.Log(connectJs.ToString());

        return connectJs.ToString();
    }

}
