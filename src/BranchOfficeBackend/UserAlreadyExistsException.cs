namespace BranchOfficeBackend {
    [System.Serializable]
    public class UserAlreadyExistsException : System.Exception
    {
        public UserAlreadyExistsException() { }
        public UserAlreadyExistsException(string message) : base(message) { }
        public UserAlreadyExistsException(string message, System.Exception inner) : base(message, inner) { }
        protected UserAlreadyExistsException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}