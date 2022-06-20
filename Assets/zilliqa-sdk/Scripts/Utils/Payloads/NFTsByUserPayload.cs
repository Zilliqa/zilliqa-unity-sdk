using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zilliqa.Requests
{
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
}
