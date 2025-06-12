using DEP.Repository.Context;
using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace DEP.Repository.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly DatabaseContext context;
        public CourseRepository(DatabaseContext context) { this.context = context; }

        public async Task<bool> AddCourse(Course course)
        {
            context.Courses.Add(course);
            var result = await context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> DeleteCourse(int id)
        {
            var course = await context.Courses.FindAsync(id);

            if (course is null)
            {
                return false;
            }

            context.Courses.Remove(course);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<Course>> GetAllCourses()
        {
            var tempList = await context.Courses
                .Select(x => new
                {
                    x.CourseId,
                    x.CourseNumber,
                    x.ModuleId,
                    x.StartDate,
                    x.EndDate,
                    x.CourseType,
                    PersonCourses = x.PersonCourses.Select(pc => new
                    {
                        pc.Status,
                        pc.CourseId,
                        pc.PersonId,
                        Person = new
                        {
                            pc.Person.Name
                        }
                    }),
                    Module = new
                    {
                        x.Module.ModuleId,
                        x.Module.Name,
                        x.Module.Description
                    }
                }).ToListAsync();

            var courses = new List<Course>();
            var personCourses = new List<PersonCourse>();
            foreach (var item in tempList)
            {
                // Making a new instance of list per iteration otherwise all courses will share the same list of PersonCourse.
                personCourses = new List<PersonCourse>();

                foreach (var x in item.PersonCourses)
                {
                    personCourses.Add(new PersonCourse()
                    {
                        PersonId = x.PersonId,
                        CourseId = x.CourseId,
                        Status = x.Status,
                        Person = new Person()
                        {
                            Name = x.Person.Name,
                        },
                    });
                }
                courses.Add(new Course()
                {
                    CourseId = item.CourseId,
                    CourseNumber = item.CourseNumber,
                    ModuleId = item.ModuleId,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    CourseType = item.CourseType,
                    PersonCourses = personCourses,
                    Module = new Module()
                    {
                        ModuleId = item.ModuleId,
                        Name = item.Module.Name,
                        Description = item.Module.Description,
                    }
                });
            }

            return courses;
        }

        public async Task<List<Course>> GetCoursesByModuleId(int moduleId)
        {
            return await context.Courses
                .Where(c => c.ModuleId == moduleId)
                .OrderBy(c => c.StartDate)
                .ThenBy(c => c.EndDate)
                .Select(c => new Course
                {
                    CourseId = c.CourseId,
                    CourseNumber = c.CourseNumber,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    CourseType = c.CourseType,
                    ModuleId = c.ModuleId,
                    PersonCourses = c.PersonCourses.Select(pc => new PersonCourse
                    {
                        PersonId = pc.PersonId,
                        CourseId = pc.CourseId,
                        Status = pc.Status
                    }).ToList()
                }).OrderBy(x => x.StartDate).ThenBy(x => x.EndDate).ToListAsync();
        }



        public async Task<Course> GetCourseByIdSimple(int id)
        {
            return await context.Courses.FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task<List<Course>> GetCoursesByModuleIdAndUserId(int moduleId, int userId)
        {

            var courses = new List<Course>();
            courses = await context.Courses
                .Where(c => c.ModuleId == moduleId)
                .OrderBy(c => c.StartDate)
                .ThenBy(c => c.EndDate)
                .Select(c => new Course
                {
                    CourseId = c.CourseId,
                    CourseNumber = c.CourseNumber,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    CourseType = c.CourseType,
                    ModuleId = c.ModuleId,
                    PersonCourses = c.PersonCourses.Select(pc => new PersonCourse
                    {
                        CourseId = pc.CourseId,
                        PersonId = pc.PersonId,
                        Status = pc.Status
                    }).ToList()
                })
                .OrderBy(x => x.StartDate).ThenBy(x => x.EndDate)
                .ToListAsync();


            var newCourses = new List<Course>();

            foreach (var course in courses)
            {
                bool any = course.PersonCourses.Any(x => x.PersonId == userId);

                if (!any)
                {
                    newCourses.Add(course);
                }
            }

            return newCourses;
        }

        public Task<Course> GetCourseById(int id)
        {
            var course = context.Courses
                .Where(x => x.CourseId == id)
                .Select(x => new Course
                {
                    CourseId = x.CourseId,
                    CourseNumber = x.CourseNumber,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    CourseType = x.CourseType,
                    ModuleId = x.ModuleId,
                    Module = new Module
                    {
                        ModuleId = x.Module.ModuleId,
                        Name = x.Module.Name
                    },
                    PersonCourses = x.PersonCourses.Select(pc => new PersonCourse
                    {
                        PersonId = pc.PersonId,
                        CourseId = pc.CourseId,
                        Status = pc.Status,
                        Person = pc.Person == null ? null : new Person
                        {
                            PersonId = pc.Person.PersonId,
                            Name = pc.Person.Name,
                            Initials = pc.Person.Initials,
                            EndDate = pc.Person.EndDate,
                            Department = pc.Person.Department == null ? null : new Department
                            {
                                DepartmentId = pc.Person.Department.DepartmentId,
                                Name = pc.Person.Department.Name
                            },
                            Location = pc.Person.Location == null ? null : new Location
                            {
                                LocationId = pc.Person.Location.LocationId,
                                Name = pc.Person.Location.Name
                            }
                        }
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return course;
        }



        public async Task<bool> UpdateCourse(Course course)
        {
            context.Entry(course).State = EntityState.Modified;
            var result = await context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<List<Course>> GetCourseWithPerson(int moduleId)
        {
            return await context.Courses
                .Where(c => c.ModuleId == moduleId)
                .Select(c => new Course
                {
                    CourseId = c.CourseId,
                    CourseNumber = c.CourseNumber,
                    ModuleId = c.ModuleId,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    CourseType = c.CourseType,
                    PersonCourses = c.PersonCourses.Select(pc => new PersonCourse
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
                            SvuApplied = pc.Person.SvuApplied,
                            Department = pc.Person.Department == null ? null : new Department
                            {
                                DepartmentId = pc.Person.Department.DepartmentId,
                                Name = pc.Person.Department.Name
                            },
                            Location = pc.Person.Location == null ? null : new Location
                            {
                                LocationId = pc.Person.Location.LocationId,
                                Name = pc.Person.Location.Name
                            }
                        }
                    }).ToList()
                }).ToListAsync();
        }
    }
}
