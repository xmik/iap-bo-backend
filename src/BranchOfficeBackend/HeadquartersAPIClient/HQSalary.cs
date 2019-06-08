namespace BranchOfficeBackend
{
    public class HQSalary
    {
        private float _value;
        private string _timePeriod;
        private int _id;
        private int _employeeId;

        public float Value { get => _value; set => _value = value; }     
        public string TimePeriod { get => _timePeriod; set => _timePeriod = value; }  
        public int ID { get => _id; set => _id = value; } 
        public int EmployeeID { get => _employeeId; set => _employeeId = value; } 
        
    }
}