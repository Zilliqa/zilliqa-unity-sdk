using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Zilliqa.Utils;
using Zilliqa.Requests;

namespace Zilliqa.Core
{
    public class ZRC6 : ZilliqaRPC
    {
        #region TokenSmartContract
        public static async Task SetSpender(
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
                                    value = tokenId
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

        public static async Task Mint(
           Wallet sender,
           string to,
           string tokenUri,
           string tokenAddress,
           string gasLimit,
           Action<GetTransactionStatusPayload> onProcessed = null)
        {
            string data = JsonConvert.SerializeObject(new ContractTransactionParams()
            {
                _tag = "Mint",
                args = new ContractTransitionArg[]
                            {
                                new ContractTransitionArg()
                                {
                                    vname = "to",
                                    type = "ByStr20",
                                    value = AddressUtils.AddPrefix_0x(to)
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_uri",
                                    type = "String",
                                    value = tokenUri
                                }
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


        public static async Task TransferFrom(
           Wallet sender,
           string to,
           string tokenId,
           string tokenAddress,
           string gasLimit,
           Action<GetTransactionStatusPayload> onProcessed = null)
        {
            string data = JsonConvert.SerializeObject(new ContractTransactionParams()
            {
                _tag = "TransferFrom",
                args = new ContractTransitionArg[]
                            {
                                new ContractTransitionArg()
                                {
                                    vname = "to",
                                    type = "ByStr20",
                                    value = AddressUtils.AddPrefix_0x(to)
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_id",
                                    type = "Uint256",
                                    value = tokenId
                                }
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

        public static async Task Pause(
            Wallet sender,
            string tokenAddress,
            string gasLimit,
            Action<GetTransactionStatusPayload> onProcessed = null)
        {
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
                data = ""
            };

            _ = await GetBalance(sender, (response) => { transactionParam.nonce = response.result.nonce + 1; });

            _ = await CreateTransaction(sender, transactionParam, onProcessed: (response) =>
            {
                onProcessed?.Invoke(response);
            });
        }


        #endregion
    }
}