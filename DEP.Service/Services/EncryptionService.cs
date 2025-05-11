using DEP.Service.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace DEP.Service.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IDataProtector _protector;

        public EncryptionService(IDataProtectionProvider protector)
        {
            _protector = protector.CreateProtector("qwertyqwe");
        }

        public string Encrypt(string textToEncrypt)
        {
            return _protector.Protect(textToEncrypt);
        }

        public string Decrypt(string textToDecrypt)
        {
            return _protector.Unprotect(textToDecrypt);
        }
    }
}
