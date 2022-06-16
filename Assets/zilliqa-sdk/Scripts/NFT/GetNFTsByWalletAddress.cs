using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphQlClient.Core;
using static GraphQlClient.Core.GraphApi;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using Zilliqa.Requests;

public class GetNFTsByWalletAddress : MonoBehaviour
{
    public string walletAddress = "0x8254b2C9aCdf181d5d6796d63320fBb20D4Edd12";
    public GraphApi indexer;
    private string indexerToken = "yQqD5eiBkK4mvkrXrNdvAUi2pJ03DTpdCe6a0gvUMyUW9KELHBJdGULvVi2WeaVc";

   
    public async void DoQuery()
    {
        indexer.SetAuthToken(indexerToken);
        
        Query query = indexer.GetQueryByName("GetNFTsByWalletAddress", Query.Type.Query);
        query.SetArgs(new { input = new { address = walletAddress} });

        UnityWebRequest re = await indexer.Post(query);
        var response = JsonConvert.DeserializeObject<NFTsByUserResponse>(re.downloadHandler.text);

        Debug.Log("GetNFTsByUser Object\n" + JsonConvert.SerializeObject(response));
    }

    private void Start()
    {
        DoQuery();
    }
}



