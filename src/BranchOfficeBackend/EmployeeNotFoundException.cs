namespace BranchOfficeBackend {
    [System.Serializable]
    public class EmployeeNotFoundException : System.Exception
    {
        public EmployeeNotFoundException() { }
        public EmployeeNotFoundException(string message) : base(message) { }
        public EmployeeNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected EmployeeNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}