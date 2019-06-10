using System;

namespace BranchOfficeBackend
{
    public class WebEmployeeHours
    {
        /// <summary>
        /// May be needed for deserialization
        /// </summary>
        public WebEmployeeHours()
        {

        }
        public WebEmployeeHours(EmployeeHours e)
        {
            this.Value = e.Value;
            this.TimePeriod = e.TimePeriod.ToString();
            this.Id = e.EmployeeHoursId;
            this.EmployeeId = e.EmployeeId;
        }

        public float Value { get; set; }
        public string TimePeriod { get; set; }
        public int Id { get; set; }
        public int EmployeeId { get; set; }
    }
}
