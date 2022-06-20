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

        public static string apiUrl = "https://dev-api.zilliqa.com/";//"https://api.zilliqa.com/";
        #endregion

        #region Blockchain
        public static async Task<GetTxBlockResponse> GetLatestTxBlock(Action<GetTxBlockResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetLatestTxBlock", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetDSBlockListingResponse> GetDSBlockListing(int pageNumber, Action<GetDSBlockListingResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("DSBlockListing", new object[] { pageNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetBlockchainInfoResponse> GetBlockchainInfo(Action<GetBlockchainInfoResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetBlockchainInfo", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetCurrentDSEpochResponse> GetCurrentDSEpoch(Action<GetCurrentDSEpochResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetCurrentDSEpoch", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetCurrentMiniEpochResponse> GetCurrentMiniEpoch(Action<GetCurrentMiniEpochResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetCurrentMiniEpoch", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetDSBlockResponse> GetDsBlock(string DSBlockNumber, Action<GetDSBlockResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetDsBlock", new object[] { DSBlockNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetDSBlockRateResponse> GetDSBlockRate(Action<GetDSBlockRateResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetDSBlockRate", new object[] { });
            var result = await PostRequest(req, onComplete); ;

            return result;
        }

        public static async Task<GetLatestDSBlockResponse> GetLatestDsBlock(Action<GetLatestDSBlockResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetLatestDsBlock", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetMinerInfoResponse> GetMinerInfo(string DSBlockNumber, Action<GetMinerInfoResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetMinerInfo", new object[] { DSBlockNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetNetworkIdResponse> GetNetworkId(Action<GetNetworkIdResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetNetworkId", new object[] { });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetNumDSBlocksResponse> GetNumDSBlocks(Action<GetNumDSBlocksResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetNumDSBlocks", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetNumTxBlocksResponse> GetNumTransactions(Action<GetNumTxBlocksResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetNumTransactions", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetNumTxBlocksResponse> GetNumTxBlocks(Action<GetNumTxBlocksResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetNumTxBlocks", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetDSDifficultyResponse> GetPrevDifficulty(Action<GetDSDifficultyResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetPrevDifficulty", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetDSDifficultyResponse> GetPrevDSDifficulty(Action<GetDSDifficultyResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetPrevDifficulty", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetTotalCoinSupplyResponse> GetTotalCoinSupply(Action<GetTotalCoinSupplyResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTotalCoinSupply", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetTxBlockRateResponse> GetTransactionRate(Action<GetTxBlockRateResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTransactionRate", new object[] { "" });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetTxBlockResponse> GetTxBlock(string transactionBlock, Action<GetTxBlockResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("GetTxBlock", new object[] { transactionBlock });
            var result = await PostRequest(req, onComplete);

            return result;
        }

        public static async Task<GetTxBlockListingResponse> TxBlockListing(int pageNumber, Action<GetTxBlockListingResponse, ZilResponse.Error> onComplete = null)
        {
            ZilRequest req = new ZilRequest("TxBlockListing", new object[] { pageNumber });
            var result = await PostRequest(req, onComplete);

            return result;
        }



        public static async Task<GetTxBlockRateResponse> GetTxBlockRate()
        {
            ZilRequest getTxBlockReq = new ZilRequest("GetTxBlockRate", new object[] { "" });
            //Response is identical to GetTxBlock
            var result = await PostRequest<GetTxBlockRateResponse>(getTxBlockReq);

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

        #region Misc Utils

        public static async Task<double> HoursToBlockNumber(float hours)
        {
            float DEFAULT_BLOCK_TX_RATE = .01f;

            var LatestBlockResponse = await ZilliqaRPC.GetLatestTxBlock();
            var txBlockRateReponse = await ZilliqaRPC.GetTxBlockRate();

            int latestBlockNum = int.Parse(LatestBlockResponse.result.header.BlockNum);
            float txBlockRate = txBlockRateReponse.result;

            double result = latestBlockNum + Math.Round(3600 * hours * txBlockRate);

            return result;
        }
        #endregion
    }
}