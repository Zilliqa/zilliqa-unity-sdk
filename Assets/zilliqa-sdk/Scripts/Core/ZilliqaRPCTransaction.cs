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
        protected static async Task<CreateTransactionResponse> CreateTransaction(
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

        public static async Task<GetMinimumGasPriceResponse> GetMinimumGasPrice(
            Action<GetMinimumGasPriceResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetMinimumGasPrice", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetNumTxnsDSEpochResponse> GetNumTxnsDSEpoch(
            Action<GetNumTxnsDSEpochResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetNumTxnsDSEpoch", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetNumTxnsDSEpochResponse> GetNumTxnsTxEpoch(
            Action<GetNumTxnsDSEpochResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetNumTxnsTxEpoch", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetRecentTransactionsResponse> GetRecentTransactions(
           Action<GetRecentTransactionsResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetRecentTransactions", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetTransactionResponse> GetTransaction(
          Action<GetTransactionResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTransaction", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetTransactionForTxBlockResponse> GetTransactionsForTxBlock(
          string txBlockNumber,
          Action<GetTransactionForTxBlockResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTransactionsForTxBlock", new object[] { txBlockNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetTransactionsForTxBlockExResponse> GetTransactionsForTxBlockEx(
          string txBlockNumber,
          string pageNumber,
          Action<GetTransactionsForTxBlockExResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTransactionsForTxBlockEx", new object[] { txBlockNumber, pageNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        private static async Task<GetTransactionStatusResponse> GetTransactionStatus(
            string transactionID,
            Action<GetTransactionStatusPayload> txStatus = null)
        {
            GetTransactionStatusResponse result = null;
            ZilRequest getTxBlockListingReq = new ZilRequest("GetTransactionStatus", new object[] { transactionID });
            _ = await PostRequest<GetTransactionStatusResponse>(getTxBlockListingReq, (response, error) =>
            {
                if (response.result != null)
                {
                    result = response;
                    string debugStr = response.ToString();

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

        public static async Task<GetTxnBodiesForTxBlockResponse> GetTxnBodiesForTxBlock(
                  string txBlockNumber,
                  Action<GetTxnBodiesForTxBlockResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTransactionsForTxBlockEx", new object[] { txBlockNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetTxnBodiesForTxBlockExResponse> GetTxnBodiesForTxBlockEx(
          string txBlockNumber,
          string pageNumber,
          Action<GetTxnBodiesForTxBlockExResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTxnBodiesForTxBlockEx", new object[] { txBlockNumber, pageNumber });
            var result = await PostRequest(req, onComplete);

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

    }
}
