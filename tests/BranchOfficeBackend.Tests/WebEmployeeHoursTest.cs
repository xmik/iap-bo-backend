using System;
using Xunit;

namespace BranchOfficeBackend.Tests
{
    public class WebEmployeeHoursTest
    {
        [Fact]
        public void TransformationTest()
        {
            var eh = new EmployeeHours();
            eh.EmployeeHoursId = 99;
            eh.EmployeeId = 1;
            eh.Value = 600;
            eh.TimePeriod = "20.1.2019-26.01.2019";

            var webEh = new WebEmployeeHours(eh);
            Assert.Equal(99, webEh.Id);
            Assert.Equal(1, webEh.EmployeeId);
            Assert.Equal(600f, webEh.Value);
            Assert.Equal("20.1.2019-26.01.2019", webEh.TimePeriod);
        }
    }
}