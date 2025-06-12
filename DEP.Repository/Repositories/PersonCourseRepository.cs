using DEP.Repository.Context;
using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace DEP.Repository.Repositories
{
    public class PersonCourseRepository : IPersonCourseRepository
    {
        private readonly DatabaseContext context;
        public PersonCourseRepository(DatabaseContext context)
        {
                this.context = context;
        }

        public async Task<bool> AddPersonCourse(PersonCourse personCourse)
        {
            context.PersonCourses.Add(personCourse);
            var result = await context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> DeletePersonCourse(int personId, int courseId)
        {
            var personCourse = await context.PersonCourses
                .FirstOrDefaultAsync(x => x.PersonId == personId && x.CourseId == courseId);

            if (personCourse is not null)
            {
                context.PersonCourses.Remove(personCourse);
                int changes = await context.SaveChangesAsync();

                if (changes > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<List<PersonCourse>> GetAllPersonCourses()
        {
            return await context.PersonCourses
                .Select(pc => new PersonCourse
                {
                    PersonId = pc.PersonId,
                    CourseId = pc.CourseId,
                    Status = pc.Status,

                    Person = pc.Person == null ? null : new Person
                    {
                        PersonId = pc.Person.PersonId,
                        Name = pc.Person.Name,
                        Initials = pc.Person.Initials,
                        DepartmentId = pc.Person.DepartmentId,
                        LocationId = pc.Person.LocationId,
                        EducationalConsultantId = pc.Person.EducationalConsultantId,
                        EducationalLeaderId = pc.Person.EducationalLeaderId,
                        OperationCoordinatorId = pc.Person.OperationCoordinatorId,
                        HiringDate = pc.Person.HiringDate,
                        EndDate = pc.Person.EndDate,
                        SvuEligible = pc.Person.SvuEligible,
                        SvuApplied = pc.Person.SvuApplied
                    },

                    Course = pc.Course == null ? null : new Course
                    {
                        CourseId = pc.Course.CourseId,
                        CourseNumber = pc.Course.CourseNumber,
                        ModuleId = pc.Course.ModuleId,
                        StartDate = pc.Course.StartDate,
                        EndDate = pc.Course.EndDate,
                        CourseType = pc.Course.CourseType
                    }
                })
                .ToListAsync();
        }

        public async Task<List<PersonCourse>> GetPersonCoursesByCourse(int courseId)
        {
            return await context.PersonCourses
                .Where(x => x.CourseId == courseId)
                .Select(pc => new PersonCourse
                {
                    PersonId = pc.PersonId,
                    CourseId = pc.CourseId,
                    Status = pc.Status,

                    Person = pc.Person == null ? null : new Person
                    {
                        PersonId = pc.Person.PersonId,
                        Name = pc.Person.Name,
                        Initials = pc.Person.Initials,
                        DepartmentId = pc.Person.DepartmentId,
                        LocationId = pc.Person.LocationId,
                        EducationalConsultantId = pc.Person.EducationalConsultantId,
                        EducationalLeaderId = pc.Person.EducationalLeaderId,
                        OperationCoordinatorId = pc.Person.OperationCoordinatorId,
                        HiringDate = pc.Person.HiringDate,
                        EndDate = pc.Person.EndDate,
                        SvuEligible = pc.Person.SvuEligible,
                        SvuApplied = pc.Person.SvuApplied
                    },

                    Course = pc.Course == null ? null : new Course
                    {
                        CourseId = pc.Course.CourseId,
                        CourseNumber = pc.Course.CourseNumber,
                        ModuleId = pc.Course.ModuleId,
                        StartDate = pc.Course.StartDate,
                        EndDate = pc.Course.EndDate,
                        CourseType = pc.Course.CourseType
                    }
                })
                .ToListAsync();
        }

        public async Task<List<PersonCourse>> GetPersonCoursesByPerson(int personId)
        {
            return await context.PersonCourses
                .Where(x => x.PersonId == personId)
                .Select(pc => new PersonCourse
                {
                    PersonId = pc.PersonId,
                    CourseId = pc.CourseId,
                    Status = pc.Status,

                    Person = pc.Person == null ? null : new Person
                    {
                        PersonId = pc.Person.PersonId,
                        Name = pc.Person.Name,
                        Initials = pc.Person.Initials,
                        DepartmentId = pc.Person.DepartmentId,
                        LocationId = pc.Person.LocationId,
                        EducationalConsultantId = pc.Person.EducationalConsultantId,
                        EducationalLeaderId = pc.Person.EducationalLeaderId,
                        OperationCoordinatorId = pc.Person.OperationCoordinatorId,
                        HiringDate = pc.Person.HiringDate,
                        EndDate = pc.Person.EndDate,
                        SvuEligible = pc.Person.SvuEligible,
                        SvuApplied = pc.Person.SvuApplied
                    },
                    Course = pc.Course == null ? null : new Course
                    {
                        CourseId = pc.Course.CourseId,
                        CourseNumber = pc.Course.CourseNumber,
                        ModuleId = pc.Course.ModuleId,
                        StartDate = pc.Course.StartDate,
                        EndDate = pc.Course.EndDate,
                        CourseType = pc.Course.CourseType,

                        Module = pc.Course.Module == null ? null : new Module
                        {
                            ModuleId = pc.Course.Module.ModuleId,
                            Name = pc.Course.Module.Name,
                            Description = pc.Course.Module.Description
                        },
                    },
                })
                .ToListAsync();
        }

        public async Task<List<PersonCourse>> GetPersonCoursesByModule(int moduleId)
        {
            return await context.PersonCourses
                .Where(x => x.Course.ModuleId == moduleId)
                .ToListAsync();
        }

        public async Task<PersonCourse> UpdatePersonCourse(PersonCourse personCourse)
        {
            context.Entry(personCourse).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return personCourse;
        }
    }
}
