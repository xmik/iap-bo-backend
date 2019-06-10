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

        public override string ToString()
        {
            return String.Format("Id: {0}, EmployeeId: {1}, Value: {2}, TimePeriod: {3}",
                this.Id, this.EmployeeId, this.Value, this.TimePeriod);
        }
    }
}
