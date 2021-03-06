using System;
using System.ComponentModel.DataAnnotations;

namespace BranchOfficeBackend
{
    /// <summary>
    /// Accessible from a db
    /// </summary>
    public class Employee
    {
        // https://stackoverflow.com/questions/48225989/the-entity-type-requires-a-primary-key-to-be-defined
        [Key]
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        // needed for authentication
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsManager { get; set; }

        public Employee()
        {
            
        }
        public Employee(Employee e, int newId)
        {
            this.Name = e.Name;
            this.Email = e.Email;
            this.DateOfBirth = e.DateOfBirth;
            this.EmployeeId = newId;
            this.IsManager = e.IsManager;
        }

        public Employee(WebEmployee e)
        {            
            this.Name = e.Name;
            this.Email = e.Email;
            this.EmployeeId = e.ID;
            this.IsManager = e.IsManager;
            if (e.DateOfBirth != null) {
                this.DateOfBirth = DateTime.Parse(e.DateOfBirth);
            }
        }
        public Employee(HQEmployee e)
        {
            this.Name = e.Name;
            this.Email = e.Email;
            // do not preserve this, we'll add our own
            this.EmployeeId = -1;
            this.IsManager = e.IsManager;
            if (e.DateOfBirth != null) {
                this.DateOfBirth = DateTime.Parse(e.DateOfBirth);
            }
        }

        public override string ToString()
        {
            return String.Format("Id: {0}, Name: {1}, Email: {2}, DateOfBirth {3}, IsManager: {4}",
                this.EmployeeId, this.Name, this.Email, this.DateOfBirth, this.IsManager);
        }
    }
}
