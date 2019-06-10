
using Xunit;

namespace BranchOfficeBackend.Tests
{
    public class ModelTest
    {
        [Fact]
        public void Employee_ToString()
        {
            var obj = new Employee { Name = "Ola Dwa", Email = "ola2@gmail.com", EmployeeId = 0 };
            string str = obj.ToString();
            Assert.Equal("Id: 0, Name: Ola Dwa, Email: ola2@gmail.com, DateOfBirth 1/1/01 12:00:00 AM, IsManager: False", str);
        }
    
    }
}