using GamingHub_SDK.Models.Payloads;
using System;

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

  
}
