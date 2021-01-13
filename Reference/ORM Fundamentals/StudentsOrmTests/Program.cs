using System;

namespace StudentsOrmTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new StudentsDbContext();
            dbContext.Database.EnsureCreated();
            dbContext.Courses.Add(new Course {Name = "Entity Framework Core"});
            dbContext.Courses.Add(new Course {Name = "Sql Server"});
            dbContext.SaveChanges();
        }
    }
}
