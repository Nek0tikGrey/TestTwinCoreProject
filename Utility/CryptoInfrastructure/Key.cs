using System;

namespace TestTwinCoreProject.Utility.CryptoInfrastructure
{
    [Serializable]
    public class Key
    {
        public byte[] key;
        public byte[] IV;
        public DateTime CreatedAt;
    }
}
