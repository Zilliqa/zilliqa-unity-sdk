using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphQlClient.Core;
using static GraphQlClient.Core.GraphApi;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public class GetNFTsByUser : MonoBehaviour
{
    public GraphApi indexer;
    private string indexerToken = "yQqD5eiBkK4mvkrXrNdvAUi2pJ03DTpdCe6a0gvUMyUW9KELHBJdGULvVi2WeaVc";

   
    public async void DoQuery()
    {
        indexer.SetAuthToken(indexerToken);
        
        Query q = indexer.GetQueryByName("NFTsByWallet", Query.Type.Query);
        q.SetArgs(new { input = new { address = "0x8254b2c9acdf181d5d6796d63320fbb20d4edd12"} });

        UnityWebRequest re = await indexer.Post(q);
        
        var response = JsonConvert.DeserializeObject<NFTsByUserResponse>(re.downloadHandler.text);

        
        Debug.Log("GetNFTsByUser Object\n" + JsonConvert.SerializeObject(response));
    }

    private void Start()
    {
        DoQuery();
    }
}

[Serializable]
public class NFTsByUserResponse
{
    public NFTsByUserPayload data;
}

[Serializable]
public class NFTsByUserPayload
{
    public UserNFTs user;
}

[Serializable]
public class UserNFTs
{
    public string address;
    public NFTListPayload ownedAssets;
    public NFTListPayload mintedAssets;
}

[Serializable]
public class NFTListPayload
{
    public NFTAsset[] assetsList;
}

[Serializable]
public class NFTAsset
{
    public string contractAddress;
    public string tokenId;
    public string tokenUri;
    public string name;
    public string resource;
    public string minterAddress;
    public string spenderAddress;
    public string[] operatorAddress;
}