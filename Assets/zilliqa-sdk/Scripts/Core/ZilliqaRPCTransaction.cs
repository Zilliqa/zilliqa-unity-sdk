using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zilliqa.Utils;
using Zilliqa.Core.Crypto;
using Zilliqa.Requests;
using System.Threading;

namespace Zilliqa.Core
{
    public delegate void NotificationEvent(INotification txStatus);
    public interface INotification
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TransactionModificationState Status { get; set; }
    }

    public interface IStaticNotifier
    {
        public static NotificationEvent OnNotification { get; set; }
    }

    public interface INotifier
    {
        public NotificationEvent OnNotification { get; set; }
    }
    public partial class ZilliqaRPC : IStaticNotifier
    {
        public static NotificationEvent OnNotification { get; set; }

        /// <summary>
        /// Create a new Transaction object and send it to the network to be processed. 
        /// </summary>
        /// <param name="sender">The caller of the on-chain transition</param>
        /// <param name="transactionParam">The transition parameters</param>
        /// <param name="onProcessed">This callback is triggered after the transaction is fully processed and has reached its final state on-chain</param>
        /// <returns></returns>
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

            //Create the request with the given parameters
            ZilRequest createTxReq = new ZilRequest("CreateTransaction", new object[] { transactionParam });
            CreateTransactionResponse result = null;
            _ = await Post<CreateTransactionResponse>(createTxReq, (response, error) =>
            {
                if (response.result != null)
                {
                    result = response;
                    Debug.Log("Info: " + response.result.Info + "\n" + "Tx hash: " + "0x" + response.result.TranID);
                    Task.Run(async () => await ListenForTransactionStatusUpdate(response.result.TranID, onProcessed));
                }
                else if (error != null)
                {
                    Debug.Log("Error code: " + error.code + "\n" + "Message: " + error.message);
                }
            });

            return result;
        }

        /// <summary>
        /// Returns the minimum gas price for this DS epoch, measured in the smallest price unit Qa (or 10^-12 Zil) in Zilliqa. 
        /// This is represented as a String.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetMinimumGasPriceResponse> GetMinimumGasPrice(
            Action<GetMinimumGasPriceResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetMinimumGasPrice", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the number of validated transactions included in this DS epoch. This is represented as a String.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetNumTxnsDSEpochResponse> GetNumTxnsDSEpoch(
            Action<GetNumTxnsDSEpochResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetNumTxnsDSEpoch", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the number of validated transactions included in this Transaction epoch. This is represented as a String.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetNumTxnsDSEpochResponse> GetNumTxnsTxEpoch(
            Action<GetNumTxnsDSEpochResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetNumTxnsTxEpoch", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the most recent 100 transactions that are validated by the Zilliqa network.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetRecentTransactionsResponse> GetRecentTransactions(
           Action<GetRecentTransactionsResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetRecentTransactions", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the details of a specified Transaction.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetTransactionResponse> GetTransaction(
          Action<GetTransactionResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTransaction", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the validated transactions included within a specified final transaction block as an array of length i, where i is the number of shards plus the DS committee. 
        /// The transactions are grouped based on the group that processed the transaction. 
        /// The first element of the array refers to the first shard. The last element of the array at index, i, refers to the transactions processed by the DS Committee.
        /// </summary>
        /// <param name="txBlockNumber">Specifed TX block number to return.</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetTransactionForTxBlockResponse> GetTransactionsForTxBlock(
          string txBlockNumber,
          Action<GetTransactionForTxBlockResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTransactionsForTxBlock", new object[] { txBlockNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// <para>
        /// This API behaves similar to <see cref="GetTransactionsForTxBlock"/> except it returns the transactions in batches (or pages) of 2,500.
        /// </para>
        /// <para>
        /// The number of pages available is defined by the NumPages header value in the <see cref="GetTxBlock"/> or <see cref="GetLatestTxBlock"/> response. 
        /// Page indexing is zero-based and indicated in the request after the block number.
        /// </para>
        /// <para>
        /// For example, to retrieve all the transactions for a block with NumPages=3, one must call GetTransactionsForTxBlockEx three times with page number 0, 1, and 2.
        /// </para>
        /// <para>
        /// This API is available from Zilliqa V7.2.0 onwards.
        /// </para>
        /// </summary>
        /// <param name="txBlockNumber">Specifed TX block number to return. Example: "1002353"</param>
        /// <param name="pageNumber">Page number (zero-based). Example: "2"</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetTransactionsForTxBlockExResponse> GetTransactionsForTxBlockEx(
          string txBlockNumber,
          string pageNumber,
          Action<GetTransactionsForTxBlockExResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTransactionsForTxBlockEx", new object[] { txBlockNumber, pageNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the status of a specified transaction. 
        /// This API is available from Zilliqa V7.0.0 onwards and supports all transaction statuses (unconfirmed, confirmed, and rejected).
        /// </summary>
        /// <remarks>
        /// <para>
        /// The GetTransactionStatus API relies on an off-chain transaction tracking solution, which may be periodically cleaned. 
        /// We recommend to still primarily use GetTransaction for older (e.g., several days old) transactions, and only use GetTransactionStatus mainly for tracking the progress of ongoing transactions.
        /// </para>
        /// <para>
        /// The GetTransactionStatus API is available only on <see href="https://api.zilliqa.com/"/>. It is disabled for community-hosted or private-hosted seed nodes.
        /// </para>
        /// </remarks>
        /// <param name="transactionID"></param>
        /// <param name="txStatus"></param>
        /// <returns></returns>
        private static async Task<GetTransactionStatusResponse> GetTransactionStatus(
            string transactionID,
            Action<GetTransactionStatusPayload> txStatus = null)
        {
            GetTransactionStatusResponse result = null;
            ZilRequest getTxBlockListingReq = new ZilRequest("GetTransactionStatus", new object[] { transactionID });
            _ = await Post<GetTransactionStatusResponse>(getTxBlockListingReq, (response, error) =>
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

        /// <summary>
        /// Returns the validated transactions (in verbose form) included within a specified final transaction block.
        /// </summary>
        /// <param name="txBlockNumber">Specifed TX block number to return.</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetTxnBodiesForTxBlockResponse> GetTxnBodiesForTxBlock(
                  string txBlockNumber,
                  Action<GetTxnBodiesForTxBlockResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTransactionsForTxBlockEx", new object[] { txBlockNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// <para>
        /// This API behaves similar to <see cref="GetTxnBodiesForTxBlock"/> except it returns the transactions in batches (or pages) of 2,500.
        /// </para>
        /// <para>
        /// The number of pages available is defined by the NumPages header value in the <see cref="GetTxBlock"/> or <see cref="GetLatestTxBlock"/> response. 
        /// Page indexing is zero-based and indicated in the request after the block number.
        /// </para>
        /// <para>
        /// For example, to retrieve all the transactions for a block with NumPages=3, one must call GetTxnBodiesForTxBlockEx three times with page number 0, 1, and 2.
        /// </para>
        /// <para>
        /// This API is available from Zilliqa V7.2.0 onwards.
        /// </para>
        /// </summary>
        /// <param name="txBlockNumber">Specifed TX block number to return. Example: "1002353"</param>
        /// <param name="pageNumber">Page number (zero-based). Example: "2"</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetTxnBodiesForTxBlockExResponse> GetTxnBodiesForTxBlockEx(
          string txBlockNumber,
          string pageNumber,
          Action<GetTxnBodiesForTxBlockExResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTxnBodiesForTxBlockEx", new object[] { txBlockNumber, pageNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Calls <see cref="GetTransactionStatus"/> until the final state of the transaction has been reached.
        /// </summary>
        /// <param name="transactionID">The block hash to monitor.</param>
        /// <param name="onTransactionProcessed">This callback is triggered when the transaction has reached its final state.</param>
        /// <returns></returns>
        private static async Task ListenForTransactionStatusUpdate(string transactionID, Action<GetTransactionStatusPayload> onTransactionProcessed = null)
        {
            var notification = new Notification();

            int currentAttemptCount = 0;
            bool hasFinalState = false;
            while (!hasFinalState)
            {
                _ = await GetTransactionStatus(transactionID, (txStatus) =>
                {
                    hasFinalState = txStatus.HasFinalState;
                    if (txStatus.HasFinalState)
                    {
                        onTransactionProcessed?.Invoke(txStatus);
                    }

                    notification = TransactionStringifier.AsNotification(txStatus, notification);
                    

                    OnNotification?.Invoke(notification);
                    
                });
                currentAttemptCount++;
                Thread.Sleep(1000);
            }
        }

    }
}
