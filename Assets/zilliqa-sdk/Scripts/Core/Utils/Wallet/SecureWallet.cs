using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zilliqa.Core;
using Zilliqa.Core.Beta;
using Zilliqa.Core.Crypto;
using Zilliqa.Utils;
public class SecureWallet : Wallet
{

    public SecureWallet(string _privateKey) : base(_privateKey) { }

    public static WalletKey GenerateWallet(string passphrase)
    {
        var ecKeyPair = Schnorr.GenerateKeyPair();
        return Secure(passphrase, ecKeyPair);
    }


    private static WalletKey Secure(string passphrase, ECKeyPair keyPair)
    {
        byte[] salt = CryptoUtil.GenerateRandomBytes(32);

        var scryptParams = new KDFParams();
        scryptParams.Salt = ByteUtil.ByteArrayToHexString(salt);
        scryptParams.dklen = 32;
        scryptParams.p = 1;
        scryptParams.r = 8;
        scryptParams.n = 8192;

        WalletKey walletKey = new WalletKey(scryptParams);

        byte[] tempDerivedK = GenerateDerivedKey(passphrase, walletKey.scrypt);
        byte[] derivedKey = CryptoUtil.GenerateRandomBytes(walletKey.scrypt.dklen);
        //Array.Copy(tempDerivedK, tempDerivedK.Length - 32, derivedKey, 0, 32);
        Array.Copy(tempDerivedK, derivedKey, scryptParams.dklen);
        Debug.Log("derived on secure " + ByteUtil.ByteArrayToHexString(derivedKey));

        byte[] iv = CryptoUtil.GenerateRandomBytes(16);
        byte[] ivPk = CryptoUtil.GenerateRandomBytes(16);

        byte[] secretKey = CryptoUtil.GenerateRandomBytes(32);
        byte[] ciphertext = HashUtil.GenerateAesCtrCipher(iv, derivedKey, secretKey);

        byte[] ciphertextPK = HashUtil.GenerateAesCtrCipher(ivPk, secretKey, BigIntegers.AsUnsignedByteArray(keyPair.privateKey));

        byte[] mac = HashUtil.GenerateMac(derivedKey, ciphertext);
        byte[] macPk = HashUtil.GenerateMac(secretKey, ciphertextPK);
        Debug.Log("mac on secure " + ByteUtil.ByteArrayToHexString(mac));
        Debug.Log("macPK on secure " + ByteUtil.ByteArrayToHexString(macPk));
        Debug.Log("cipher on secure " + ByteUtil.ByteArrayToHexString(secretKey));


        walletKey.secretCipher = ByteUtil.ByteArrayToHexString(ciphertext);
        walletKey.pkCipher = ByteUtil.ByteArrayToHexString(ciphertextPK);
        walletKey.secretCipher = ByteUtil.ByteArrayToHexString(ciphertext);
        walletKey.secretMac = ByteUtil.ByteArrayToHexString(mac);
        walletKey.pkMac = ByteUtil.ByteArrayToHexString(macPk);
        walletKey.iv = ByteUtil.ByteArrayToHexString(iv);
        walletKey.ivPk = ByteUtil.ByteArrayToHexString(ivPk);

        PlayerPrefs.SetString("walletkey", JsonConvert.SerializeObject(walletKey));
        PlayerPrefs.Save();
        return walletKey;
    }

    public static Wallet Login(string passphrase)
    {
        var walletKey = JsonConvert.DeserializeObject<WalletKey>(PlayerPrefs.GetString("walletkey"));

        byte[] tempDerivedK = GenerateDerivedKey(passphrase, walletKey.scrypt);
        byte[] _derivedKey = CryptoUtil.GenerateRandomBytes(walletKey.scrypt.dklen);
        //Array.Copy(tempDerivedK, tempDerivedK.Length - 32, _derivedKey, 0, 32);
        Array.Copy(tempDerivedK, _derivedKey, walletKey.scrypt.dklen);
        Debug.Log("derived on login " + ByteUtil.ByteArrayToHexString(_derivedKey));

        byte[] _mac = HashUtil.GenerateMac(_derivedKey, walletKey.secretCipherBytes);
        Debug.Log("mac on login " + ByteUtil.ByteArrayToHexString(_mac));

        if (ByteUtil.ByteArrayToHexString(_mac) != walletKey.secretMac)
        {
            Debug.Log("Invalid secret key ");
            return null;
        }

        byte[] encryptKey = new byte[32];
        Array.Copy(_derivedKey, encryptKey, 32);

        byte[] secretKey = HashUtil.GenerateAesCtrCipher(walletKey.ivBytes, encryptKey, walletKey.secretCipherBytes);
        Debug.Log("secretKey on login " + ByteUtil.ByteArrayToHexString(secretKey));

        byte[] _macPk = HashUtil.GenerateMac(secretKey, walletKey.pkCipherBytes);
        Debug.Log("macPK on login " + ByteUtil.ByteArrayToHexString(_macPk));

        if (ByteUtil.ByteArrayToHexString(_macPk) != walletKey.pkMac)
        {
            Debug.Log("Failed to login ");
            return null;
        }

        byte[] _privateKey = HashUtil.GenerateAesCtrCipher(walletKey.ivPkBytes, secretKey, walletKey.pkCipherBytes);
        Debug.Log("private key on login " + ByteUtil.ByteArrayToHexString(_privateKey));
        Debug.Log("Login Succesful");

        return new Wallet(ByteUtil.ByteArrayToHexString(_privateKey));
    }

    private static byte[] GenerateDerivedKey(string passphrase, KDFParams scryptParams)
    {
        return HashUtil.GetDerivedKey(Encoding.Default.GetBytes(passphrase), scryptParams);
    }

    public string DecryptPK()
    {
        //input params: key, iv, ciphertext
        //output: decrypted data
        var key = ParameterUtilities.CreateKeyParameter("AES", encryptionKey);
        var parametersWithIv = new ParametersWithIV(key, iv);
        var cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
        cipher.Init(true, parametersWithIv);
        var decryptedPk = cipher.DoFinal(ByteUtil.HexStringToByteArray(encryptedPk));

        return ByteUtil.ByteArrayToHexString(decryptedPk);
    }

}

public class WalletKey
{
    public string secretCipher;
    public string pkCipher;
    public KDFParams scrypt;
    public string secretMac;
    public string pkMac;
    public string iv;
    public string ivPk;

    public byte[] secretCipherBytes => ByteUtil.HexStringToByteArray(secretCipher);
    public byte[] pkCipherBytes => ByteUtil.HexStringToByteArray(pkCipher);
    public byte[] secretMacBytes => ByteUtil.HexStringToByteArray(secretMac);
    public byte[] pkMacBytes => ByteUtil.HexStringToByteArray(pkMac);
    public byte[] ivBytes => ByteUtil.HexStringToByteArray(iv);
    public byte[] ivPkBytes => ByteUtil.HexStringToByteArray(ivPk);

    public WalletKey(KDFParams _params)
    {
        scrypt = _params;
    }
}