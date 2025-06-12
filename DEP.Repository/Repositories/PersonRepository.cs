using DEP.Repository.Context;
using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using Microsoft.EntityFrameworkCore;
using File = DEP.Repository.Models.File;
using System.Reflection.Metadata.Ecma335;

namespace DEP.Repository.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly DatabaseContext context;

        public PersonRepository(DatabaseContext context) { this.context = context; }

        public async Task<Person?> AddPerson(Person person)
        {
            context.Persons.Add(person);
            var result = await context.SaveChangesAsync();
            if (result > 0)
            {
                return person;
            }

            return null;
        }

        public async Task<List<Person>> GetPersons()
        {
            return await context.Persons
                .Select(p => new Person
                {
                    PersonId = p.PersonId,
                    Name = p.Name,
                    Initials = p.Initials,
                    DepartmentId = p.DepartmentId,
                    LocationId = p.LocationId,
                    EducationalConsultantId = p.EducationalConsultantId,
                    EducationalLeaderId = p.EducationalLeaderId,
                    OperationCoordinatorId = p.OperationCoordinatorId,
                    HiringDate = p.HiringDate,
                    EndDate = p.EndDate,
                    SvuEligible = p.SvuEligible,
                    SvuApplied = p.SvuApplied,

                    Department = p.Department == null ? null : new Department
                    {
                        DepartmentId = p.Department.DepartmentId,
                        Name = p.Department.Name
                    },

                    Location = p.Location == null ? null : new Location
                    {
                        LocationId = p.Location.LocationId,
                        Name = p.Location.Name
                    },

                    EducationalConsultant = p.EducationalConsultant == null ? null : new User
                    {
                        UserId = p.EducationalConsultant.UserId,
                        Name = p.EducationalConsultant.Name,
                        UserName = p.EducationalConsultant.UserName,
                        UserRole = p.EducationalConsultant.UserRole
                    },

                    EducationalLeader = p.EducationalLeader == null ? null : new User
                    {
                        UserId = p.EducationalLeader.UserId,
                        Name = p.EducationalLeader.Name,
                        UserName = p.EducationalLeader.UserName,
                        UserRole = p.EducationalLeader.UserRole
                    },

                    OperationCoordinator = p.OperationCoordinator == null ? null : new User
                    {
                        UserId = p.OperationCoordinator.UserId,
                        Name = p.OperationCoordinator.Name,
                        UserName = p.OperationCoordinator.UserName,
                        UserRole = p.OperationCoordinator.UserRole
                    },

                    PersonCourses = p.PersonCourses.Select(pc => new PersonCourse
                    {
                        CourseId = pc.CourseId,
                        PersonId = pc.PersonId,
                        Status = pc.Status
                    }).ToList()
                })
                .ToListAsync();
        }


        public async Task<bool> UpdatePerson(Person person)
        {
            context.Entry(person).State = EntityState.Modified;
            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        public async Task UpdatePersons(List<Person> persons)
        {
            context.Persons.UpdateRange(persons);
            await context.SaveChangesAsync();
        }


        public async Task<bool> DeletePerson(int id)
        {
            var person = await context.Persons.FindAsync(id);

            if (person is null)
            {
                return false;
            }

            context.Persons.Remove(person);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<Person>> GetPersonsByName(string name)
        {
            name = name.ToLower();

            return await context.Persons
                .Where(x => x.Name.ToLower().Contains(name) || x.Initials.ToLower().Contains(name))
                .OrderBy(x => x.Name)
                .Select(x => new Person
                {
                    PersonId = x.PersonId,
                    Name = x.Name,
                    Initials = x.Initials,
                    DepartmentId = x.DepartmentId,
                    LocationId = x.LocationId,
                    EducationalConsultantId = x.EducationalConsultantId,
                    EducationalLeaderId = x.EducationalLeaderId,
                    OperationCoordinatorId = x.OperationCoordinatorId,
                    HiringDate = x.HiringDate,
                    EndDate = x.EndDate,
                    SvuEligible = x.SvuEligible,
                    SvuApplied = x.SvuApplied,

                    Department = x.Department == null ? null : new Department
                    {
                        DepartmentId = x.Department.DepartmentId,
                        Name = x.Department.Name
                    },

                    Location = x.Location == null ? null : new Location
                    {
                        LocationId = x.Location.LocationId,
                        Name = x.Location.Name
                    },

                    EducationalConsultant = x.EducationalConsultant == null ? null : new  User
                    {
                        UserId = x.EducationalConsultant.UserId,
                        Name = x.EducationalConsultant.Name,
                        UserName = x.EducationalConsultant.UserName,
                        UserRole = x.EducationalConsultant.UserRole
                    },

                    OperationCoordinator = x.OperationCoordinator == null ? null : new User
                    {
                        UserId = x.OperationCoordinator.UserId,
                        Name = x.OperationCoordinator.Name,
                        UserName = x.OperationCoordinator.UserName,
                        UserRole = x.OperationCoordinator.UserRole
                    },

                    Files = x.Files.Select(f => new File
                    {
                        FileId = f.FileId,
                        FileName = f.FileName,
                        FilePath = f.FilePath,
                        FileFormat = f.FileFormat,
                        ContentType = f.ContentType,
                        UploadDate = f.UploadDate,
                        FileTagId = f.FileTagId,
                        PersonId = f.PersonId,

                        FileTag = f.FileTag == null ? null : new FileTag
                        {
                            FileTagId = f.FileTag.FileTagId,
                            TagName = f.FileTag.TagName,
                            FileTagUserRoles = f.FileTag.FileTagUserRoles.Select(r => new FileTagUserRole
                            {
                                FileTagId = r.FileTagId,
                                Role = r.Role
                            }).ToList()
                        }
                    }).ToList(),

                    PersonCourses = x.PersonCourses.Select(pc => new PersonCourse
                    {
                        CourseId = pc.CourseId,
                        PersonId = pc.PersonId,
                        Status = pc.Status,

                        Course = new Course
                        {
                            CourseId = pc.Course.CourseId,
                            CourseNumber = pc.Course.CourseNumber,
                            ModuleId = pc.Course.ModuleId,
                            StartDate = pc.Course.StartDate,
                            EndDate = pc.Course.EndDate,

                            Module = new Module
                            {
                                ModuleId = pc.Course.Module.ModuleId,
                                Name = pc.Course.Module.Name,
                                Description = pc.Course.Module.Description
                            }
                        }
                    }).ToList()
                })
                .ToListAsync();
        }


        public async Task<Person?> GetPersonById(int personId)
        {
            var person = await context.Persons
                .Where(x => x.PersonId == personId)
                .Select(x => new Person
                {
                    PersonId = x.PersonId,
                    Name = x.Name,
                    Initials = x.Initials,
                    DepartmentId = x.DepartmentId,
                    LocationId = x.LocationId,
                    EducationalConsultantId = x.EducationalConsultantId,
                    EducationalLeaderId = x.EducationalLeaderId,
                    OperationCoordinatorId = x.OperationCoordinatorId,
                    HiringDate = x.HiringDate,
                    EndDate = x.EndDate,
                    SvuEligible = x.SvuEligible,
                    SvuApplied = x.SvuApplied,

                    Location = x.Location == null ? null : new Location
                    {
                        LocationId = x.Location.LocationId,
                        Name = x.Location.Name
                    },
                    Department = x.Department == null ? null : new Department
                    {
                        DepartmentId = x.Department.DepartmentId,
                        Name = x.Department.Name
                    },
                    Files = x.Files.Select(f => new File
                    {
                        FileId = f.FileId,
                        FileName = f.FileName,
                        FilePath = f.FilePath,
                        FileFormat = f.FileFormat,
                        ContentType = f.ContentType,
                        UploadDate = f.UploadDate,
                        FileTagId = f.FileTagId,
                        PersonId = f.PersonId,
                        FileTag = f.FileTag == null ? null : new FileTag
                        {
                            FileTagId = f.FileTag.FileTagId,
                            TagName = f.FileTag.TagName,
                            FileTagUserRoles = f.FileTag.FileTagUserRoles.Select(r => new FileTagUserRole
                            {
                                FileTagId = r.FileTagId,
                                Role = r.Role
                            }).ToList()
                        }
                    }).ToList(),
                    PersonCourses = x.PersonCourses.Select(pc => new PersonCourse
                    {
                        CourseId = pc.CourseId,
                        PersonId = pc.PersonId,
                        Status = pc.Status,
                        Course = pc.Course == null ? null : new Course
                        {
                            CourseId = pc.Course.CourseId,
                            CourseNumber = pc.Course.CourseNumber,
                            ModuleId = pc.Course.ModuleId,
                            StartDate = pc.Course.StartDate,
                            EndDate = pc.Course.EndDate,
                            Module = pc.Course.Module == null ? null : new Module
                            {
                                ModuleId = pc.Course.Module.ModuleId,
                                Name = pc.Course.Module.Name,
                                Description = pc.Course.Module.Description
                            }
                        }
                    }).ToList(),
                    EducationalConsultant = x.EducationalConsultant == null ? null : new User
                    {
                        UserId = x.EducationalConsultant.UserId,
                        Name = x.EducationalConsultant.Name,
                        UserName = x.EducationalConsultant.UserName,
                        UserRole = x.EducationalConsultant.UserRole
                    },
                    EducationalLeader = x.EducationalLeader == null ? null : new User
                    {
                        UserId = x.EducationalLeader.UserId,
                        Name = x.EducationalLeader.Name,
                        UserName = x.EducationalLeader.UserName,
                        UserRole = x.EducationalLeader.UserRole
                    },
                    OperationCoordinator = x.OperationCoordinator == null ? null : new User
                    {
                        UserId = x.OperationCoordinator.UserId,
                        Name = x.OperationCoordinator.Name,
                        UserName = x.OperationCoordinator.UserName,
                        UserRole = x.OperationCoordinator.UserRole
                    }
                })
                .FirstOrDefaultAsync();

            return person;
        }


        public async Task<List<Person>> GetPersonsByUserId(int userId)
        {
            return await context.Persons
                .Where(p => p.EducationalConsultantId == userId ||
                            p.EducationalLeaderId == userId ||
                            p.OperationCoordinatorId == userId)
                .ToListAsync();
        }

        public async Task<List<Person>> GetPersonsByCourseId(int courseId)
        {
            var persons = await context.Persons
                .Where(p => p.PersonCourses.Any(pc => pc.CourseId == courseId))
                .Select(p => new Person
                {
                    PersonId = p.PersonId,
                    Name = p.Name,
                    Initials = p.Initials,
                    DepartmentId = p.DepartmentId,
                    LocationId = p.LocationId,
                    EducationalConsultantId = p.EducationalConsultantId,
                    EducationalLeaderId = p.EducationalLeaderId,
                    OperationCoordinatorId = p.OperationCoordinatorId,
                    HiringDate = p.HiringDate,
                    EndDate = p.EndDate,
                    SvuEligible = p.SvuEligible,
                    SvuApplied = p.SvuApplied,

                    Department = p.Department,
                    Location = p.Location,

                    PersonCourses = p.PersonCourses
                        .Where(pc => pc.CourseId == courseId)
                        .Select(pc => new PersonCourse
                        {
                            PersonId = pc.PersonId,
                            CourseId = pc.CourseId,
                            Status = pc.Status,
                        }).ToList(),

                    EducationalConsultant = p.EducationalConsultant,
                    EducationalLeader = p.EducationalLeader,
                    OperationCoordinator = p.OperationCoordinator,
                })
                .ToListAsync();

            return persons;
        }

        public async Task<List<Person>> GetPersonsNotInCourse(int courseId)
        {
            var persons = await context.Persons
                .Where(p => !p.PersonCourses.Any(pc => pc.CourseId == courseId))
                .Select(p => new Person
                {
                    PersonId = p.PersonId,
                    Name = p.Name,
                    Initials = p.Initials,
                    DepartmentId = p.DepartmentId,
                    LocationId = p.LocationId,
                    HiringDate = p.HiringDate,
                    EndDate = p.EndDate,
                    SvuEligible = p.SvuEligible,
                    SvuApplied = p.SvuApplied,

                    Department = p.Department == null ? null : new Department
                    {
                        DepartmentId = p.Department.DepartmentId,
                        Name = p.Department.Name
                    },
                    Location = p.Location == null ? null : new Location
                    {
                        LocationId = p.Location.LocationId,
                        Name = p.Location.Name
                    },
                    PersonCourses = p.PersonCourses
                        .Select(pc => new PersonCourse
                        {
                            PersonId = pc.PersonId,
                            CourseId = pc.CourseId,
                            Status = pc.Status
                        }).ToList()
                })
                .ToListAsync();

            return persons;
        }

        public async Task<List<Person>> GetPersonsByDepartmentAndLocation(int departmentId, int locationId)
        {
            return await context.Persons
                .Where(p => p.DepartmentId == departmentId && p.LocationId == locationId)
                .Select(p => new Person
                {
                    PersonId = p.PersonId,
                    Name = p.Name,
                    Initials = p.Initials,
                    DepartmentId = p.DepartmentId,
                    LocationId = p.LocationId,
                    HiringDate = p.HiringDate,
                    EndDate = p.EndDate,
                    SvuEligible = p.SvuEligible,
                    SvuApplied = p.SvuApplied,
                    Department = p.Department == null ? null : new Department
                    {
                        DepartmentId = p.Department.DepartmentId,
                        Name = p.Department.Name
                    },
                    Location = p.Location == null ? null : new Location
                    {
                        LocationId = p.Location.LocationId,
                        Name = p.Location.Name
                    }
                })
                .ToListAsync();
        }

        public async Task<List<Person>> GetPersonsExcel(int leaderId)
        {
            return await context.Persons
                .Where(p => p.EducationalLeaderId == leaderId)
                .Select(p => new Person
                {
                    PersonId = p.PersonId,
                    Name = p.Name,
                    Initials = p.Initials,
                    DepartmentId = p.DepartmentId,
                    LocationId = p.LocationId,
                    HiringDate = p.HiringDate,
                    EndDate = p.EndDate,
                    SvuEligible = p.SvuEligible,
                    SvuApplied = p.SvuApplied,
                    Department = p.Department == null ? null : new Department
                    {
                        DepartmentId = p.Department.DepartmentId,
                        Name = p.Department.Name
                    },
                    Location = p.Location == null ? null : new Location
                    {
                        LocationId = p.Location.LocationId,
                        Name = p.Location.Name
                    },
                    PersonCourses = p.PersonCourses.Select(pc => new PersonCourse
                    {
                        PersonId = pc.PersonId,
                        CourseId = pc.CourseId,
                        Status = pc.Status,
                        Course = pc.Course == null ? null : new Course
                        {
                            CourseId = pc.Course.CourseId,
                            CourseNumber = pc.Course.CourseNumber,
                            StartDate = pc.Course.StartDate,
                            EndDate = pc.Course.EndDate,
                            CourseType = pc.Course.CourseType,
                            ModuleId = pc.Course.ModuleId,
                            Module = pc.Course.Module == null ? null : new Module
                            {
                                ModuleId = pc.Course.Module.ModuleId,
                                Name = pc.Course.Module.Name
                            }
                        }
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<Person?> GetPersonExcel(int id)
        {
            return await context.Persons
                .Where(p => p.PersonId == id)
                .Select(p => new Person
                {
                    PersonId = p.PersonId,
                    Name = p.Name,
                    Initials = p.Initials,
                    DepartmentId = p.DepartmentId,
                    LocationId = p.LocationId,
                    HiringDate = p.HiringDate,
                    EndDate = p.EndDate,
                    SvuEligible = p.SvuEligible,
                    SvuApplied = p.SvuApplied,
                    Department = p.Department == null ? null : new Department
                    {
                        DepartmentId = p.Department.DepartmentId,
                        Name = p.Department.Name
                    },
                    Location = p.Location == null ? null : new Location
                    {
                        LocationId = p.Location.LocationId,
                        Name = p.Location.Name
                    },
                    PersonCourses = p.PersonCourses.Select(pc => new PersonCourse
                    {
                        PersonId = pc.PersonId,
                        CourseId = pc.CourseId,
                        Status = pc.Status,
                        Course = pc.Course == null ? null : new Course
                        {
                            CourseId = pc.Course.CourseId,
                            CourseNumber = pc.Course.CourseNumber,
                            StartDate = pc.Course.StartDate,
                            EndDate = pc.Course.EndDate,
                            CourseType = pc.Course.CourseType,
                            ModuleId = pc.Course.ModuleId,
                            Module = pc.Course.Module == null ? null : new Module
                            {
                                ModuleId = pc.Course.Module.ModuleId,
                                Name = pc.Course.Module.Name
                            }
                        }
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<Person>> GetPersonsByModuleId(int moduleId)
        {
            return await context.Persons
                .Where(p => p.PersonCourses.Any(pc => pc.Course.ModuleId == moduleId))
                .Distinct()
                .ToListAsync();
        }
    }
}
