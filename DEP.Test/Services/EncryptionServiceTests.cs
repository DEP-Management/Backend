using DEP.Service.Services;
using Microsoft.AspNetCore.DataProtection;
using Xunit;

namespace DEP.Test.Services
{
    public class EncryptionServiceTests
    {
        private readonly EncryptionService _service;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public EncryptionServiceTests()
        {
            // Use a real instance of IDataProtectionProvider
            _dataProtectionProvider = DataProtectionProvider.Create("qwertyqwe");

            // Instantiate the EncryptionService with the real provider
            _service = new EncryptionService(_dataProtectionProvider);
        }

        [Fact]
        public void Encrypt_ShouldReturnEncryptedText()
        {
            // Arrange
            string originalText = "mySecret";

            // Act
            var encryptedText = _service.Encrypt(originalText);

            // Assert
            Assert.NotEqual(originalText, encryptedText); // Ensure encryption is happening
        }

        [Fact]
        public void Decrypt_ShouldReturnDecryptedText()
        {
            // Arrange
            string originalText = "mySecret";
            var encryptedText = _service.Encrypt(originalText); // Encrypt first

            // Act
            var decryptedText = _service.Decrypt(encryptedText);

            // Assert
            Assert.Equal(originalText, decryptedText); // Ensure decryption returns the original text
        }

        [Fact]
        public void EncryptAndDecrypt_ShouldReturnOriginalText()
        {
            // Arrange
            string originalText = "mySecret";

            // Act
            var encryptedText = _service.Encrypt(originalText);
            var decryptedText = _service.Decrypt(encryptedText);

            // Assert
            Assert.Equal(originalText, decryptedText); // Verify round-trip encryption/decryption
        }
    }
}
