namespace BranchOfficeBackend
{
    /// <summary>
    /// Accessible through API server
    /// </summary>
    public class WebSalary
    {
        public WebSalary()
        {

        }
        public WebSalary(Salary e)
        {
            this.EmployeeId = e.EmployeeId;
            this.TimePeriod = e.TimePeriod;
            this.ID = e.SalaryId;
            this.Value = e.Value;
        }

        public float Value { get; set; }
        public string TimePeriod { get; set; }
        public int ID { get; set; }
        public int EmployeeId { get; set; }
    }
}
