using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using DEP.Service.Interfaces;
using DEP.Service.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DEP.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepo;
        private readonly IConfiguration configuration;
        private readonly IEncryptionService encryptionService;

        public AuthService(IUserRepository userRepo, IConfiguration configuration, IEncryptionService encryptionService)
        {
            this.userRepo = userRepo;
            this.configuration = configuration;
            this.encryptionService = encryptionService;
        }

        public async Task<AuthenticatedResponse> Login(LoginViewModel loginRequest)
        {
            var allUsers = await userRepo.GetUsers(); // load all users

            User? matchedUser = null;

            foreach (var user in allUsers)
            {
                var decryptedUsername = encryptionService.Decrypt(user.UserName);
                if (decryptedUsername.Equals(loginRequest.Username, StringComparison.OrdinalIgnoreCase))
                {
                    matchedUser = user;
                    break;
                }
            }

            if (matchedUser == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(loginRequest.Password, matchedUser.PasswordHash, matchedUser.PasswordSalt))
            {
                return null;
            }

            string newAccessToken = CreateJwtToken(matchedUser);
            string newRefreshToken = await CreateRefreshToken();
            matchedUser.RefreshToken = newRefreshToken;
            matchedUser.RefreshTokenExpiryDate = DateTime.Now.AddDays(1);
            await userRepo.UpdateUser(matchedUser);

            return new AuthenticatedResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                PasswordExpiryDate = matchedUser.PasswordExpiryDate,
            };
        }

        public enum ChangePasswordResult
        {
            Success,
            UserNotFound,
            WrongOldPassword,
            Failure
        }

        public async Task<ChangePasswordResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            var user = await userRepo.GetUserById(viewModel.UserId);

            if (user is null)
            {
                return ChangePasswordResult.UserNotFound;
            }

            // Verify that the old password is correct
            if (!VerifyPasswordHash(viewModel.OldPassword, user.PasswordHash, user.PasswordSalt))
            {
                return ChangePasswordResult.WrongOldPassword;
            }

            try
            {
                // Create new passwordHash and passwordSalt for the new password
                CreatePasswordHash(viewModel.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.PasswordExpiryDate = DateTime.Now.AddMonths(3);

                await userRepo.UpdateUser(user);

                return ChangePasswordResult.Success;
            }
            catch
            {
                return ChangePasswordResult.Failure;
            }
        }

        public async Task<bool> ResetPassword(int userId)
        {
            var user = await userRepo.GetUserById(userId);

            if (user is null)
            {
                return false;
            }

            var newPassword = configuration.GetSection("AppSettings:Password").Value;

            //Create new passwordHash and passwordSalt for the new password
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;


            await userRepo.UpdateUser(user);
            return true;
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public string CreateJwtToken(User user)
        {
            //user.Name = encryptionService.Decrypt(user.Name);
            var name = encryptionService.Decrypt(user.Name);


            // Adding claims, claims are Key-Value pairs that can be used after the token is decoded.
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, user.UserRole.ToString()),
                new Claim("userId", user.UserId.ToString()),
                new Claim(ClaimTypes.Name, name.ToString()),
                //new Claim(ClaimTypes.NameIdentifier, user.Name.ToString()),
                new Claim("roleId", ((int)user.UserRole).ToString())
            };

            //user.Name = encryptionService.Encrypt(user.Name);
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));
            var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: signInCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }

        public async Task<string> CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            // Check if token exists in the Database already.
            var tokenInUser = await userRepo.GetUserByRefreshToken(refreshToken);
            if (tokenInUser is not null)
            {
                // If token already exists then run the method again.
                return await CreateRefreshToken();
            }
            return refreshToken;
        }
    }
}
