using System;
using System.ComponentModel.DataAnnotations;

namespace BranchOfficeBackend
{
     public class Salary
    {
        [Key]
        public int SalaryId { get; set; }
        public float Value { get; set; }
        public string TimePeriod { get; set; }

        public int EmployeeId { get; set; }

        public Salary()
        {
            SalaryId = -1;
            EmployeeId = -1;
            Value = -1;
        }

        public Salary(Salary other, int v)
        {
            this.SalaryId = v;
            this.EmployeeId = other.EmployeeId;
            this.Value = other.Value;
            this.TimePeriod = other.TimePeriod;
        }

        public override string ToString()
        {
            return String.Format("SalaryId: {0}, Value: {1}, TimePeriod: {2}, EmployeeId {3}",
                this.SalaryId, this.Value, this.TimePeriod, this.EmployeeId);
        }
    }
}
