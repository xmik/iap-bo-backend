
using System;
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
        [Fact]
        public void Employee_FromHQEmployee()
        {
            var other = new HQEmployee { Name = "Ola Dwa", Email = "ola2@gmail.com", ID = 0,
                 BranchOfficeID = 123, DateOfBirth = "1996-01-25", IsManager = true, Pay = 30 };
            var obj = new Employee (other);
            Assert.Equal(-1, obj.EmployeeId);
            Assert.Equal("Ola Dwa", obj.Name);
            Assert.Equal("ola2@gmail.com", obj.Email);
            Assert.True(obj.IsManager);
            Assert.Equal(new DateTime(1996,1,25), obj.DateOfBirth);
        }
    
    }
}