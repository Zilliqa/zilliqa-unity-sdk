using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Zilliqa.Utils;
using Zilliqa.Requests;

namespace Zilliqa.Core
{
    /// <summary>
    /// This class handles RPC calls to ZRC-6 smart contracts.
    /// <remarks>
    /// Note that not all ZRC-6 RPC calls are currently present in this class.
    /// </remarks>
    /// </summary>
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
            //We build the parameters for the smart contract transition
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

            //We build the parameters for CreateTransaction
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

            //We call GetBalance to get the current nonce for the caller
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
            //We build the parameters for the smart contract transition
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

            //We build the parameters for CreateTransaction
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

            //We call GetBalance to get the current nonce for the caller
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
            //We build the parameters for the smart contract transition
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

            //We build the parameters for CreateTransaction
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

            //We call GetBalance to get the current nonce for the caller
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
            //We build the parameters for the smart contract transition
            string data = JsonConvert.SerializeObject(new ContractTransactionParams()
            {
                _tag = "Pause",
                args = new ContractTransitionArg[0]
            });

            //We build the parameters for CreateTransaction
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

            //We call GetBalance to get the current nonce for the caller
            _ = await GetBalance(sender, (response) => { transactionParam.nonce = response.result.nonce + 1; });

            _ = await CreateTransaction(sender, transactionParam, onProcessed: (response) =>
            {
                onProcessed?.Invoke(response);
            });
        }


        #endregion
    }
}