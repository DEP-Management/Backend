namespace DEP.Service.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string textToEncrypt);
        string Decrypt(string textToDecrypt);
    }
}
