using DEP.Repository.Models;
using DEP.Service.Interfaces;

namespace DEP.Service.EncryptionHelpers
{
    public static class LocationEncryptionHelper
    {
        public static void Encrypt(Location location, IEncryptionService enc)
        {
            location.Name = enc.Encrypt(location.Name);
        }

        public static void Decrypt(Location location, IEncryptionService enc)
        {
            location.Name = enc.Decrypt(location.Name);
        }
    }

}
