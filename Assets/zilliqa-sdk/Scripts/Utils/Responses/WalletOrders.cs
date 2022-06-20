using System.Collections.Generic;
using Zilliqa.Core;
using System.Linq;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;


namespace Zilliqa.Marketplace
{
    [Serializable]
    public class WalletOrders : Dictionary<string, Order> { }

    [Serializable]
    public class Order : Dictionary<string, JObject>
    {

        public async Task<List<OrderInfo>> GetInfo(OrderType orderType, MarketplaceType marketplaceType, Func<OrderInfo, bool> filter = null)
        {
            switch (marketplaceType)
            {
                case MarketplaceType.FixedPrice:
                    return await GetFixedPriceInfo(orderType, filter);
                case MarketplaceType.Auction:
                    return await GetAuctionOrders(orderType, filter);
                default:
                    return new List<OrderInfo>();
            }
        }

        private async Task<List<OrderInfo>> GetFixedPriceInfo(OrderType orderType, Func<OrderInfo, bool> filter)
        {
            var lastTxBlock = await ZilliqaRPC.GetLatestTxBlock();

            var infos = new List<OrderInfo>();
            foreach (var item in Keys)
            {
                var props = this[item].Properties().ToList();
                foreach (var tokenId in props)
                {
                    var paymentInfo = (JObject)this[item][tokenId.Name];
                    var priceDepth = (JObject)paymentInfo[paymentInfo.Properties().First().Name];

                    if (priceDepth.Properties().Count() == 0)
                        continue;

                    var arguments = (JObject)priceDepth[priceDepth.Properties().ToList().First().Name];

                    var info = new OrderInfo();
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

                    if (info.state != OrderState.Ended && (filter == null ||filter(info)))
                        infos.Add(info);
                }
            }

            return infos;
        }

        private async Task<List<OrderInfo>> GetAuctionOrders(OrderType orderType, Func<OrderInfo, bool> filter)
        {
            var lastTxBlock = await ZilliqaRPC.GetLatestTxBlock();

            var infos = new List<OrderInfo>();
            foreach (var item in Keys)
            {
                var props = this[item].Properties().ToList();
                foreach (var tokenId in props)
                {
                    var arguments = (JObject)this[item][tokenId.Name];

                    var info = new OrderInfo();
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

                        info.state = orderType == OrderType.SellOrder &&
                            long.Parse(lastTxBlock.result.header.BlockNum) > long.Parse(info.expirationBlockNumber) ?
                            OrderState.Expired : OrderState.Active;
                    }
                    else
                    {
                        info.state = OrderState.Ended;
                    }

                    if (info.state != OrderState.Ended && (filter == null || filter(info)))
                        infos.Add(info);
                }
            }

            return infos;
        }
    }
}