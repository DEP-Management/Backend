using DEP.Repository.Context;
using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using DEP.Repository.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DEP.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext context;

        public UserRepository(DatabaseContext context)
        {
            this.context = context;
        }

        public async Task<List<User>> GetUsers()
        {
            return await context.Users
                .Select(u => new User
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Name = u.Name,
                    DepartmentId = u.DepartmentId,
                    LocationId = u.LocationId,
                    EducationBossId = u.EducationBossId,
                    UserRole = u.UserRole,
                    PasswordHash = u.PasswordHash,
                    PasswordSalt = u.PasswordSalt,
                    PasswordExpiryDate = u.PasswordExpiryDate,
                    RefreshToken = u.RefreshToken,
                    RefreshTokenExpiryDate = u.RefreshTokenExpiryDate,
                    Department = u.Department == null ? null : new Department
                    {
                        DepartmentId = u.Department.DepartmentId,
                        Name = u.Department.Name
                    },
                    Location = u.Location == null ? null : new Location
                    {
                        LocationId = u.Location.LocationId,
                        Name = u.Location.Name
                    },
                    EducationBoss = u.EducationBoss == null ? null : new User
                    {
                        UserId = u.EducationBoss.UserId,
                        UserName = u.EducationBoss.UserName,
                        Name = u.EducationBoss.Name
                    }
                })
                .ToListAsync();
        }

        public async Task<User?> GetEducationLeaderByIdExcel(int id)
        {
            return await context.Users
                .Where(u => u.UserId == id)
                .Select(el => new User
                {
                    UserId = el.UserId,
                    Name = el.Name,
                    UserRole = el.UserRole,
                    Department = el.Department == null ? null : new Department
                    {
                        DepartmentId = el.Department.DepartmentId,
                        Name = el.Department.Name
                    },
                    Location = el.Location == null ? null : new Location
                    {
                        LocationId = el.Location.LocationId,
                        Name = el.Location.Name
                    },

                })
                .FirstOrDefaultAsync();
        }


        public async Task<List<User>> GetEducationBossesExcel()
        {
            var test = await context.Users
                .Where(u => u.UserRole == UserRole.Uddannelseschef)
                .Select(eb => new User
                {
                    UserId = eb.UserId,
                    Name = eb.Name,
                    UserRole = eb.UserRole,
                    EducationLeaders = eb.EducationLeaders
                        .Select(el => new User
                        {
                            UserId = el.UserId,
                            Name = el.Name,
                            UserRole = el.UserRole,
                            Department = el.Department == null ? null : new Department
                            {
                                DepartmentId = el.Department.DepartmentId,
                                Name = el.Department.Name
                            },
                            Location = el.Location == null ? null : new Location
                            {
                                LocationId = el.Location.LocationId,
                                Name = el.Location.Name
                            },
                        })
                        .ToList()
                })
                .ToListAsync();

            return test;
        }


        public async Task<User?> GetEducationBossByIdExcel(int id)
        {
            return await context.Users
                .Where(eb => eb.UserId == id)
                .Select(eb => new User
                {
                    UserId = eb.UserId,
                    Name = eb.Name,
                    UserRole = eb.UserRole,
                    EducationLeaders = eb.EducationLeaders
                        .Select(el => new User
                        {
                            UserId = el.UserId,
                            Name = el.Name,
                            UserRole = el.UserRole,
                            Department = el.Department == null ? null : new Department
                            {
                                DepartmentId = el.Department.DepartmentId,
                                Name = el.Department.Name
                            },
                            Location = el.Location == null ? null : new Location
                            {
                                LocationId = el.Location.LocationId,
                                Name = el.Location.Name
                            },
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }


        public async Task<List<User>> GetUsersByEducationBossId(int id)
        {
            return await context.Users
                .Where(x => x.EducationBossId == id)
                .Select(u => new User
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Name = u.Name,
                    DepartmentId = u.DepartmentId,
                    LocationId = u.LocationId,
                    EducationBossId = u.EducationBossId,
                    UserRole = u.UserRole,
                    Department = u.Department == null ? null : new Department
                    {
                        DepartmentId = u.Department.DepartmentId,
                        Name = u.Department.Name
                    },
                    Location = u.Location == null ? null : new Location
                    {
                        LocationId = u.Location.LocationId,
                        Name = u.Location.Name
                    }
                })
                .ToListAsync();
        }


        public async Task<List<User>> GetUsersByUserRole(UserRole userRole)
        {
            var query = context.Users
                .Where(x => x.UserRole == userRole);

            if (userRole == UserRole.Uddannelsesleder)
            {
                return await query
                    .Select(u => new User
                    {
                        UserId = u.UserId,
                        UserName = u.UserName,
                        Name = u.Name,
                        UserRole = u.UserRole,
                        DepartmentId = u.DepartmentId,
                        LocationId = u.LocationId,
                        EducationBossId = u.EducationBossId,
                        Department = u.Department == null ? null : new Department
                        {
                            DepartmentId = u.Department.DepartmentId,
                            Name = u.Department.Name
                        },
                        Location = u.Location == null ? null : new Location
                        {
                            LocationId = u.Location.LocationId,
                            Name = u.Location.Name
                        }
                    })
                    .ToListAsync();
            }

            return await query
                .Select(u => new User
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Name = u.Name,
                    UserRole = u.UserRole,
                    DepartmentId = u.DepartmentId,
                    LocationId = u.LocationId,
                    EducationBossId = u.EducationBossId,
                })
                .ToListAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<UserDashboardViewModel?> GetUserDashboardById(int id)
        {
            var user = await context.Users
                .Where(x => x.UserId == id)
                .Select(u => new UserDashboardViewModel
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department != null ? u.Department.Name : null,
                    LocationId = u.LocationId,
                    LocationName = u.Location != null ? u.Location.Name : null,
                    EducationBossId = u.EducationBossId,
                    EducationBossName = u.EducationBoss != null ? u.EducationBoss.Name : null,
                    UserRole = u.UserRole,
                    PasswordExpiryDate = u.PasswordExpiryDate,
                    EducationLeaders = u.EducationLeaders.Select(el => new UserViewModel
                    {
                        UserId = el.UserId,
                        Name = el.Name,
                        DepartmentId = el.DepartmentId,
                        DepartmentName = el.Department != null ? el.Department.Name : null,
                        LocationId = el.LocationId,
                        LocationName = el.Location != null ? el.Location.Name : null,
                        UserRole = el.UserRole
                    }).ToList(),
                    EducationalConsultantPersons = u.EducationalConsultantPersons.Select(p => new PersonViewModel
                    {
                        PersonId = p.PersonId,
                        Name = p.Name,
                        Initials = p.Initials,
                        HiringDate = p.HiringDate,
                        EndDate = p.EndDate,
                        SvuEligible = p.SvuEligible,
                        SvuApplied = p.SvuApplied,
                        DepartmentId = p.DepartmentId,
                        DepartmentName = p.Department != null ? p.Department.Name : null,
                        LocationId = p.LocationId,
                        LocationName = p.Location != null ? p.Location.Name : null,
                        CompletedModules = p.PersonCourses.Count(pc => pc.Status == Status.Bestået)
                    }).ToList(),
                    EducationLeaderPersons = u.EducationLeaderPersons.Select(p => new PersonViewModel
                    {
                        PersonId = p.PersonId,
                        Name = p.Name,
                        Initials = p.Initials,
                        HiringDate = p.HiringDate,
                        EndDate = p.EndDate,
                        SvuEligible = p.SvuEligible,
                        SvuApplied = p.SvuApplied,
                        DepartmentId = p.DepartmentId,
                        DepartmentName = p.Department != null ? p.Department.Name : null,
                        LocationId = p.LocationId,
                        LocationName = p.Location != null ? p.Location.Name : null,
                        CompletedModules = p.PersonCourses.Count(pc => pc.Status == Status.Bestået)
                    }).ToList(),
                    OperationCoordinatorPersons = u.OperationCoordinatorPersons.Select(p => new PersonViewModel
                    {
                        PersonId = p.PersonId,
                        Name = p.Name,
                        Initials = p.Initials,
                        HiringDate = p.HiringDate,
                        EndDate = p.EndDate,
                        SvuEligible = p.SvuEligible,
                        SvuApplied = p.SvuApplied,
                        DepartmentId = p.DepartmentId,
                        DepartmentName = p.Department != null ? p.Department.Name : null,
                        LocationId = p.LocationId,
                        LocationName = p.Location != null ? p.Location.Name : null,
                        CompletedModules = p.PersonCourses.Count(pc => pc.Status == Status.Bestået)
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return user;
        }


        public async Task<User> GetUserByUsername(string username)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<List<User>> GetUserByName(string name)
        {
            return await context.Users.Where(x => x.Name.Contains(name.ToLower())).ToListAsync();
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        }

        public async Task<User> AddUser(User addRequest)
        {
            context.Users.Add(addRequest);
            await context.SaveChangesAsync();
            return addRequest;
        }


        public async Task<bool> ReassignUser(ReassignUserViewModel model)
        {
            var personsToUpdate = await context.Persons
                .Where(p => p.EducationalConsultantId == model.DeletedUserId
                            || p.EducationalLeaderId == model.DeletedUserId
                            || p.OperationCoordinatorId == model.DeletedUserId)
                .ToListAsync();

            foreach (var person in personsToUpdate)
            {
                if (person.EducationalConsultantId == model.DeletedUserId)
                {
                    person.EducationalConsultantId = model.NewEducationalConsultantId;
                }

                if (person.EducationalLeaderId == model.DeletedUserId)
                {
                    person.EducationalLeaderId = model.NewEducationalLeaderId;
                }

                if (person.OperationCoordinatorId == model.DeletedUserId)
                {
                    person.OperationCoordinatorId = model.NewOperationCoordinatorId;
                }
            }

            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateUser(User user)
        {
            context.Entry(user).State = EntityState.Modified;
            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await context.Users.FindAsync(id);

            if (user is null)
            {
                return false;
            }

            var personList = await context.Persons.Where(x => x.EducationalConsultantId == id || x.OperationCoordinatorId == id).ToListAsync();
            foreach (var person in personList)
            {
                if (person.EducationalConsultantId == id)
                {
                    person.EducationalConsultantId = null;
                }
                if (person.OperationCoordinatorId == id)
                {
                    person.OperationCoordinatorId = null;
                }
            }

            var educationalLeaders = await context.Users.Where(x => x.EducationBossId == id).ToListAsync();
            foreach (var leader in educationalLeaders)
            {
                leader.EducationBossId = null;
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
