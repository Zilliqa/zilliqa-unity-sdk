using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zilliqa.Core;
using Zilliqa.Utils;
using System.Linq;
using System;
using GraphQlClient.Core;
using Zilliqa.Requests;
using UnityEngine.UI;
using Zilliqa.Marketplace;

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
        WalletAddressText.text = AddressUtils.AddPrefix_0x(wallet.Address);
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
        List<OrderInfo> sellOrders = new List<OrderInfo>();
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
                                Action<OrderInfo,Action> onButtonClick = marketplaceType == MarketplaceType.FixedPrice ? BuyNFT : (Action<OrderInfo, Action>)BidOnNFT;
                                gridItem.Initialize(info, buttonLabel, onButtonClick);
                            });
                        }

                    });

                    Debug.Log("payment info " + info.tokenAddress + " " + info.marketplaceAddress + " " + info.price + " " + info.paymentMethod + " " + info.seller + " " + info.expirationBlockNumber);
                }
            });
    }

    private void BuyNFT(OrderInfo order, Action onBuy = null)
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

    private void BidOnNFT(OrderInfo order, Action onBid = null)
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


