using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphQlClient.Core;
using static GraphQlClient.Core.GraphApi;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using Zilliqa.Requests;

public class GetNFTMetadata : MonoBehaviour
{
    public GraphApi indexer;
    public string contractAddress = "0xe52cb846a86ffe28b3c40e99f3c842e6ad55b594";
    public string tokenId = "47";


    private string indexerToken = "yQqD5eiBkK4mvkrXrNdvAUi2pJ03DTpdCe6a0gvUMyUW9KELHBJdGULvVi2WeaVc";
    public async void DoQuery()
    {
        indexer.SetAuthToken(indexerToken);

        Query query = indexer.GetQueryByName("GetNFTMetadata", Query.Type.Query);
        query.SetArgs(new { input = new { contractAddress, tokenId } });

        UnityWebRequest re = await indexer.Post(query);
        var response = JsonConvert.DeserializeObject<GetNFTMetadataResponse>(re.downloadHandler.text);

        Debug.Log("GetNFTMetadata Object\n" + JsonConvert.SerializeObject(response));
    }

    private void Start()
    {
        DoQuery();
    }


}
