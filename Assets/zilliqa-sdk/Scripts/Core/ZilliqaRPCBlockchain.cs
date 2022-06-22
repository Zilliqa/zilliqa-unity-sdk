using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zilliqa.Utils;
using Zilliqa.Requests;

namespace Zilliqa.Core
{
    public partial class ZilliqaRPC
    {
        #region Properties
        protected static string gasPrice = "2000000000";
        protected static int version = 21823489;

        //The blockchain branch that we wish to perform our RPCs on.
        public static string apiUrl = "https://dev-api.zilliqa.com/";//"https://api.zilliqa.com/";
        #endregion

        #region Blockchain
        /// <summary>
        /// Returns the details of the most recent Transaction block.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetTxBlockResponse> GetLatestTxBlock(Action<GetTxBlockResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetLatestTxBlock", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns a paginated list of up to 10 Directory Service (DS) blocks and their block hashes for a specified page. 
        /// The maxPages variable that specifies the maximum number of pages available is also returned.
        /// </summary>
        /// <param name="pageNumber">Specifed page of DS blocks listing to return.</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetDSBlockListingResponse> GetDSBlockListing(int pageNumber, Action<GetDSBlockListingResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("DSBlockListing", new object[] { pageNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the current network statistics for the specified network.
        ///NOTE: CHAIN_ID from 2 to 9 are reserved for Zilliqa Core use.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetBlockchainInfoResponse> GetBlockchainInfo(Action<GetBlockchainInfoResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetBlockchainInfo", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the current number of DS blocks in the network. This is represented as a String.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetCurrentDSEpochResponse> GetCurrentDSEpoch(Action<GetCurrentDSEpochResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetCurrentDSEpoch", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the current TX block number of the network. This is represented as a String.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetCurrentMiniEpochResponse> GetCurrentMiniEpoch(Action<GetCurrentMiniEpochResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetCurrentMiniEpoch", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the details of a specified Directory Service block.
        /// </summary>
        /// <param name="DSBlockNumber">Specified DS block number to return.</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetDSBlockResponse> GetDsBlock(string DSBlockNumber, Action<GetDSBlockResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetDsBlock", new object[] { DSBlockNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the current Directory Service blockrate per second.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetDSBlockRateResponse> GetDSBlockRate(Action<GetDSBlockRateResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetDSBlockRate", new object[] { });
            var result = await PostRequest(req, onComplete); ;

            return result;
        }

        /// <summary>
        /// Returns the details of the most recent Directory Service block.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetLatestDSBlockResponse> GetLatestDsBlock(Action<GetLatestDSBlockResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetLatestDsBlock", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the mining nodes (i.e., the members of the DS committee and shards) at the specified DS block. 
        /// </summary>
        /// <param name="DSBlockNumber"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetMinerInfoResponse> GetMinerInfo(string DSBlockNumber, Action<GetMinerInfoResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetMinerInfo", new object[] { DSBlockNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetNetworkIdResponse> GetNetworkId(Action<GetNetworkIdResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetNetworkId", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetNumDSBlocksResponse> GetNumDSBlocks(Action<GetNumDSBlocksResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetNumDSBlocks", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the current number of validated Transactions in the network. This is represented as a String.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetNumTxBlocksResponse> GetNumTransactions(Action<GetNumTxBlocksResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetNumTransactions", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the current number of Transaction blocks in the network. This is represented as a String.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetNumTxBlocksResponse> GetNumTxBlocks(Action<GetNumTxBlocksResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetNumTxBlocks", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the minimum shard difficulty of the previous block. This is represented as an Number.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetDSDifficultyResponse> GetPrevDifficulty(Action<GetDSDifficultyResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetPrevDifficulty", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the minimum DS difficulty of the previous block. This is represented as an Number.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetDSDifficultyResponse> GetPrevDSDifficulty(Action<GetDSDifficultyResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetPrevDifficulty", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the total supply (ZIL) of coins in the network. This is represented as a String.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetTotalCoinSupplyResponse> GetTotalCoinSupply(Action<GetTotalCoinSupplyResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTotalCoinSupply", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the current Transaction rate per second (TPS) of the network. This is represented as an Number.
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetTxBlockRateResponse> GetTransactionRate(Action<GetTxBlockRateResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTransactionRate", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the details of a specified Transaction block.
        /// </summary>
        /// <param name="transactionBlock">Specifed TX block number to return. Example: "1002353"</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetTxBlockResponse> GetTxBlock(string transactionBlock, Action<GetTxBlockResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTxBlock", new object[] { transactionBlock });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns a paginated list of up to 10 Transaction blocks and their block hashes for a specified page. 
        /// The maxPages variable that specifies the maximum number of pages available is also returned.
        /// </summary>
        /// <param name="pageNumber">Specifed page of TX blocks listing to return.</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static async Task<GetTxBlockListingResponse> TxBlockListing(int pageNumber, Action<GetTxBlockListingResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("TxBlockListing", new object[] { pageNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        /// <summary>
        /// Returns the current Transaction blockrate per second for the network.
        /// </summary>
        /// <returns></returns>
        public static async Task<GetTxBlockRateResponse> GetTxBlockRate()
        {
            ZilRequest getTxBlockReq = new ZilRequest("GetTxBlockRate", new object[] { "" });
            //Response is identical to GetTxBlock
            var result = await PostRequest<GetTxBlockRateResponse>(getTxBlockReq);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
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

        #region Misc Utils
        /// <summary>
        /// Returns the estimated block number to be reached after a given number of hours.
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static async Task<double> HoursToBlockNumber(float hours)
        {
            //Get the latest achieved block number
            var LatestBlockResponse = await GetLatestTxBlock();
            //Get the current block creation rate
            var txBlockRateReponse = await GetTxBlockRate();

            int latestBlockNum = int.Parse(LatestBlockResponse.result.header.BlockNum);
            float txBlockRate = txBlockRateReponse.result;

            //Estimate the block number after a given number of hours
            double result = latestBlockNum + Math.Round(3600 * hours * txBlockRate);

            return result;
        }
        #endregion
    }
}