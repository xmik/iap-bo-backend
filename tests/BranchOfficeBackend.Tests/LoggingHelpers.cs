using System.Reflection;
using log4net;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Xunit.Abstractions;

namespace BranchOfficeBackend.Tests {
    public class LoggingHelpers 
    { 
        public static void Configure(ITestOutputHelper testOutputHelper) 
        { 
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository(Assembly.GetEntryAssembly()); 
            hierarchy.Root.RemoveAllAppenders(); /*Remove any other appenders*/ 
 
            XUnitAppender xunitAppender = new XUnitAppender(testOutputHelper); 
            PatternLayout pl = new PatternLayout(); 
            pl.ConversionPattern = "%timestamp [%thread] %level %logger %ndc - %message"; 
            pl.ActivateOptions(); 
            xunitAppender.Layout = pl; 
            xunitAppender.ActivateOptions(); 
 
            log4net.Config.BasicConfigurator.Configure(hierarchy, xunitAppender); 
        } 
    } 
}