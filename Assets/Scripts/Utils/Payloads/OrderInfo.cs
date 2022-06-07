using UnityEngine;
using System;
using Zilliqa.Requests;

namespace Zilliqa.Marketplace
{
    public enum OrderState
    {
        Active,
        Expired,
        Ended
    }

    public enum MarketplaceType
    {
        FixedPrice,
        Auction
    }

    [Serializable]
    public class OrderInfo
    {
        public string paymentMethod;
        public string price;
        public string seller;
        public string tokenAddress;
        public string marketplaceAddress;
        public string tokenId;
        public string expirationBlockNumber;
        public Sprite tokenImage;
        public NFT tokenInfo;
        public OrderState state;
        public OrderType type;
        public MarketplaceType marketplaceType;
        public string minimumBidIncrement;
        public string comissionFees;
    }
}
