using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zilliqa.Utils;
using Zilliqa.Core.Crypto;
using Zilliqa.Requests;

namespace Zilliqa.Core
{
    public partial class ZilliqaRPC
    {
        #region SmartContracts
        /// <summary>
        /// Returns the state (or a part specified) of a smart contract address, represented in a JSON format.
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="variableName"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetSmartContractSubStateResponse> GetSmartContractSubState(
            string contractAddress,
            string variableName,
            Action<GetSmartContractSubStateResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest req = new ZilRequest("GetSmartContractSubState", new object[] { contractAddress, variableName, new object[] { } });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// <para>
        /// Returns a smart contract address of 20 bytes. This is represented as a String.
        /// </para>
        /// <para>
        /// NOTE: This only works for contract deployment transactions.
        /// </para>
        /// </summary>
        /// <param name="transactionID">A Transaction ID of 32 bytes. </param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetContractAddressFromTransactionIDResponse> GetContractAddressFromTransactionID(
            string transactionID,
            Action<GetContractAddressFromTransactionIDResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest req = new ZilRequest("GetContractAddressFromTransactionID", new object[] { transactionID });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the Scilla code associated with a smart contract address. This is represented as a String.
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetSmartContractCodeResponse> GetSmartContractCode(
            string contractAddress,
            Action<GetSmartContractCodeResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest req = new ZilRequest("GetSmartContractCode", new object[] { AddressUtils.RemovePrefix_0x(contractAddress) });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the initialization (immutable) parameters of a given smart contract, represented in a JSON format.
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetSmartContractInitResponse> GetSmartContractInit(
           string contractAddress,
           Action<GetSmartContractInitResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest req = new ZilRequest("GetSmartContractInit", new object[] { AddressUtils.RemovePrefix_0x(contractAddress) });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the list of smart contract addresses created by an User's account.
        /// </summary>
        /// <param name="creatorAddress"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetSmartContractsResponse> GetSmartContracts(
           string creatorAddress,
           Action<GetSmartContractsResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest req = new ZilRequest("GetSmartContracts", new object[] { AddressUtils.RemovePrefix_0x(creatorAddress) });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the state (mutable) variables of a smart contract address, represented in a JSON format.
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetSmartContractStateResponse> GetSmartContractState(
            string contractAddress,
            Action<GetSmartContractStateResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest req = new ZilRequest("GetSmartContractState", new object[] { AddressUtils.RemovePrefix_0x(contractAddress) });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// <para>
        /// Returns the state proof for the corresponding TxBlock for a smart contract. 
        /// Please note that this API only serve state proof for the last 50 TxBlock.
        /// </para>
        /// <para>
        /// This API is available from Zilliqa V8.1.0 onwards.
        /// </para>
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="variableHash"></param>
        /// <param name="txBlock"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetStateProofResponse> GetStateProof(
            string contractAddress,
            string variableHash,
            string txBlock,
            Action<GetStateProofResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest req = new ZilRequest("GetStateProof", new object[] { AddressUtils.RemovePrefix_0x(contractAddress), variableHash, txBlock });
            //Response is identical to GetTxBlock
            var result = await PostRequest(req, onComplete);

            return result;
        }
        #endregion
    }
}