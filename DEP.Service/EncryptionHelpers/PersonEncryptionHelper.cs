using System;
using DEP.Repository.Models;
using DEP.Service.Interfaces;
using DEP.Service.ViewModels;

namespace DEP.Service.EncryptionHelpers
{
    public static class PersonEncryptionHelper
    {
        public static void Encrypt(Person person, IEncryptionService encryptionService)
        {
            person.Name = encryptionService.Encrypt(person.Name);
            person.Initials = encryptionService.Encrypt(person.Initials);
        }

        // Basic overload for single person (for GetPersonById etc.)
        public static void Decrypt(Person person, IEncryptionService encryptionService)
        {
            var decryptedDepartments = new HashSet<Department>();
            var decryptedLocations = new HashSet<Location>();
            var decryptedUsers = new HashSet<User>();
            var decryptedPersons = new HashSet<Person>();

            Decrypt(person, encryptionService, decryptedDepartments, decryptedLocations, decryptedUsers, decryptedPersons);
        }

        // For lists
        public static void Decrypt(List<Person> people, IEncryptionService encryptionService)
        {
            var decryptedDepartments = new HashSet<Department>();
            var decryptedLocations = new HashSet<Location>();
            var decryptedUsers = new HashSet<User>();
            var decryptedPersons = new HashSet<Person>();

            foreach (var person in people)
            {
                Decrypt(person, encryptionService, decryptedDepartments, decryptedLocations, decryptedUsers, decryptedPersons);
            }
        }

        // For list of courses with persons
        public static void DecryptModuleCoursesWithPersons(List<ModuleWithCourseViewModel> modules, IEncryptionService encryptionService)
        {
            var decryptedDepartments = new HashSet<Department>();
            var decryptedLocations = new HashSet<Location>();
            var decryptedUsers = new HashSet<User>();
            var decryptedPersons = new HashSet<Person>();


            foreach (var module in modules)
            {
                foreach (var course in module.Courses)
                {
                    foreach (var pc in course.PersonCourses)
                    {
                        if (pc.Person != null && !decryptedPersons.Contains(pc.Person))
                        {
                            if (pc.Person.Name != null)
                                pc.Person.Name = encryptionService.Decrypt(pc.Person.Name);

                            if (pc.Person.Initials != null)
                                pc.Person.Initials = encryptionService.Decrypt(pc.Person.Initials);

                            decryptedPersons.Add(pc.Person);
                        }

                        if (pc.Person.Department != null && !decryptedDepartments.Contains(pc.Person.Department))
                        {
                            DepartmentEncryptionHelper.Decrypt(pc.Person.Department, encryptionService);
                            decryptedDepartments.Add(pc.Person.Department);
                        }

                        if (pc.Person.Location != null && !decryptedLocations.Contains(pc.Person.Location))
                        {
                            LocationEncryptionHelper.Decrypt(pc.Person.Location, encryptionService);
                            decryptedLocations.Add(pc.Person.Location);
                        }

                        if (pc.Person.OperationCoordinator != null && !decryptedUsers.Contains(pc.Person.OperationCoordinator))
                        {
                            UserEncryptionHelper.DecryptUpdatableFields(pc.Person.OperationCoordinator, encryptionService);
                            decryptedUsers.Add(pc.Person.OperationCoordinator);
                        }

                        if (pc.Person.EducationalConsultant != null && !decryptedUsers.Contains(pc.Person.EducationalConsultant))
                        {
                            UserEncryptionHelper.DecryptUpdatableFields(pc.Person.EducationalConsultant, encryptionService);
                            decryptedUsers.Add(pc.Person.EducationalConsultant);
                        }

                        if (pc.Person.EducationalLeader != null && !decryptedUsers.Contains(pc.Person.EducationalLeader))
                        {
                            UserEncryptionHelper.DecryptUpdatableFields(pc.Person.EducationalLeader, encryptionService);
                            decryptedUsers.Add(pc.Person.EducationalLeader);
                        }
                        //if (pc.Person is not null)
                        //{
                        //    //Decrypt(pc.Person, encryptionService, decryptedDepartments, decryptedLocations, decryptedUsers, decryptedPersons);
                        //}
                    }
                }
            }


        }

        public static void DecryptCoursesWithPersons(List<Course> courses, IEncryptionService encryptionService)
        {
            var decryptedDepartments = new HashSet<Department>();
            var decryptedLocations = new HashSet<Location>();
            var decryptedUsers = new HashSet<User>();
            var decryptedPersons = new HashSet<Person>();


                foreach (var course in courses)
                {
                    foreach (var pc in course.PersonCourses)
                    {
                        if (pc.Person != null && !decryptedPersons.Contains(pc.Person))
                        {
                            if (pc.Person.Name != null)
                                pc.Person.Name = encryptionService.Decrypt(pc.Person.Name);

                            if (pc.Person.Initials != null)
                                pc.Person.Initials = encryptionService.Decrypt(pc.Person.Initials);

                            decryptedPersons.Add(pc.Person);
                        }

                        if (pc.Person.Department != null && !decryptedDepartments.Contains(pc.Person.Department))
                        {
                            DepartmentEncryptionHelper.Decrypt(pc.Person.Department, encryptionService);
                            decryptedDepartments.Add(pc.Person.Department);
                        }

                        if (pc.Person.Location != null && !decryptedLocations.Contains(pc.Person.Location))
                        {
                            LocationEncryptionHelper.Decrypt(pc.Person.Location, encryptionService);
                            decryptedLocations.Add(pc.Person.Location);
                        }

                        if (pc.Person.OperationCoordinator != null && !decryptedUsers.Contains(pc.Person.OperationCoordinator))
                        {
                            UserEncryptionHelper.DecryptUpdatableFields(pc.Person.OperationCoordinator, encryptionService);
                            decryptedUsers.Add(pc.Person.OperationCoordinator);
                        }

                        if (pc.Person.EducationalConsultant != null && !decryptedUsers.Contains(pc.Person.EducationalConsultant))
                        {
                            UserEncryptionHelper.DecryptUpdatableFields(pc.Person.EducationalConsultant, encryptionService);
                            decryptedUsers.Add(pc.Person.EducationalConsultant);
                        }

                        if (pc.Person.EducationalLeader != null && !decryptedUsers.Contains(pc.Person.EducationalLeader))
                        {
                            UserEncryptionHelper.DecryptUpdatableFields(pc.Person.EducationalLeader, encryptionService);
                            decryptedUsers.Add(pc.Person.EducationalLeader);
                        }
                    }
                }


        }

        // Internal shared decrypt method
        private static void Decrypt(
            Person person,
            IEncryptionService encryptionService,
            HashSet<Department> decryptedDepartments,
            HashSet<Location> decryptedLocations,
            HashSet<User> decryptedUsers,
            HashSet<Person> decryptedPersons)
        {
            if (person != null && !decryptedPersons.Contains(person))
            {
                if (person.Name != null)
                    person.Name = encryptionService.Decrypt(person.Name);

                if (person.Initials != null)
                    person.Initials = encryptionService.Decrypt(person.Initials);

                decryptedPersons.Add(person);
            }

            if (person.Department != null && !decryptedDepartments.Contains(person.Department))
            {
                DepartmentEncryptionHelper.Decrypt(person.Department, encryptionService);
                decryptedDepartments.Add(person.Department);
            }

            if (person.Location != null && !decryptedLocations.Contains(person.Location))
            {
                LocationEncryptionHelper.Decrypt(person.Location, encryptionService);
                decryptedLocations.Add(person.Location);
            }

            if (person.OperationCoordinator != null && !decryptedUsers.Contains(person.OperationCoordinator))
            {
                UserEncryptionHelper.DecryptUpdatableFields(person.OperationCoordinator, encryptionService);
                decryptedUsers.Add(person.OperationCoordinator);
            }

            if (person.EducationalConsultant != null && !decryptedUsers.Contains(person.EducationalConsultant))
            {
                UserEncryptionHelper.DecryptUpdatableFields(person.EducationalConsultant, encryptionService);
                decryptedUsers.Add(person.EducationalConsultant);
            }

            if (person.EducationalLeader != null && !decryptedUsers.Contains(person.EducationalLeader))
            {
                UserEncryptionHelper.DecryptUpdatableFields(person.EducationalLeader, encryptionService);
                decryptedUsers.Add(person.EducationalLeader);
            }
        }
    }
}
