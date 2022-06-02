using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphQlClient.Core;
using static GraphQlClient.Core.GraphApi;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using Zilliqa.Requests;

public class GetEnglishAuctionOffers : MonoBehaviour
{
    public GraphApi indexer;
    public string contractAddress = "0x0184398def9b2e48a826d67116229e79b889873c";
    public string tokenId = "47";


    private string indexerToken = "yQqD5eiBkK4mvkrXrNdvAUi2pJ03DTpdCe6a0gvUMyUW9KELHBJdGULvVi2WeaVc";
    public async void DoQuery()
    {
        //contractAddress = TestWallets.FixedPriceSmartContract0;
        indexer.SetAuthToken(indexerToken);

        Query query = indexer.GetQueryByName("GetAuctionOffers", Query.Type.Query);
        query.SetArgs(new { input = new { contractAddress = this.contractAddress, tokenId = this.tokenId } });

        UnityWebRequest re = await indexer.Post(query);
        //var response = JsonConvert.DeserializeObject<GetNFTMetadataResponse>(re.downloadHandler.text);

        //Debug.Log("GetFixedPriceOffers Object\n" + JsonConvert.SerializeObject(response));
        Debug.Log("GetAuctionOffers Object\n" + re.downloadHandler.text);
    }

    private void Start()
    {
        DoQuery();
    }


}
