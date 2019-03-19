using System;

namespace BranchOfficeBackend
{
    public class Employee
    {
        // TODO: can we create a constructor so that Project entity gets assigned from Employee entity?
        // will the ORM autoincrement the id? 

        public int EmployeeId { get; set; }
        public string Name { get; set; }
        // needed for authentication
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }

        // Establish a one-to-many relationship where many different
        // Employee entities are linked with one Project entity.
        // This uses Convention 1 from:
        // http://www.entityframeworktutorial.net/efcore/one-to-many-conventions-entity-framework-core.aspx
        public Project Project {get; set;}

        public override string ToString()
        {
            string projectId;
            if (this.Project != null)
                projectId = this.Project.ProjectId.ToString();
            else
                projectId = "-1";

            return String.Format("Id: {0}, Name: {1}, Email: {2}, DateOfBirth {3}, Project.ProjectId {4}",
                this.EmployeeId, this.Name, this.Email, this.DateOfBirth, projectId);
        }
    }
}