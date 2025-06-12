using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using DEP.Repository.ViewModels;
using DEP.Service.EncryptionHelpers;
using DEP.Service.Interfaces;
using DEP.Service.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Collections;

namespace DEP.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IAuthService authService;
        private readonly IConfiguration configuration;
        private readonly IPersonRepository personRepository;
        private readonly IEncryptionService encryptionService;

        public UserService(
            IUserRepository userRepository, 
            IAuthService authService, 
            IConfiguration configuration, 
            IPersonRepository personRepository, 
            IEncryptionService encryptionService)
        {
            this.userRepository = userRepository;
            this.authService = authService;
            this.configuration = configuration;
            this.personRepository = personRepository;
            this.encryptionService = encryptionService;
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await userRepository.GetUsers();
            foreach (var user in users)
            {
                user.PasswordHash = new byte[32];
                user.PasswordSalt = new byte[32];
            }
            UserEncryptionHelper.Decrypt(users, encryptionService);
            return users;
        }

        public async Task<List<User>> GetUsersByEducationBossId(int id)
        {
            var leaders = await userRepository.GetUsersByEducationBossId(id);

            UserEncryptionHelper.Decrypt(leaders, encryptionService);

            return leaders;
        }

        public async Task<List<User>> GetUsersByUserRole(UserRole userRole)
        {
            var users = await userRepository.GetUsersByUserRole(userRole);
            UserEncryptionHelper.Decrypt(users, encryptionService);
            return users;
        }

        public async Task<User> GetUserById(int id)
        {
            return await userRepository.GetUserById(id);
        }

        public async Task<UserDashboardViewModel?> GetUserDashboardById(int id)
        {
            var userDashBoard = await userRepository.GetUserDashboardById(id);

            if (userDashBoard is not null)
            {
                UserEncryptionHelper.Decrypt(userDashBoard, encryptionService);
            }

            return userDashBoard;
        }

        public async Task<List<User>> GetUserByName(string name)
        {
            return await userRepository.GetUserByName(name);
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var users = await userRepository.GetUsers();

            UserEncryptionHelper.DecryptNames(users, encryptionService);

            var existingUser = users.FirstOrDefault(x => x.UserName == username);

            UserEncryptionHelper.EncryptNames(users, encryptionService);

            return existingUser;
        }

        public async Task<UserViewModel> AddUser(AddUserViewModel viewModel)
        {
            var defaultPass = configuration.GetSection("UserSettings:DefaultPassword").Value;
            authService.CreatePasswordHash(defaultPass, out byte[] passwordHash, out byte[] passwordSalt);

            User newUser = new User
            {
                UserName = viewModel.Username,
                Name = viewModel.Name,
                LocationId = viewModel.LocationId,
                DepartmentId = viewModel.DepartmentId,
                EducationBossId = viewModel.EducationBossId,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserRole = viewModel.UserRole,
                PasswordExpiryDate = DateTime.Now.AddDays(-1)
            };

            UserEncryptionHelper.Encrypt(newUser, encryptionService);

            var createdUser = await userRepository.AddUser(newUser);

            // Optional: Decrypt here if you're returning decrypted data
            UserEncryptionHelper.Decrypt(createdUser, encryptionService);

            return new UserViewModel
            {
                UserId = createdUser.UserId,
                Name = createdUser.Name,
                LocationId = createdUser.LocationId,
                LocationName = createdUser.Location?.Name,
                DepartmentId = createdUser.DepartmentId,
                DepartmentName = createdUser.Department?.Name,
                EducationBossId = createdUser.EducationBossId,
                EducationBossName = createdUser.EducationBoss?.Name,
                UserRole = createdUser.UserRole
            };
        }

        public async Task<bool> ReassignUser(ReassignUserViewModel model)
        {
            return await userRepository.ReassignUser(model);
        }

        public async Task<bool> DeleteUser(int id)
        {
            return await userRepository.DeleteUser(id);
        }

        public async Task<bool> UpdateUser(User user)
        {
            //UserEncryptionHelper.Encrypt(user, encryptionService);
            return await userRepository.UpdateUser(user);
        }

        public async Task<bool> UpdateUserFromViewModel(UserViewModel viewModel)
        {
            var user = await userRepository.GetUserById(viewModel.UserId);

            if (user is null)
            {
                return false;
            }

            // Check if the role is changing
            bool isRoleChanging = user.UserRole != viewModel.UserRole;

            // Fetch persons where this user is assigned as a role
            if (isRoleChanging)
            {
                var affectedPersons = await personRepository.GetPersonsByUserId(user.UserId);

                foreach (var person in affectedPersons)
                {
                    // If user is no longer eligible for a role, set it to null
                    if (user.UserRole == UserRole.Uddannelsesleder && person.EducationalLeaderId == user.UserId)
                    {
                        person.EducationalLeaderId = null;
                    }
                    if (user.UserRole == UserRole.Pædagogisk_konsulent && person.EducationalConsultantId == user.UserId)
                    {
                        person.EducationalConsultantId = null;
                    }
                    if (user.UserRole == UserRole.Driftskoordinator && person.OperationCoordinatorId == user.UserId)
                    {
                        person.OperationCoordinatorId = null;
                    }
                }

                // Update all affected persons
                await personRepository.UpdatePersons(affectedPersons);
            }

            // Map updated values to existing user
            user.Name = viewModel.Name;
            user.DepartmentId = viewModel.DepartmentId;
            user.EducationBossId = viewModel.EducationBossId;
            user.LocationId = viewModel.LocationId;
            user.UserRole = viewModel.UserRole;

            UserEncryptionHelper.EncryptUpdatableFields(user, encryptionService);

            return await userRepository.UpdateUser(user);
        }


        public async Task<List<EducationBossViewModel>> GetEducationBosses()
        {
            var bosses = await userRepository.GetEducationBossesExcel();
            var viewModel = new List<EducationBossViewModel>();

            foreach (var boss in bosses)
            {
                var bossViewModel = new EducationBossViewModel
                {
                    UserId = boss.UserId,
                    Name = boss.Name,
                    UserRole = boss.UserRole,

                };

                viewModel.Add(bossViewModel);
            }

            return viewModel;
        }

        public async Task<List<EducationBossViewModel>> GetEducationBossesExcel()
        {
            var bosses = await userRepository.GetEducationBossesExcel();

            UserEncryptionHelper.Decrypt(bosses, encryptionService);

            var viewModel = new List<EducationBossViewModel>();

            foreach (var boss in bosses)
            {
                var leaderViewModel = new List<EducationLeaderViewModel>();
                // Fetch leaders under this boss
                foreach (var leader in boss.EducationLeaders)
                {
                    //UserEncryptionHelper.Decrypt(boss.EducationLeaders, encryptionService);
                    var persons = await personRepository.GetPersonsExcel(leader.UserId);
                    PersonEncryptionHelper.Decrypt(persons, encryptionService);
                    var EducationalleaderViewModel = new EducationLeaderViewModel
                    {
                        UserId = leader.UserId,
                        Name = leader.Name,
                        UserRole = leader.UserRole,
                        EducationBossId = boss.UserId,
                        Location = leader.Location,
                        Department = leader.Department,
                        Teachers = persons.ToList()
                    };

                    leaderViewModel.Add(EducationalleaderViewModel);
                }
                var bossViewModel = new EducationBossViewModel
                {
                    UserId = boss.UserId,
                    Name = boss.Name,
                    UserRole = boss.UserRole,
                    EducationalLeaders = leaderViewModel
                };

                viewModel.Add(bossViewModel);
            }

            return viewModel;
        }

        public async Task<List<EducationBossViewModel>> GetSelctedEducationBossExcel(int id)
        {
            var boss = await userRepository.GetEducationBossByIdExcel(id);

            var viewModel = new EducationBossViewModel
            {
                UserId = boss.UserId,
                Name = boss.Name,
            };

            // Fetch leaders under this boss
            foreach (var leader in boss.EducationLeaders)
            {
                var persons = await personRepository.GetPersonsExcel(leader.UserId);

                var leaderViewModel = new EducationLeaderViewModel
                {
                    UserId = leader.UserId,
                    Name = leader.Name,
                    UserRole = leader.UserRole,
                    EducationBossId = boss.UserId,
                    Location = leader.Location,
                    Department = leader.Department,
                    Teachers = persons.ToList()
                };

                viewModel.EducationalLeaders.Add(leaderViewModel);
            }

            return new List<EducationBossViewModel> { viewModel };
        }

        public async Task<List<EducationLeaderViewModel>> GetSelectedEducationLeaderExcel(int id)
        {
            var leader = await userRepository.GetEducationLeaderByIdExcel(id);
            var persons = await personRepository.GetPersonsExcel(leader.UserId);

            var leaderViewModel = new EducationLeaderViewModel
            {
                UserId = leader.UserId,
                Name = leader.Name,
                UserRole = leader.UserRole,
                EducationBossId = leader.EducationBossId,
                Location = leader.Location,
                Department = leader.Department,
                Teachers = persons
            };

            return new List<EducationLeaderViewModel> { leaderViewModel };
        }

    }
}
