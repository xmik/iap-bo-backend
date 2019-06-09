using System;

namespace BranchOfficeBackend
{
    /// <summary>
    /// Accessible from a db
    /// </summary>
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        // needed for authentication
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsManager { get; set; }

        public override string ToString()
        {
            return String.Format("Id: {0}, Name: {1}, Email: {2}, DateOfBirth {3}, IsManager {4}",
                this.EmployeeId, this.Name, this.Email, this.DateOfBirth, this.IsManager);
        }
    }
}
