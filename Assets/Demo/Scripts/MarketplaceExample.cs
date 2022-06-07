using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zilliqa.Core;
using Zilliqa.Utils;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using GraphQlClient.Core;
using Zilliqa.Requests;
using System.Threading.Tasks;
using UnityEngine.UI;

public class MarketplaceExample : MonoBehaviour
{

    public TMP_Text WalletAddressText;
    public Transform GridRoot;
    public GameObject GridItemPrefab;
    public GraphApi indexerAPI;
    public Button SellOrdersButton;
    public Button BuyOrdersButton;
    public Button MyNFTsButton;
    public string gasLimit;
    private Wallet wallet;
    private List<NFTGridItem> loadedItems;
    // Start is called before the first frame update
    void Start()
    {
        loadedItems = new List<NFTGridItem>();
        InitializeUI();
        LoadWalletInfo();
        //LoadMarketOrders(OrderType.SellOrder);
        //LoadMyNFTs();
    }

    private void InitializeUI()
    {
        SellOrdersButton.onClick.AddListener(() => { loadAllOrders(OrderType.SellOrder); });
        BuyOrdersButton.onClick.AddListener(() => { loadAllOrders(OrderType.BuyOrder); });
        MyNFTsButton.onClick.AddListener(() => { LoadMyNFTs(); });
    }

    private void loadAllOrders(OrderType orderType)
    {
        LoadMarketOrders(TestWallets.FixedPriceSmartContract0, orderType, MarketplaceType.FixedPrice);
        LoadMarketOrders(TestWallets.AuctionSmartContract0, orderType, MarketplaceType.Auction);
    }
    private void LoadWalletInfo()
    {
        wallet = new Wallet(TestWallets.WalletPK2);
        WalletAddressText.text = "0x" + wallet.Address;
    }

    private void ClearGrid()
    {
        foreach (var item in loadedItems)
        {
            Destroy(item.gameObject);
        }

        loadedItems.Clear();
    }

    private async void LoadMyNFTs()
    {
        ClearGrid();

        _ = await Indexer.GetWalletNFTs(wallet, indexerAPI, async (response) =>
        {
            var ownedAssets = response.data.user.ownedAssets.assetsList.ToList();

            foreach (var item in ownedAssets)
            {
                _ = await Indexer.GetMetadata(item.contractAddress, item.tokenId, indexerAPI, async (response) =>
               {
                   if (response.data.assetById != null)
                   {
                       Debug.Log("ITEM URI " + item.tokenUri);
                       await MetadataUtils.LoadTextureFromWeb(response.data.assetById.externalUrl, (image) =>
                       {
                           var gridItem = Instantiate(GridItemPrefab.gameObject, GridRoot).GetComponent<NFTGridItem>();
                           loadedItems.Add(gridItem);
                           gridItem.Initialize(item, image, SetMyNFTOnSale);
                           gridItem.EnableSecondaryButton(item, "Auction", SetMyNFTOnAuction);
                       });
                   }
               });
            }
        });

    }

    private async void LoadMarketOrders(string marketplaceAddress, OrderType orderType, MarketplaceType marketplaceType)
    {
        ClearGrid();

        var propName = orderType == OrderType.SellOrder ? "sell_orders" : "buy_orders";
        List<SellOrderInfo> sellOrders = new List<SellOrderInfo>();
        _ = await ZilliqaRPC.GetSmartContractSubState(AddressUtils.RemovePrefix_0x(marketplaceAddress), propName, async (response, error) =>
            {
                var forsale = response.result;
                
                sellOrders = await forsale[propName].GetInfo(orderType, marketplaceType);

                foreach (var info in sellOrders)
                {
                    _ = await Indexer.GetMetadata(info.tokenAddress, info.tokenId, indexerAPI, async (response) =>
                    {
                        if (response.data.assetById != null)
                        {
                            info.tokenInfo = response.data.assetById;
                            await MetadataUtils.LoadTextureFromWeb(response.data.assetById.externalUrl, (image) =>
                            {
                                info.tokenImage = image;

                                var gridItem = Instantiate(GridItemPrefab.gameObject, GridRoot).GetComponent<NFTGridItem>();
                                loadedItems.Add(gridItem);
                                string buttonLabel = marketplaceType == MarketplaceType.FixedPrice ? "Buy" : "Bid";
                                Action<SellOrderInfo,Action> onButtonClick = marketplaceType == MarketplaceType.FixedPrice ? BuyNFT : (Action<SellOrderInfo, Action>)BidOnNFT;
                                gridItem.Initialize(info, buttonLabel, onButtonClick);
                            });
                        }

                    });

                    Debug.Log("payment info " + info.tokenAddress + " " + info.marketplaceAddress + " " + info.price + " " + info.paymentMethod + " " + info.seller + " " + info.expirationBlockNumber);
                }
            });
    }

    private void BuyNFT(SellOrderInfo order, Action onBuy = null)
    {
        _ = ZilliqaRPC.FulfillFixedPriceOrder(
            wallet, 
            order.tokenId, 
            order.price, 
            order.type, 
            order.tokenAddress, 
            order.marketplaceAddress, 
            gasLimit, 
            onBuy);
    }

    private void BidOnNFT(SellOrderInfo order, Action onBid = null)
    {
        _ = ZilliqaRPC.BidOnAuctionOrder(
            wallet,
            order.tokenId,
            order.price + 2000,
            order.tokenAddress,
            order.marketplaceAddress,
            gasLimit,
            onBid);
    }

    private void SetMyNFTOnSale(NFTAsset nft)
    {
        _ = ZilliqaRPC.CreateFixedPriceOrder(
            wallet, 
            nft.tokenId, 
            "10000", 
            OrderType.SellOrder, 
            nft.contractAddress, 
            TestWallets.FixedPriceSmartContract0, 
            gasLimit);
    }

    private void SetMyNFTOnAuction(NFTAsset nft)
    {
        _ = ZilliqaRPC.CreateAuctionOrder(
            wallet,
            nft.tokenId,
            "10000",
            nft.contractAddress,
            TestWallets.AuctionSmartContract0,
            10f,
            gasLimit);
    }
}

[Serializable]
public class WalletSellOrders : Dictionary<string, SellOrder> { }

[Serializable]
public class SellOrder : Dictionary<string, JObject>
{

    public async Task<List<SellOrderInfo>> GetInfo(OrderType orderType, MarketplaceType marketplaceType)
    {
        switch (marketplaceType)
        {
            case MarketplaceType.FixedPrice:
                return await GetFixedPriceInfo(orderType);
            case MarketplaceType.Auction:
                return await GetAuctionOrders(orderType);
            default:
                return new List<SellOrderInfo>();
        }
    }

    private async Task<List<SellOrderInfo>> GetFixedPriceInfo(OrderType orderType)
    {
        var lastTxBlock = await ZilliqaRPC.GetLatestTxBlock();

        var infos = new List<SellOrderInfo>();
        foreach (var item in Keys)
        {
            var props = this[item].Properties().ToList();
            foreach (var tokenId in props)
            {
                var paymentInfo = (JObject)this[item][tokenId.Name];
                var priceDepth = (JObject)paymentInfo[paymentInfo.Properties().First().Name];
                var arguments = (JObject)priceDepth[priceDepth.Properties().ToList().First().Name];

                var info = new SellOrderInfo();
                info.tokenAddress = item;
                info.tokenId = tokenId.Name;
                info.paymentMethod = paymentInfo.Properties().FirstOrDefault().Name;
                info.price = priceDepth.Properties().ToList().FirstOrDefault().Name;
                info.marketplaceAddress = arguments["constructor"].ToString().Replace(".Order", "");
                info.type = orderType;
                info.marketplaceType = MarketplaceType.FixedPrice;
                if (arguments["arguments"] != null)
                {
                    info.seller = arguments["arguments"][0].ToString();
                    info.expirationBlockNumber = arguments["arguments"][1].ToString();

                    info.state = long.Parse(lastTxBlock.result.header.BlockNum) > long.Parse(info.expirationBlockNumber) ? OrderState.Expired : OrderState.Active;
                }
                else
                {
                    info.state = OrderState.Ended;
                }

                infos.Add(info);
            }
        }

        return infos;
    }

    private async Task<List<SellOrderInfo>> GetAuctionOrders(OrderType orderType)
    {
        var lastTxBlock = await ZilliqaRPC.GetLatestTxBlock();

        var infos = new List<SellOrderInfo>();
        foreach (var item in Keys)
        {
            var props = this[item].Properties().ToList();
            foreach (var tokenId in props)
            {
                var arguments = (JObject)this[item][tokenId.Name];

                var info = new SellOrderInfo();
                info.tokenAddress = item;
                info.tokenId = tokenId.Name;
                info.marketplaceAddress = arguments["constructor"].ToString().Replace(".BuyOrder", "").Replace(".SellOrder", "");
                info.type = orderType;
                info.marketplaceType = MarketplaceType.Auction;
                if (arguments["arguments"] != null)
                {
                    if (orderType == OrderType.SellOrder)
                    {
                        info.seller = arguments["arguments"][0].ToString();
                        info.expirationBlockNumber = arguments["arguments"][1].ToString();
                        info.paymentMethod = arguments["arguments"][2].ToString();
                        info.price = arguments["arguments"][3].ToString();
                        info.minimumBidIncrement = arguments["arguments"][5].ToString();
                        info.comissionFees = arguments["arguments"][7].ToString();
                    }
                    else
                    {
                        info.seller = arguments["arguments"][0].ToString();
                        info.price = arguments["arguments"][1].ToString();
                    }
                    
                    info.state = long.Parse(lastTxBlock.result.header.BlockNum) > long.Parse(info.expirationBlockNumber) ? OrderState.Expired : OrderState.Active;
                }
                else
                {
                    info.state = OrderState.Ended;
                }

                infos.Add(info);
            }
        }

        return infos;
    }
}

[Serializable]
public class SellOrderInfo
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


