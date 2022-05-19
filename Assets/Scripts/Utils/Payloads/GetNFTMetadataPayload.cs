using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zilliqa.Requests
{
    [Serializable]
    public class GetNFTMetadataPayload
    {
        public NFTById assetById;
    }

    [Serializable]
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

    [Serializable]
    public class NFTAttributes
    {
        public string traitType;
        public string value;
    }
}
