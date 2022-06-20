using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphQlClient.Core;
using static GraphQlClient.Core.GraphApi;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using Zilliqa.Requests;
using System.Threading.Tasks;
using Zilliqa.Core;
using Zilliqa.Utils;

public class Indexer
{
    
    private static string indexerToken = "yQqD5eiBkK4mvkrXrNdvAUi2pJ03DTpdCe6a0gvUMyUW9KELHBJdGULvVi2WeaVc";
    public static async Task<GetNFTMetadataResponse> GetMetadata(string contractAddress, string tokenId, GraphApi api, Action<GetNFTMetadataResponse> onComplete = null)
    {
        api.SetAuthToken(indexerToken);

        api.Introspect();
        Query query = api.GetQueryByName("GetNFTMetadata", Query.Type.Query);
        query.SetArgs(new { input = new { contractAddress, tokenId } });

        UnityWebRequest re = await api.Post(query);
        var response = JsonConvert.DeserializeObject<GetNFTMetadataResponse>(re.downloadHandler.text);

        Debug.Log("GetNFTMetadata Object\n" + JsonConvert.SerializeObject(response));

        onComplete?.Invoke(response);

        return response;
    }

    public static async Task<NFTsByUserResponse> GetWalletNFTs(Wallet wallet, GraphApi api, Action<NFTsByUserResponse> onComplete = null)
    {
        api.SetAuthToken(indexerToken);

        api.Introspect();
        Query query = api.GetQueryByName("GetNFTsByWalletAddress", Query.Type.Query);
        query.SetArgs(new { input = new { address = AddressUtils.AddPrefix_0x(wallet.Address).ToLower() } });

        UnityWebRequest re = await api.Post(query);
        var response = JsonConvert.DeserializeObject<NFTsByUserResponse>(re.downloadHandler.text);

        Debug.Log("GetNFTsByUser Object\n" + JsonConvert.SerializeObject(response));

        onComplete?.Invoke(response);

        return response;
    }

    public static async Task<GetGetAuctionOffersByNFTIdResponse> GetNFTAuctionOffersByNFTId(string tokenAddress, string tokenId, GraphApi api, Action<GetGetAuctionOffersByNFTIdResponse> onComplete = null)
    {
        //contractAddress = TestWallets.FixedPriceSmartContract0;
        api.SetAuthToken(indexerToken);

        Query query = api.GetQueryByName("GetAuctionOffersByNFTId", Query.Type.Query);
        query.SetArgs(new { input = new { contractAddress = tokenAddress, tokenId = tokenId } });

        UnityWebRequest re = await api.Post(query);
        var response = JsonConvert.DeserializeObject<GetGetAuctionOffersByNFTIdResponse>(re.downloadHandler.text);

        Debug.Log("GetFixedPriceOffers Object\n" + re.downloadHandler.text);

        onComplete?.Invoke(response);

        return response;

    }

    public static async Task<GetFixedPricedOffersByNFTIdResponse> GetFixedPricedOffersByNFTId(string tokenAddress, string tokenId, GraphApi api, Action<GetFixedPricedOffersByNFTIdResponse> onComplete = null)
    {
        //contractAddress = TestWallets.FixedPriceSmartContract0;
        api.SetAuthToken(indexerToken);

        Query query = api.GetQueryByName("GetFixedPriceOffers", Query.Type.Query);
        query.SetArgs(new { input = new { contractAddress = tokenAddress, tokenId = tokenId } });

        UnityWebRequest re = await api.Post(query);
        var response = JsonConvert.DeserializeObject<GetFixedPricedOffersByNFTIdResponse>(re.downloadHandler.text);

        Debug.Log("GetFixedPriceOffers Object\n" + re.downloadHandler.text);

        onComplete?.Invoke(response);

        return response;
    }
}
