using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zilliqa.Core;

public class KeyStoreLogin : MonoBehaviour
{
    private string privateKey = "3375F915F3F9AE35E6B301B7670F53AD1A5BE15D8221EC7FD5E503F21D3450C8";
    private string passphrase = "12345678";
    public string keystoreExample;

    // Start is called before the first frame update
    void Start()
    {
        var keystoreStr = KeyStore.DefaultKeyStore().EncryptPrivateKey(privateKey, passphrase, KDFType.Scrypt);
        Debug.Log("encrypted json " + keystoreStr);
        var priK = KeyStore.DefaultKeyStore().DecryptPrivateKey(keystoreStr, passphrase);
        Debug.Log("result private key " + priK);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
