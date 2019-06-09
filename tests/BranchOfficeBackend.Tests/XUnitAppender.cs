using System.IO;
using log4net.Appender;
using log4net.Core;
using Xunit.Abstractions;

namespace BranchOfficeBackend.Tests {
    public class XUnitAppender : AppenderSkeleton 
    { 
        private readonly ITestOutputHelper testOutputHelper; 
 
        public XUnitAppender(ITestOutputHelper testOutputHelper)  
        { 
            this.testOutputHelper = testOutputHelper; 
        } 
        /// <summary> 
        /// Writes the logging event to a MessageBox 
        /// </summary> 
        override protected void Append( LoggingEvent loggingEvent ) 
        { 
            using(var writer = new StringWriter()) { 
                this.Layout.Format(writer, loggingEvent); 
                writer.Flush(); 
                this.testOutputHelper.WriteLine(writer.ToString()); 
                if(loggingEvent.ExceptionObject != null) { 
                    this.testOutputHelper.WriteLine(loggingEvent.ExceptionObject.Message); 
                    this.testOutputHelper.WriteLine(loggingEvent.ExceptionObject.StackTrace); 
                } 
            } 
        } 
         
        /// <summary> 
        /// This appender requires a <see cref="Layout"/> to be set. 
        /// </summary> 
        override protected bool RequiresLayout 
        { 
            get { return true; } 
        } 
    } 
}