using System;
using System.Collections.Generic;

#nullable disable

namespace ORM_Fundamentals.Models
{
    public partial class VSomeEmployeesQuery
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime HireDate { get; set; }
        public int? EmployeesWithSameName { get; set; }
    }
}
