using MusZil_Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zilliqa.Testing;
namespace Zilliqa.Testing
{
    public class KeyStore
    {
        private PBKDF2Wrapper pbkdf2Wrapper;
        private ScryptWrapper scryptWrapper;

        public KeyStore(PBKDF2Wrapper pbkdf2Wrapper, ScryptWrapper scryptWrapper)
        {
            this.pbkdf2Wrapper = pbkdf2Wrapper;
            this.scryptWrapper = scryptWrapper;
        }

        public static KeyStore DefaultKeyStore()
        {
            return new KeyStore(new PBKDF2Wrapper(), new ScryptWrapper());
        }

        public byte[] GetDerivedKey(byte[] password, KDFParams paras)
        {
            if (paras is PBKDF2Params)
            {
                PBKDF2Params pbkdf2Params = (PBKDF2Params)paras;
                return pbkdf2Wrapper.GetDerivedKey(password, ByteUtil.HexStringToByteArray(pbkdf2Params.Salt), pbkdf2Params.Count, pbkdf2Params.DkLen);
            }
            else if (paras is ScryptParams)
            {
                ScryptParams scryptParams = (ScryptParams)paras;
                return scryptWrapper.GetDerivedKey(password, ByteUtil.HexStringToByteArray(scryptParams.Salt), scryptParams.N, scryptParams.R, scryptParams.P, scryptParams.DkLen);
            }
            else
            {
                throw new Exception("unsupport kdf params");
            }
        }

        public string EncryptPrivateKey(string privateKey, string passphrase, KDFType type)
        {
            string address = KeyTools.GetAddressFromPrivateKey(privateKey);
            byte[] iv = KeyTools.GenerateRandomBytes(16);
            byte[] salt = KeyTools.GenerateRandomBytes(32);
            byte[] derivedKey;
            if (type == KDFType.PBKDF2)
            {
                PBKDF2Params pbkdf2Params = new PBKDF2Params();

                pbkdf2Params.Salt = ByteUtil.ByteArrayToHexString(salt);
                pbkdf2Params.DkLen = 32;
                pbkdf2Params.Count = 262144;
                derivedKey = GetDerivedKey(Encoding.Default.GetBytes(passphrase), pbkdf2Params);
            }
            else
            {
                ScryptParams scryptParams = new ScryptParams();

                scryptParams.Salt = ByteUtil.ByteArrayToHexString(salt);

                scryptParams.DkLen = 32;
                scryptParams.P = 1;
                scryptParams.R = 8;
                scryptParams.N = 8192;
                derivedKey = GetDerivedKey(Encoding.Default.GetBytes(passphrase), scryptParams);
            }

            byte[] encryptKey = new byte[16];
            Array.Copy(derivedKey, encryptKey, 16);

            KeyStoreCrypto cry = new KeyStoreCrypto();
            byte[] ciphertext = cry.GenerateAesCtrCipher(iv, encryptKey, ByteUtil.HexStringToByteArray(privateKey));
            byte[] mac = HashUtil.GenerateMac(derivedKey, ciphertext);

            //build struct
            CipherParams cipherParams = new CipherParams();
            cipherParams.Iv = ByteUtil.ByteArrayToHexString(iv);

            Kdfparams kp = new Kdfparams(ByteUtil.ToSbyte(salt));
            Crypto crypto = new Crypto();
            crypto.Cipher = "aes-128-ctr";
            crypto.Cipherparams = cipherParams;
            crypto.Ciphertext = ByteUtil.ByteArrayToHexString(ciphertext);
            crypto.Kdf = (type == KDFType.PBKDF2 ? "pbkdf2" : "scrypt");
            crypto.Kdfparams = kp;
            crypto.Mac = ByteUtil.ByteArrayToHexString(mac);

            KeystoreV3 key = new KeystoreV3();
            key.Address = address;
            key.Crypto = crypto;
            key.Id = Guid.NewGuid().ToString();
            key.Version = 3;

            return JsonConvert.SerializeObject(key);
        }

        public class KeystoreV3
        {
            public string Address { get; set; }
            public Crypto Crypto { get; set; }
            public string Id { get; set; }
            public int Version { get; set; }

        }

        public class Crypto
        {
            public string Cipher { get; set; }
            public CipherParams Cipherparams { get; set; }
            public string Ciphertext { get; set; }
            public string Kdf { get; set; }
            public Kdfparams Kdfparams { get; set; }
            public string Mac { get; set; }

        }

        public class CipherParams
        {
            public string Iv { get; set; }
        }

        public class Kdfparams
        {
            public int N { get; set; } = 8192;
            public int C { get; set; } = 262144;
            public int R { get; set; } = 8;
            public int P { get; set; } = 1;
            public int Dklen { get; set; } = 32;
            public sbyte[] Salt { get; set; }

            public Kdfparams(sbyte[] salt)
            {
                this.Salt = salt;
            }
        }

        public string DecryptPrivateKey(string encryptJson, string passphrase)
        {
            KeystoreV3 keystoreV3 = JsonConvert.DeserializeObject<KeystoreV3>(encryptJson);

            byte[] ciphertext = ByteUtil.HexStringToByteArray(keystoreV3.Crypto.Ciphertext);
            byte[] iv = ByteUtil.HexStringToByteArray(keystoreV3.Crypto.Cipherparams.Iv);
            Kdfparams kp = keystoreV3.Crypto.Kdfparams;
            string kdf = keystoreV3.Crypto.Kdf;
            byte[] derivedKey;
            if (kdf == "pbkdf2")
            {
                PBKDF2Params pbkdf2Params = new PBKDF2Params();
                pbkdf2Params.Salt = ByteUtil.ByteArrayToHexString(kp.Salt);
                pbkdf2Params.DkLen = 32;
                pbkdf2Params.Count = 262144;
                derivedKey = GetDerivedKey(Encoding.Default.GetBytes(passphrase), pbkdf2Params);
            }
            else
            {
                ScryptParams scryptParams = new ScryptParams();
                scryptParams.Salt = ByteUtil.ByteArrayToHexString(kp.Salt);
                scryptParams.DkLen = 32;
                scryptParams.P = 1;
                scryptParams.R = 8;
                scryptParams.N = 8192;

                derivedKey = GetDerivedKey(Encoding.Default.GetBytes(passphrase), scryptParams);
            }
            string mac = ByteUtil.ByteArrayToHexString(HashUtil.GenerateMac(derivedKey, ciphertext));
            if (mac.ToUpper() != keystoreV3.Crypto.Mac)
            {
                throw new Exception("Failed to decrypt.");
            }

            byte[] encryptKey = new byte[16];
            Array.Copy(derivedKey, encryptKey, 16);

            KeyStoreCrypto cry = new KeyStoreCrypto();
            byte[] ciphertextByte = cry.GenerateAesCtrCipher(iv, encryptKey, ciphertext);

            return ByteUtil.ByteArrayToHexString(ciphertextByte);

        }
    }

}