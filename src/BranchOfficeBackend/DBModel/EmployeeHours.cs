using System;
using System.ComponentModel.DataAnnotations;

namespace BranchOfficeBackend
{
     public class EmployeeHours
    {
        // The entity type 'DateTimeRange' requires a primary key to be defined.
        // https://stackoverflow.com/questions/48225989/the-entity-type-requires-a-primary-key-to-be-defined
        [Key]
        public int EmployeeHoursId { get; set; }
        /// <summary>
        /// How many hours an employee worked within a given TimePeriod
        /// </summary>
        /// <value></value>
        public float Value { get; set; }
        public string TimePeriod { get; set; }

        public int EmployeeId { get; set; }

        public EmployeeHours()
        {
            EmployeeHoursId = -1;
            EmployeeId = -1;
            Value = -1;
        }

        public EmployeeHours(WebEmployeeHours webEmployeeHours)
        {
            this.Value = webEmployeeHours.Value;
            this.TimePeriod = webEmployeeHours.TimePeriod;
            this.EmployeeId = webEmployeeHours.EmployeeId;
            this.EmployeeHoursId = webEmployeeHours.Id;
        }

        public EmployeeHours(EmployeeHours employeeHours, int v)
        {
            this.Value = employeeHours.Value;
            this.TimePeriod = employeeHours.TimePeriod;
            this.EmployeeId = employeeHours.EmployeeId;
            this.EmployeeHoursId = v;
        }

        public override string ToString()
        {
            return String.Format("Id: {0}, Value: {1}, TimePeriod: {2}, EmployeeId {3}",
                this.EmployeeHoursId, this.Value, this.TimePeriod, this.EmployeeId);
        }
    }
}
