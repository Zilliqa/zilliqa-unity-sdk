using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphQlClient.Core;
using static GraphQlClient.Core.GraphApi;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public class GetNFTMetadata : MonoBehaviour
{
    public GraphApi indexer;
    public string contractAddress = "0xe52cb846a86ffe28b3c40e99f3c842e6ad55b594";
    public string tokenId = "47";


    private string indexerToken = "yQqD5eiBkK4mvkrXrNdvAUi2pJ03DTpdCe6a0gvUMyUW9KELHBJdGULvVi2WeaVc";
    public async void DoQuery()
    {
        indexer.SetAuthToken(indexerToken);

        Query q = indexer.GetQueryByName("NFTMetadata", Query.Type.Query);
        q.SetArgs(new { input = new { contractAddress, tokenId } });

        UnityWebRequest re = await indexer.Post(q);
        Debug.Log("GetNFTMetadata GraphQL query\n" + q.query);
        Debug.Log("GetNFTMetadata response\n" + re.downloadHandler.text);

        var response = JsonConvert.DeserializeObject<GetNFTMetadataResponse>(re.downloadHandler.text);


        Debug.Log("GetNFTMetadata Object\n" + JsonConvert.SerializeObject(response));
    }

    private void Start()
    {
        DoQuery();
    }

    public class GetNFTMetadataResponse
    {
        public GetNFTMetadataPayload data;
    }

    public class GetNFTMetadataPayload
    {
        public NFTById assetById;
    }

    public class NFTById
    {
        public string tokenId;
        public string tokenUri;
        public string name;
        public string description;
        public string externalUrl;
        public string ownerAddress;
        public string minterAddress;
        public string contractAddress;
        public string resource;
        public string resourceMimetype;
        public NFTAttributes[] attributes;
    }

    public class NFTAttributes
    {
        public string traitType;
        public string value;
    }
}
