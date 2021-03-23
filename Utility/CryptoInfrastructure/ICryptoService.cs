using TestTwinCoreProject.Models;

namespace TestTwinCoreProject.Utility.CryptoInfrastructure
{
    public interface ICryptoService
    {
        public void Encrypt(Note note);
        public void Decrypt(Note note);
    }
}
