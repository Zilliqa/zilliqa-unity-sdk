using MusZil_Core.Crypto;
using System;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using MusZil_Core.Utils;
using Newtonsoft.Json;

namespace MusZil_Core
{
    public class KeyStore
    {

        public string Address { get; set; }
        public MusCipher Crypto { get; set; }
        public string Id { get; set; }
        public int Version { get; set; }

        public KeyStore()
        {
            
        }

        

        public byte[] GenerateCipherKey(byte[] derivedKey)
        {
            var cypherKey = new byte[16];
            Array.Copy(derivedKey, cypherKey, 16);
            return cypherKey;
        }

        public byte[] GenerateDerivedScryptKey(string pwd)
        {
            //Using Scrypt.Net from nuget
            var encoder = new Scrypt.ScryptEncoder();
            //ScyptDec
            var key = encoder.Encode(pwd);

            return Encoding.UTF8.GetBytes(key);
        }

        public byte[] GeneratePbkdf2Sha256DerivedKey(string password, byte[] salt, int count, int dklen)
        {
            var pdb = new Pkcs5S2ParametersGenerator(new Sha256Digest());

            pdb.Init(PbeParametersGenerator.Pkcs5PasswordToUtf8Bytes(password.ToCharArray()), salt,
                count);
            var key = (KeyParameter)pdb.GenerateDerivedMacParameters(8 * dklen);
            return key.GetKey();
        }

        public byte[] GenerateAesCtrCipher(byte[] iv, byte[] encryptKey, byte[] input)
        {
            var key = ParameterUtilities.CreateKeyParameter("AES", encryptKey);

            var parametersWithIv = new ParametersWithIV(key, iv);

            var cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
            cipher.Init(true, parametersWithIv);
            return cipher.DoFinal(input);
        }

        public byte[] GenerateAesCtrDeCipher(byte[] iv, byte[] encryptKey, byte[] input)
        {
            var key = ParameterUtilities.CreateKeyParameter("AES", encryptKey);
            var parametersWithIv = new ParametersWithIV(key, iv);

            var cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");

            cipher.Init(false, parametersWithIv);
            return cipher.DoFinal(input);
        }


        public byte[] DecryptScrypt(string password, byte[] mac, byte[] iv, byte[] cipherText, int n, int p, int r,
            byte[] salt, int dklen)
        {
            var derivedKey = GenerateDerivedScryptKey(password);
            return Decrypt(mac, iv, cipherText, derivedKey);
        }

        public byte[] DecryptPbkdf2Sha256(string password, byte[] mac, byte[] iv, byte[] cipherText, int c, byte[] salt,
            int dklen)
        {
            var derivedKey = GeneratePbkdf2Sha256DerivedKey(password, salt, c, dklen);
            return Decrypt(mac, iv, cipherText, derivedKey);
        }

        public byte[] Decrypt(byte[] mac, byte[] iv, byte[] cipherText, byte[] derivedKey)
        {
            ValidateMac(mac, cipherText, derivedKey);
            var encryptKey = new byte[16];
            Array.Copy(derivedKey, encryptKey, 16);
            var privateKey = GenerateAesCtrCipher(iv, encryptKey, cipherText);
            return privateKey;
        }

       
        
        public byte[] GetPasswordAsBytes(string password)
        {
            return Encoding.UTF8.GetBytes(password);
        }

        public byte[] GetDerivedKey(byte[] password, KDFParams parameters)
        {
            if (parameters is Pbkdf2)
            {
                var pbkdf2Params = (Pbkdf2)parameters;
                return pbkdf2Params.GetDerivedKey(password);
            }
            else if(parameters is KDFParams)
            {
                
                return GenerateDerivedScryptKey(Encoding.UTF8.GetString(password));
            }
            else
            {
                throw new Exception("unsupport kdf params");
            }
        }

        public string EncryptPrivateKey(string privateKey, string passphrase, KDFType type)
        {
            string address = CryptoUtil.GetAddressFromPrivateKey(privateKey);
            byte[] iv = CryptoUtil.GenerateRandomBytes(16);
            byte[] salt = CryptoUtil.GenerateRandomBytes(32);
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
                var scryptParams = new KDFParams();

                scryptParams.Salt = ByteUtil.ByteArrayToHexString(salt);

                scryptParams.dklen = 32;
                scryptParams.p = 1;
                scryptParams.r = 8;
                scryptParams.n = 8192;
                derivedKey = GetDerivedKey(Encoding.Default.GetBytes(passphrase), scryptParams);
            }

            byte[] encryptKey = new byte[16];
            Array.Copy(derivedKey, encryptKey, 16);

            KeyStore cry = new KeyStore();
            byte[] ciphertext = cry.GenerateAesCtrCipher(iv, encryptKey, ByteUtil.HexStringToByteArray(privateKey));
            //build struct
            CipherParams cipherParams = new CipherParams();
            cipherParams.Iv = ByteUtil.ByteArrayToHexString(iv);

            var kp = new KDFParams() { Salt = Encoding.UTF8.GetString(salt)};
            var crypto = new MusCipher();
            crypto.Cipher = "aes-128-ctr";
            crypto.Cipherparams = cipherParams;
            crypto.Ciphertext = ByteUtil.ByteArrayToHexString(ciphertext);
            crypto.Kdf = (type == KDFType.PBKDF2 ? "pbkdf2" : "scrypt");
            crypto.KdfParams = kp;
            crypto.Mac = Encoding.UTF8.GetString(HashUtil.GenerateMac(derivedKey,ciphertext));

            KeyStore key = new KeyStore();
            key.Address = "0x" + address;
            key.Crypto = crypto;
            key.Id = Guid.NewGuid().ToString();
            key.Version = 3;

            return JsonConvert.SerializeObject(key);
        }
      
        public string DecryptPrivateKey(string encryptJson, string passphrase)
        {
            KeyStore keystore = JsonConvert.DeserializeObject<KeyStore>(encryptJson);

            byte[] ciphertext = ByteUtil.HexStringToByteArray(keystore.Crypto.Ciphertext);
            byte[] iv = ByteUtil.HexStringToByteArray(keystore.Crypto.Cipherparams.Iv);
            KDFParams kp = keystore.Crypto.KdfParams;
            string kdf = keystore.Crypto.Kdf;
            byte[] derivedKey;
            if (kdf == "pbkdf2")
            {
                PBKDF2Params pbkdf2Params = new PBKDF2Params();
                pbkdf2Params.Salt = ByteUtil.ByteArrayToHexString(Encoding.UTF8.GetBytes(kp.Salt));
                pbkdf2Params.DkLen = 32;
                pbkdf2Params.Count = 262144;
                derivedKey = GetDerivedKey(Encoding.Default.GetBytes(passphrase), pbkdf2Params);
            }
            else
            {
                KDFParams scryptParams = new KDFParams();
                scryptParams.Salt = ByteUtil.ByteArrayToHexString(Encoding.UTF8.GetBytes(kp.Salt));
                scryptParams.dklen = 32;
                scryptParams.p = 1;
                scryptParams.r = 8;
                scryptParams.n = 8192;

                derivedKey = GetDerivedKey(Encoding.Default.GetBytes(passphrase), scryptParams);
            }
            string mac = ByteUtil.ByteArrayToHexString(HashUtil.GenerateMac(derivedKey, ciphertext));
            if (mac.ToUpper() != keystore.Crypto.Mac)
            {
                throw new Exception("Failed to decrypt.");
            }

            byte[] encryptKey = new byte[16];
            Array.Copy(derivedKey, encryptKey, 16);

            byte[] ciphertextByte = GenerateAesCtrCipher(iv, encryptKey, ciphertext);

            return ByteUtil.ByteArrayToHexString(ciphertextByte);

        }
        private void ValidateMac(byte[] mac, byte[] cipherText, byte[] derivedKey)
        {
            var generatedMac = HashUtil.GenerateMac(derivedKey, cipherText);
            if (ByteUtil.ByteArrayToHexString(generatedMac) != ByteUtil.ByteArrayToHexString(mac))
                throw new Exception(
                    "Cannot derive the same mac as the one provided from the cipher and derived key");
        }
    }
}
