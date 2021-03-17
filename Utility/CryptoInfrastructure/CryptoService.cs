using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TestTwinCoreProject.Models;

namespace TestTwinCoreProject.Utility.CryptoInfrastructure
{
    public class CryptoService:ICryptoService
    {
        Key CurentKey { get; set; }
        private string path { get; set; }
        public CryptoService(IWebHostEnvironment enviroment)
        {
            path = enviroment.WebRootPath + "\\datakeys.json";
        }
        public static byte[] EncryptString(SymmetricAlgorithm symAlg, string inString)
        {
            byte[] inBlock = UnicodeEncoding.Unicode.GetBytes(inString);
            ICryptoTransform xfrm = symAlg.CreateEncryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(inBlock, 0, inBlock.Length);

            return outBlock;
        }

        public static string DecryptBytes(SymmetricAlgorithm symAlg, byte[] inBytes)
        {
            ICryptoTransform xfrm = symAlg.CreateDecryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(inBytes, 0, inBytes.Length);

            return UnicodeEncoding.Unicode.GetString(outBlock);
        }

        public void GetKey(DateTime? date)
        {
            KeyStorage keyStorage=null;
            //KeyStorage.Read(out keyStorage, path);
            try
            {
                KeyStorage.Read(out keyStorage, path);
            }
            catch (Exception)
            { }
            if (keyStorage == null) keyStorage = new KeyStorage();
            CurentKey = keyStorage.Keys.OrderByDescending(p => p.CreatedAt).FirstOrDefault(p => p.CreatedAt - date <= TimeSpan.FromDays(10));

            if (CurentKey == null)
            {
                AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider();
                aesCSP.GenerateKey();
                aesCSP.GenerateIV();
                CurentKey = new Key();

                CurentKey.CreatedAt = DateTime.Now;
                CurentKey.key = aesCSP.Key;
                CurentKey.IV = aesCSP.IV;
                keyStorage.Keys.Add(CurentKey);
            }

            KeyStorage.Write(keyStorage, path);
        }

        public void Encrypt(Note note)
        {
            //if (CurentKey == null) GetKey(false,note.DateTime);
            //else if (DateTime.Now - CurentKey.CreatedAt >= TimeSpan.FromDays(10)) GetKey(true, null);
            GetKey(note.DateTime);
            AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider();
            //aesCSP.Padding = PaddingMode.Zeros;
            aesCSP.Key = CurentKey.key;
            aesCSP.IV = CurentKey.IV;

            note.Content = Convert.ToBase64String(EncryptString(aesCSP, note.Content));
            
        }

        public void Decrypt(Note note)
        {
            //if (CurentKey == null) 
            //    GetKey( note.DateTime);
            //else if (CurentKey.CreatedAt - note.DateTime >= TimeSpan.FromDays(10))
                GetKey(note.DateTime);
            AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider();
            //aesCSP.Padding = PaddingMode.Zeros;
            aesCSP.Key = CurentKey.key;
            aesCSP.IV = CurentKey.IV;
            note.Content = DecryptBytes(aesCSP, Convert.FromBase64String(note.Content));
        }
    }
}
