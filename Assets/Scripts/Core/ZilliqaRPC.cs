using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Org.BouncyCastle.Math;
using Zilliqa.Core;
using Zilliqa.Utils;
using Zilliqa.Core.Crypto;
using Zilliqa.Requests;

namespace Zilliqa.Core
{
    public class ZilliqaRPC
    {
        #region Properties
        private static string gasPrice = "2000000000";
        private static int version = 21823489;

        public static Wallet wallet;
        public static string apiUrl = "https://dev-api.zilliqa.com/";//"https://api.zilliqa.com/";
        #endregion

        #region Core
        private static async Task<CreateTransactionResponse> CreateTransaction(
           Wallet sender,
           Transaction transactionParam,
           Action<GetTransactionStatusPayload> onProcessed = null)
        {
            //Encode the protobuf object to byte array
            byte[] message = transactionParam.Encode();
            // sign the transaction based on the payload
            Signature signature = Schnorr.Sign(sender.ecKeyPair, message);
            transactionParam.signature = signature.ToString().ToLower();

            ZilRequest createTxReq = new ZilRequest("CreateTransaction", new object[] { transactionParam });
            CreateTransactionResponse result = null;
            _ = await PostRequest<CreateTransactionResponse>(createTxReq, (response, error) =>
            {
                if (response.result != null)
                {
                    result = response;
                    Debug.Log("Info: " + response.result.Info + "\n" + "Tx hash: " + "0x" + response.result.TranID);
                    _ = ListenForTransactionStatusUpdate(response.result.TranID, onProcessed);
                }
                else if (error != null)
                {
                    Debug.Log("Error code: " + error.code + "\n" + "Message: " + error.message);
                }
            });

            return result;
        }

        public static async Task<GetTxBlockResponse> GetLatestTxBlock()
        {
            ZilRequest getTxBlockReq = new ZilRequest("GetLatestTxBlock", new object[] { "" });
            //Response is identical to GetTxBlock
            var result = await PostRequest<GetTxBlockResponse>(getTxBlockReq);

            return result;
        }

        public static async Task<GetBalanceResponse> GetBalance(Wallet sender, Action<GetBalanceResponse> onResponse)
        {
            GetBalanceResponse res = null;
            if (string.IsNullOrEmpty(sender.Address))
                Debug.LogError("Error: Failed to auto increase nonce. Please input wallet address.");
            else
            {
                ZilRequest getBalanceReq = new ZilRequest("GetBalance", sender.Address);
                await PostRequest<GetBalanceResponse>(getBalanceReq, (response, error) =>
                {
                    if (response.result != null)
                    {
                        onResponse?.Invoke(response);
                        res = response;
                    }
                    else if (error != null)
                    {
                        Debug.Log("Error code: " + error.code + "\n" + "Message: " + error.message);
                    }
                });
            }

            return res;
        }

        public static async Task<GetTxBlockRateResponse> GetTxBlockRate()
        {
            ZilRequest getTxBlockReq = new ZilRequest("GetTxBlockRate", new object[] { "" });
            //Response is identical to GetTxBlock
            var result = await PostRequest<GetTxBlockRateResponse>(getTxBlockReq);

            return result;
        }

        private static async Task ListenForTransactionStatusUpdate(string transactionID, Action<GetTransactionStatusPayload> onTransactionProcessed = null)
        {
            int currentAttemptCount = 0;
            bool hasFinalState = false;
            while (!hasFinalState)
            {
                _ = await GetTransactionStatus(transactionID, (txStatus) =>
                {
                    hasFinalState = txStatus.HasFinalState;
                    if (txStatus.HasFinalState)
                        onTransactionProcessed?.Invoke(txStatus);
                });
                currentAttemptCount++;
            }
        }

        private static async Task<GetTransactionStatusResponse> GetTransactionStatus(string transactionID, Action<GetTransactionStatusPayload> txStatus = null)
        {
            GetTransactionStatusResponse result = null;
            ZilRequest getTxBlockListingReq = new ZilRequest("GetTransactionStatus", new object[] { transactionID });
            _ = await PostRequest<GetTransactionStatusResponse>(getTxBlockListingReq, (response, error) =>
            {
                if (response.result != null)
                {
                    result = response;
                    string debugStr = "{\n\t<b>Id</b>: " + response.result.Id + "\n" +
                        "\t<b>Oid</b>: " + response.result.Oid.ToString() + "\n" +
                        "\t<b>Amount</b>: " + response.result.Amount + "\n" +
                        "\t<b>Data</b>: " + response.result.Data + "\n" +
                        "\t<b>EpochInserted</b>: " + response.result.EpochInserted + "\n" +
                        "\t<b>EpochUpdated</b>: " + response.result.EpochUpdated + "\n" +
                        "\t<b>GasLimit</b>: " + response.result.GasLimit + "\n" +
                        "\t<b>GasPrice</b>: " + response.result.GasPrice + "\n" +
                        "\t<b>LastModified</b>: " + response.result.LastModified + "\n" +
                        "\t<b>ModificationState</b>: " + "<color=#39002e>" + response.result.ModificationState + "</color>\n" +
                        "\t<b>Nonce</b>: " + response.result.Nonce + "\n" +
                        "\t<b>SenderAddr</b>: " + response.result.SenderAddr + "\n" +
                        "\t<b>Signature</b>: " + response.result.Signature + "\n" +
                        "\t<b>Status</b>: " + "<color=#39002e>" + response.result.Status + "</color>\n" +
                        "\t<b>Success</b>: " + "<color=#39000d>" + response.result.Success + "</color>\n" +
                        "\t<b>ToAddr</b>: " + response.result.ToAddr + "\n" +
                        "\t<b>Version</b>: " + response.result.Version + "\n}";

                    Debug.Log("Get transaction: " + debugStr);
                    txStatus?.Invoke(response.result);
                }
                else if (error != null)
                {
                    Debug.Log("Error code: " + error.code + "\n" + "Message: " + error.message);
                }
            });

            return result;
        }

        protected static async Task<T> PostRequest<T>(ZilRequest request, Action<T, ZilResponse.Error> onComplete = null)
       where T : ZilResponse
        {
            string json = request.ToJson();
            Debug.Log(json);
            using UnityEngine.Networking.UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST");
            byte[] rawData = Encoding.UTF8.GetBytes(json);

            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.uploadHandler = new UploadHandlerRaw(rawData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            await webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    Debug.Log("response " + webRequest.downloadHandler.text);
                    var response = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                    onComplete?.Invoke(response, response.error);
                    return response;
                default:
                    return null;
            }
        }
        #endregion

        #region TokenSmartContract
        protected static async Task SetSpender(
           Wallet sender,
           string tokenId,
           string tokenAddress,
           string MarketplaceAddress,
           string gasLimit,
           Action<GetTransactionStatusPayload> onProcessed = null)
        {
            string data = JsonConvert.SerializeObject(new ContractTransactionParams()
            {
                _tag = "SetSpender",
                args = new ContractTransitionArg[]
                          {
                                new ContractTransitionArg()
                                {
                                    vname = "spender",
                                    type = "ByStr20",
                                    value = MarketplaceAddress
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_id",
                                    type = "Uint256",
                                    value = "" + tokenId
                                },
                          }

            });

            Transaction transactionParam = new Transaction()
            {
                version = version,
                nonce = 0,
                // the contract address needs to be checksummed
                toAddr = AddressUtils.ToCheckSumAddress(tokenAddress),
                amount = "0",
                pubKey = sender.PublicKey,
                gasPrice = gasPrice,
                gasLimit = gasLimit,
                code = "",
                priority = false,
                data = data
            };

            _ = await GetBalance(sender, (response) => { transactionParam.nonce = response.result.nonce + 1; });

            _ = await CreateTransaction(sender, transactionParam, onProcessed: (response) =>
            {
                onProcessed?.Invoke(response);
            });
        }
        #endregion

        #region FixedPriceMarketplace
        public static async Task CreateFixedPriceOrder(
           Wallet sender,
           string tokenId,
           string salePrice,
           OrderType orderType,
           string tokenAddress,
           string MarketplaceAddress,
           string gasLimit,
           Action onComplete = null)
        {
            //data needs to be a stringified json, more info on the structure can be found here:
            //https://dev.zilliqa.com/docs/apis/api-transaction-create-tx
            string data = JsonConvert.SerializeObject(new ContractTransactionParams()
            {
                _tag = "SetOrder",
                args = new ContractTransitionArg[]
                                {
                                new ContractTransitionArg()
                                {
                                    vname = "token_address",
                                    type = "ByStr20",
                                    value = AddressUtils.ToCheckSumAddress(tokenAddress)
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_id",
                                    type = "Uint256",
                                    value = "" + tokenId
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "payment_token_address",
                                    type = "ByStr20",
                                    value = "0x0000000000000000000000000000000000000000"
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "sale_price",
                                    type = "Uint128",
                                    value = "" + salePrice
                                },
                                 new ContractTransitionArg()
                                {
                                    vname = "side",
                                    type = "Uint32",
                                    value = "" + (int)orderType
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "expiration_bnum",
                                    type = "BNum",
                                    value = "5240908"
                                }
                                }

            });

            Transaction transactionParam = new Transaction()
            {
                version = version,
                nonce = 0,
                // the contract address needs to be checksummed
                toAddr = AddressUtils.ToCheckSumAddress(MarketplaceAddress),
                amount = "" + salePrice,
                pubKey = sender.PublicKey,
                gasPrice = gasPrice,
                gasLimit = gasLimit,
                code = "",
                priority = false,
                data = data
            };

            await SetSpender(sender, tokenId, tokenAddress, MarketplaceAddress, gasLimit, async (txStatus) =>
            {
                // GetBalance rpc is being called to get nonce counter if autoNonce is used
                _ = await GetBalance(sender, (response) => { transactionParam.nonce = response.result.nonce + 1; });
                _ = CreateTransaction(sender, transactionParam, onProcessed: (response) => { onComplete?.Invoke(); });
            });
        }

        public static async Task FulfillFixedPriceOrder(
            Wallet sender,
            string tokenId,
            string salePrice,
            OrderType orderType,
            string tokenAddress,
            string MarketplaceAddress,
            string gasLimit,
            Action onComplete = null)
        {
            //data needs to be a stringified json, more info on the structure can be found here:
            //https://dev.zilliqa.com/docs/apis/api-transaction-create-tx
            string data = JsonConvert.SerializeObject(new ContractTransactionParams()
            {
                _tag = "FulfillOrder",
                args = new ContractTransitionArg[]
                                {
                                new ContractTransitionArg()
                                {
                                    vname = "token_address",
                                    type = "ByStr20",
                                    value = AddressUtils.ToCheckSumAddress(tokenAddress)
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_id",
                                    type = "Uint256",
                                    value = "" + tokenId
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "payment_token_address",
                                    type = "ByStr20",
                                    value = "0x0000000000000000000000000000000000000000"
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "sale_price",
                                    type = "Uint128",
                                    value = "" + salePrice
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "side",
                                    type = "Uint32",
                                    value = "" + (int)orderType
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "dest",
                                    type = "ByStr20",
                                    value = "0x" + sender.Address
                                }

                                }
            });

            Transaction transactionParam = new Transaction()
            {
                version = version,
                nonce = 0,
                // the contract address needs to be checksummed
                toAddr = AddressUtils.ToCheckSumAddress(MarketplaceAddress),
                amount = "" + salePrice,
                pubKey = sender.PublicKey,
                gasPrice = gasPrice,
                gasLimit = gasLimit,
                code = "",
                priority = false,
                data = data
            };

            // GetBalance rpc is being called to get nonce counter if autoNonce is used
            _ = await GetBalance(sender, (response) => { transactionParam.nonce = response.result.nonce + 1; });
            _ = CreateTransaction(sender, transactionParam, onProcessed: (response) => { onComplete?.Invoke(); });
        }

        public static async Task CancelFixedPriceOrder(
            Wallet sender,
            string tokenId,
            string salePrice,
            OrderType orderType,
            string tokenAddress,
            string MarketplaceAddress,
            string gasLimit,
            Action onComplete = null)
        {
            string data = JsonConvert.SerializeObject(new ContractTransactionParams()
            {
                _tag = "CancelOrder",
                args = new ContractTransitionArg[]
                            {
                                new ContractTransitionArg()
                                {
                                    vname = "token_address",
                                    type = "ByStr20",
                                    value = AddressUtils.ToCheckSumAddress(tokenAddress)
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_id",
                                    type = "Uint256",
                                    value = "" + tokenId
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "payment_token_address",
                                    type = "ByStr20",
                                    value = "0x0000000000000000000000000000000000000000"
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "sale_price",
                                    type = "Uint128",
                                    value = "" + salePrice
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "side",
                                    type = "Uint32",
                                    value = "" + (int)orderType
                                },
                            }

            });

            Transaction transactionParam = new Transaction()
            {
                version = version,
                nonce = 0,
                // the contract address needs to be checksummed
                toAddr = AddressUtils.ToCheckSumAddress(MarketplaceAddress),
                amount = "" + salePrice,
                pubKey = sender.PublicKey,
                gasPrice = gasPrice,
                gasLimit = gasLimit,
                code = "",
                priority = false,
                data = data
            };

            // GetBalance rpc is being called to get nonce counter if autoNonce is used
            _ = await GetBalance(sender, (response) => { transactionParam.nonce = response.result.nonce + 1; });
            _ = CreateTransaction(sender, transactionParam, onProcessed: (response) => { onComplete?.Invoke(); });
        }
        #endregion

        #region AuctionMarketplace
        /// <summary>
        /// A seller can put an asset on auction by calling the Start transition.
        /// Conditions:
        /// The token owner or the spender should be the one calling the Start transition of the English Auction contract, otherwise the transition will fail.
        /// There should be no ongoing auction on the asset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="tokenId"></param>
        /// <param name="startingAuctionPrice"></param>
        /// <param name="tokenAddress"></param>
        /// <param name="MarketplaceAddress"></param>
        /// <param name="auctionDurationInHours"></param>
        /// <param name="gasLimit"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task CreateAuctionOrder(
            Wallet sender,
            string tokenId,
            string startingAuctionPrice,
            string tokenAddress,
            string MarketplaceAddress,
            float auctionDurationInHours,
            string gasLimit,
            Action onComplete = null)
        {
            var expirationBlockNum = await CryptoUtil.HoursToBlockNumber(auctionDurationInHours);

            string data = JsonConvert.SerializeObject(new ContractTransactionParams()
            {
                _tag = "Start",
                args = new ContractTransitionArg[]
                            {
                                new ContractTransitionArg()
                                {
                                    vname = "token_address",
                                    type = "ByStr20",
                                    value = AddressUtils.ToCheckSumAddress(tokenAddress)
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_id",
                                    type = "Uint256",
                                    value = "" + tokenId
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "payment_token_address",
                                    type = "ByStr20",
                                    value = "0x0000000000000000000000000000000000000000"
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "start_amount",
                                    type = "Uint128",
                                    value = "" + startingAuctionPrice
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "expiration_bnum",
                                    type = "BNum",
                                    value = "" + expirationBlockNum
                                }
                            }
            });

            Transaction transactionParam = new Transaction()
            {
                version = version,
                nonce = 0,
                // the contract address needs to be checksummed
                toAddr = AddressUtils.ToCheckSumAddress(MarketplaceAddress),
                amount = "0",
                pubKey = sender.PublicKey,
                gasPrice = gasPrice,
                gasLimit = gasLimit,
                code = "",
                priority = false,
                data = data
            };
            await SetSpender(sender, tokenId, tokenAddress, MarketplaceAddress, gasLimit, async (txStatus) =>
            {
                // GetBalance rpc is being called to get nonce counter if autoNonce is used
                _ = await GetBalance(sender, (response) => { transactionParam.nonce = response.result.nonce + 1; });
                _ = CreateTransaction(sender, transactionParam, onProcessed: (response) => { onComplete?.Invoke(); });
            });
        }

        /// <summary>
        /// A buyer can place a bid on the asset by calling the Bid transition on the English Auction marketplace. 
        /// The amount will be deducted from the bidders wallet once a successful bid is placed.
        /// Conditions: 
        /// 1- There should be on going auction on the asset | 
        /// 2- The auction on the item must not be expired |
        /// 3- The bid increment amount should be the minimum rate set, see bid_increment_bps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="tokenId"></param>
        /// <param name="bidPrice"></param>
        /// <param name="tokenAddress"></param>
        /// <param name="MarketplaceAddress"></param>
        /// <param name="gasLimit"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task BidOnAuctionOrder(
            Wallet sender,
            string tokenId,
            string bidPrice,
            string tokenAddress,
            string MarketplaceAddress,
            string gasLimit,
            Action onComplete = null)
        {

            string data = JsonConvert.SerializeObject(new ContractTransactionParams()
            {
                _tag = "Bid",
                args = new ContractTransitionArg[]
                    {
                                new ContractTransitionArg()
                                {
                                    vname = "token_address",
                                    type = "ByStr20",
                                    value = AddressUtils.ToCheckSumAddress(tokenAddress)
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_id",
                                    type = "Uint256",
                                    value = "" + tokenId
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "amount",
                                    type = "Uint128",
                                    value = "" + bidPrice
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "dest",
                                    type = "ByStr20",
                                    value = "0x" + sender.Address
                                },
                    }

            });

            Transaction transactionParam = new Transaction()
            {
                version = version,
                nonce = 0,
                // the contract address needs to be checksummed
                toAddr = AddressUtils.ToCheckSumAddress(MarketplaceAddress),
                amount = "" + bidPrice,
                pubKey = sender.PublicKey,
                gasPrice = gasPrice,
                gasLimit = gasLimit,
                code = "",
                priority = false,
                data = data
            };

            // GetBalance rpc is being called to get nonce counter if autoNonce is used
            _ = await GetBalance(sender, (response) => { transactionParam.nonce = response.result.nonce + 1; });
            _ = CreateTransaction(sender, transactionParam, onProcessed: (response) => { onComplete?.Invoke(); });

        }

        /// <summary>
        /// A seller can cancel an auction listing by calling the Canceltransition on the English Auction contract. 
        /// Note that this can only be called as long as the auction has not expired.
        /// When an auction is cancelled, all the buy orders are added to the payment token balance. 
        /// The bidders should withdraw their funds via WithdrawPaymentTokens, see Withdraw section below.
        /// </summary>
        /// <returns></returns>
        public static async Task CancelAuctionOrder(
            Wallet sender,
            string tokenId,
            string tokenAddress,
            string MarketplaceAddress,
            string gasLimit,
            Action onComplete = null)
        {
            string data = JsonConvert.SerializeObject(new ContractTransactionParams()
            {
                _tag = "Cancel",
                args = new ContractTransitionArg[]
                            {
                                new ContractTransitionArg()
                                {
                                    vname = "token_address",
                                    type = "ByStr20",
                                    value = AddressUtils.ToCheckSumAddress(tokenAddress)
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_id",
                                    type = "Uint256",
                                    value = "" + tokenId
                                }
                         }
            });

            Transaction transactionParam = new Transaction()
            {
                version = version,
                nonce = 0,
                // the contract address needs to be checksummed
                toAddr = AddressUtils.ToCheckSumAddress(MarketplaceAddress),
                amount = "0",
                pubKey = sender.PublicKey,
                gasPrice = gasPrice,
                gasLimit = gasLimit,
                code = "",
                priority = false,
                data = data
            };

            _ = await GetBalance(sender, (response) => { transactionParam.nonce = response.result.nonce + 1; });
            _ = CreateTransaction(sender, transactionParam, onProcessed: (response) => { onComplete?.Invoke(); });
        }

        public async Task EndAuctionOrder(
            Wallet sender,
            string tokenId,
            string bidPrice,
            string tokenAddress,
            string MarketplaceAddress,
            string gasLimit,
            Action onComplete = null)
        {
            string data = JsonConvert.SerializeObject(new ContractTransactionParams()
            {
                _tag = "End",
                args = new ContractTransitionArg[]
                            {
                                new ContractTransitionArg()
                                {
                                    vname = "token_address",
                                    type = "ByStr20",
                                    value = AddressUtils.ToCheckSumAddress(tokenAddress)
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_id",
                                    type = "Uint256",
                                    value = "" + tokenId
                                }
                         }
            });

            Transaction transactionParam = new Transaction()
            {
                version = version,
                nonce = 0,
                // the contract address needs to be checksummed
                toAddr = AddressUtils.ToCheckSumAddress(MarketplaceAddress),
                amount = "" + bidPrice,
                pubKey = sender.PublicKey,
                gasPrice = gasPrice,
                gasLimit = gasLimit,
                code = "",
                priority = false,
                data = data
            };

            // GetBalance rpc is being called to get nonce counter if autoNonce is used
            _ = await GetBalance(sender, (response) => { transactionParam.nonce = response.result.nonce + 1; });
            _ = CreateTransaction(sender, transactionParam, onProcessed: (response) => { onComplete?.Invoke(); });
        }

        public async Task WithdrawFunds(
            Wallet sender,
            string MarketplaceAddress,
            string gasLimit,
            Action onComplete = null)
        {
            string data = JsonConvert.SerializeObject(new ContractTransactionParams()
            {
                _tag = "WithdrawPaymentTokens",
                args = new ContractTransitionArg[]
                    {
                        new ContractTransitionArg()
                        {
                            vname = "payment_token_address",
                            type = "ByStr20",
                            value = "0x0000000000000000000000000000000000000000"
                        }
                    }
            });

            Transaction transactionParam = new Transaction()
            {
                version = version,
                nonce = 0,
                // the contract address needs to be checksummed
                toAddr = AddressUtils.ToCheckSumAddress(MarketplaceAddress),
                amount = "0",
                pubKey = sender.PublicKey,
                gasPrice = gasPrice,
                gasLimit = gasLimit,
                code = "",
                priority = false,
                data = data
            };

            // GetBalance rpc is being called to get nonce counter if autoNonce is used
            _ = await GetBalance(sender, (response) => { transactionParam.nonce = response.result.nonce + 1; });
            _ = CreateTransaction(sender, transactionParam, onProcessed: (response) => { onComplete?.Invoke(); });
        }

        public static async Task RedeemNFT(
            Wallet sender,
            string tokenId,
            string bidPrice,
            string tokenAddress,
            string MarketplaceAddress,
            string gasLimit,
            Action onComplete = null)
        {
            string data = JsonConvert.SerializeObject(new ContractTransactionParams()
            {
                _tag = "WithdrawAsset",
                args = new ContractTransitionArg[]
                            {
                                new ContractTransitionArg()
                                {
                                    vname = "token_address",
                                    type = "ByStr20",
                                    value = AddressUtils.ToCheckSumAddress(tokenAddress)
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_id",
                                    type = "Uint256",
                                    value = "" + tokenId
                                }
                         }
            });

            Transaction transactionParam = new Transaction()
            {
                version = version,
                nonce = 0,
                // the contract address needs to be checksummed
                toAddr = AddressUtils.ToCheckSumAddress(MarketplaceAddress),
                amount = "" + bidPrice,
                pubKey = sender.PublicKey,
                gasPrice = gasPrice,
                gasLimit = gasLimit,
                code = "",
                priority = false,
                data = data
            };

            // GetBalance rpc is being called to get nonce counter if autoNonce is used
            _ = await GetBalance(sender, (response) => { transactionParam.nonce = response.result.nonce + 1; });
            _ = CreateTransaction(sender, transactionParam, onProcessed: (response) => { onComplete?.Invoke(); });
        }
        #endregion

        #region SmartContracts
        public static async Task<GetSmartContractSubStateResponse> GetSmartContractSubState(string contractAddress, string variableName, Action<GetSmartContractSubStateResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest getTxBlockReq = new ZilRequest("GetSmartContractSubState", new object[] { contractAddress, variableName, new object[] { } });
            //Response is identical to GetTxBlock
            var result = await PostRequest<GetSmartContractSubStateResponse>(getTxBlockReq, onComplete);

            return result;
        }
        #endregion
    }
}