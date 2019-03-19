using System;

namespace BranchOfficeBackend
{
     public class EmployeeHours
    {
        public int EmployeeHoursId { get; set; }
        public double Value { get; set; }
        public TimeSpan TimePeriod { get; set; }

        public Employee Employee { get; set; }
        public Project Project { get; set; }

        public override string ToString()
        {
            return String.Format("Id: {0}, Value: {1}, TimePeriod: {2}, EmployeeId {3}, ProjectId {4}",
                this.EmployeeHoursId, this.Value, this.TimePeriod, this.Employee.EmployeeId, this.Project.ProjectId);
        }
    }
}