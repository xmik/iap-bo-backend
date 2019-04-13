using Moq;
using Xunit;

namespace BranchOfficeBackend.Tests
{
    /// <summary>
    /// Tests only the WebObjectService, other objects mocked
    /// </summary>
    public class WebObjectServiceTest
    {
        [Fact]
        public void TestGetAllEmployees()
        {
            var mock = new Moq.Mock<IDataAccessObjectService>();
            mock.Setup(m => m.GetAllEmployees()).Returns(new System.Collections.Generic.List<Employee>() {
                new Employee() { Name = "John" },
                new Employee() { Name = "Jane" }
            });
            WebObjectService webObj = new WebObjectService(mock.Object);
            var webEmployees = webObj.GetAllEmployees();

            Assert.Equal(2, webEmployees.Count);
            Assert.Equal("John", webEmployees[0].Name);
            Assert.Equal("Jane", webEmployees[1].Name);
            mock.Verify(m => m.GetAllEmployees(), Times.Once());
        }
    }
}
