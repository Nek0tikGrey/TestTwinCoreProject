using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TestTwinCoreProject.Utility.CryptoInfrastructure
{
    [Serializable]
    public class KeyStorage
    {
        public List<Key> Keys { get; set; } = new List<Key>();
        public static void Read(out KeyStorage storage, string path = "datakeys.dat")
        {
            using (StreamReader fs = new StreamReader(path))
            {
                storage = JsonConvert.DeserializeObject<KeyStorage>(fs.ReadToEnd());
            }
        }
        public static void Write(in KeyStorage storage, string path = "datakeys.dat")
        {
            if (storage != null)
            {
                using (StreamWriter fs = new StreamWriter(path))
                    fs.Write(JsonConvert.SerializeObject(storage));
            }
            else
                throw new ArgumentException();
        }

    }
}
