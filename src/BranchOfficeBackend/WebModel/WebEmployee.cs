using System;

namespace BranchOfficeBackend
{
    /// <summary>
    /// Accessible through API server
    /// </summary>
    public class WebEmployee
    {
        /// <summary>
        /// May be needed for deserialization
        /// </summary>
        public WebEmployee()
        {

        }
        public WebEmployee(Employee e)
        {
            this.Name = e.Name;
            this.Email = e.Email;
            this.ID = e.EmployeeId;
            this.IsManager = e.IsManager;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public int ID { get; set; }
        public bool IsManager { get; set; }

        public override string ToString()
        {
            return String.Format("Id: {0}, Name: {1}, Email: {2}, IsManager: {3}",
                this.ID, this.Name, this.Email, this.IsManager);
        }
    }
}
