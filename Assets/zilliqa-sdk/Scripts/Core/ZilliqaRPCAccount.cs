using System;
using System.Threading.Tasks;
using UnityEngine;

using Zilliqa.Requests;

namespace Zilliqa.Core
{
    public partial class ZilliqaRPC
    {
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

    }
}
