using System;
using System.Linq;
using ORM_Fundamentals.Models;

namespace ORM_Fundamentals
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new SoftuniContext();

            //var employees = dbContext.Employees.Where(x => x.Department.Manager.Department.Name == "Sales")
            //    .Select(x => new
            //    {
            //        Name = x.FirstName + ' ' + x.LastName,
            //        DepartmentName = x.Department.Name,
            //        Manager = x.Manager.FirstName + ' ' + x.Manager.LastName
            //    });

            //foreach (var employee in employees)
            //{
            //    Console.WriteLine(employee.Name + " => " + employee.DepartmentName + " => " + employee.Manager);
            //}

            var employeesGroups = dbContext.Employees.GroupBy(x => x.Department.Name)
                .Select(x => new
                {
                    DepartmentName = x.Key,
                    CountEmployees = x.Count(),
                    SalariesSum = x.Sum(e => e.Salary)
                });

            foreach (var employeeGroup in employeesGroups)
            {
                Console.WriteLine(employeeGroup.DepartmentName + " => " + employeeGroup.CountEmployees + " => __ Salaries sum: __" + employeeGroup.SalariesSum);
            }
        }
    }
}
