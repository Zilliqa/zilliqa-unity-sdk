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
        public static async Task<GetSmartContractSubStateResponse> GetSmartContractSubState(
            string contractAddress,
            string variableName,
            Action<GetSmartContractSubStateResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest req = new ZilRequest("GetSmartContractSubState", new object[] { contractAddress, variableName, new object[] { } });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetContractAddressFromTransactionIDResponse> GetContractAddressFromTransactionID(
            string transactionID,
            Action<GetContractAddressFromTransactionIDResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest req = new ZilRequest("GetContractAddressFromTransactionID", new object[] { transactionID });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetSmartContractCodeResponse> GetSmartContractCode(
            string contractAddress,
            Action<GetSmartContractCodeResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest req = new ZilRequest("GetSmartContractCode", new object[] { AddressUtils.RemovePrefix_0x(contractAddress) });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetSmartContractInitResponse> GetSmartContractInit(
           string contractAddress,
           Action<GetSmartContractInitResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest req = new ZilRequest("GetSmartContractInit", new object[] { AddressUtils.RemovePrefix_0x(contractAddress) });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetSmartContractsResponse> GetSmartContracts(
           string creatorAddress,
           Action<GetSmartContractsResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest req = new ZilRequest("GetSmartContracts", new object[] { AddressUtils.RemovePrefix_0x(creatorAddress) });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetSmartContractStateResponse> GetSmartContractState(
            string contractAddress,
            Action<GetSmartContractStateResponse, ZilResponse.Error> onComplete)
        {
            ZilRequest req = new ZilRequest("GetSmartContractState", new object[] { AddressUtils.RemovePrefix_0x(contractAddress) });
            var result = await PostRequest(req, onComplete);

            return result;
        }

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