using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zilliqa.Requests
{
    [Serializable]
    public class GetNFTMetadataPayload
    {
        public NFT assetById;
    }

    [Serializable]
    public class NFT
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

    [Serializable]
    public class GetAuctionOffersByNFTIdPayload
    {
        public AuctionedNFT assetById;
    }

    public class GetFixedPricedOffersByNFTIdPayload
    {
        public FixedPricedNFT assetById;
    }


    [Serializable]
    public class FixedPricedNFT : NFT
    {
        public FixedPriceListingDataWrapper[] listingData;
    }

    public class FixedPriceListingDataWrapper
    {
        public string sourceId;
        public FixedPriceListing data;

    }

    [Serializable]
    public class FixedPriceListing
    {
        public FixedPriceListingData ZILLIQA_FIXED_PRICE;
    }

    [Serializable]
    public class FixedPriceListingData
    {
        public FixedPriceOffer[] othersOffers;
        public FixedPriceOffer[] ownerOffers;
        public string marketplaceContractAddress;
    }

    [Serializable]
    public class FixedPriceOffer
    {
        public string maker;
        public string unit;
        public long amount;
        public string currency;
        public string expirationInBlockNumber;
    }

    [Serializable]
    public class AuctionedNFT : NFT
    {
        public AuctionListingDataWrapper[] listingData;
    }

    [Serializable]
    public class AuctionListingDataWrapper
    {
        public string sourceId;
        public AuctionListing data;

    }

    [Serializable]
    public class AuctionListing
    {
        public AuctionListingData ZILLIQA_ENGLISH_AUCTION;
        public AuctionFulfillmentInfo fulfilled;
    }

    [Serializable]
    public class AuctionListingData
    {
        public AuctionBid startingBid;
        public AuctionBid winnerBid;
        public AuctionBid[] Bids;
        public string maker;
        public string marketplaceContractAddress;

        public long CurrentBid => Bids.Length > 0 ? Bids.ToList().Max(bid => bid.amount) : startingBid.amount;

    }

    [Serializable]
    public class AuctionBid
    {
        public string unit;
        public long amount;
        public string currency;
    }

    [Serializable]
    public class AuctionFulfillmentInfo
    {
        public string __typename;
        public int id;
        public string marketplaceContractAddress;
        public long amount;
        public string buyer;
        public string currency;
        public string seller;
        public string unit;
        public string assetRecipient;
        public string paymentTokensRecipient;
        public string royaltyRecipient;
    }
}
