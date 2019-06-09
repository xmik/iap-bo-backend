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
            eh.TimePeriod = "2019-1-20_2019-1-26";

            var webEh = new WebEmployeeHours(eh);
            Assert.Equal(99, webEh.EmployeeHoursId);
            Assert.Equal(1, webEh.EmployeeId);
            Assert.Equal(600, webEh.Value);
            Assert.Equal("2019-1-20_2019-1-26", webEh.TimePeriod);
        }
    }
}