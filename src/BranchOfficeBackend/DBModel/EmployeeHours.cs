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
        public double Value { get; set; }
        public string TimePeriod { get; set; }

        public int EmployeeId { get; set; }

        public override string ToString()
        {
            return String.Format("Id: {0}, Value: {1}, TimePeriod: {2}, EmployeeId {3}",
                this.EmployeeHoursId, this.Value, this.TimePeriod, this.EmployeeId);
        }
    }
}
