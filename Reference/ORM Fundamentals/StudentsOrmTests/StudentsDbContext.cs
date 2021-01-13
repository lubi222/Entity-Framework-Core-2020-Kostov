using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Sources;
using Microsoft.EntityFrameworkCore;

namespace StudentsOrmTests
{
    class StudentsDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=GradesDb;Integrated Security=true;");
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Grade> Grades { get; set; }
    }
}
