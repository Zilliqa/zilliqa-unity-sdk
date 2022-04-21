using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour
{
    public static IEnumerator WaitForWWW(WWW www, bool showDebug, string mehodId)
    {
        yield return www;

        string ret;
        if (string.IsNullOrEmpty(www.error))
            ret = www.text;
        else
            ret = www.error;

        if (showDebug)
            Debug.Log(mehodId + ":\n" + ret);
    }
}
