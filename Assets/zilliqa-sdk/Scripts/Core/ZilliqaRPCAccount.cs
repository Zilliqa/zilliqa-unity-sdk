using System;
using System.Threading.Tasks;
using UnityEngine;

using Zilliqa.Requests;

namespace Zilliqa.Core
{
    public partial class ZilliqaRPC
    {
        /// <summary>
        /// <para>
        /// Returns the current balance of an account, measured in the smallest accounting unit Qa (or 10^-12 Zil). This is represented as a String.
        /// </para>
        /// <para>
        /// Returns the current nonce of an account. This is represented as a Number.
        /// </para>
        /// </summary>
        /// <param name="sender">the caller's address</param>
        /// <param name="onResponse"></param>
        /// <returns></returns>
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
