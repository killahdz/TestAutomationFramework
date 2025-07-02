using NUnit.Framework;
using TechTalk.SpecFlow.Tracing;

namespace Core.Library.Specflow
{
    public class SpecFlowLogTraceListener : ITraceListener
    {
        public static string LastLogMessage;

        public void WriteTestOutput(string message)
        {
            LastLogMessage = message;
            TestContext.Out.WriteLine(message);
            //Console.WriteLine(message);
        }

        public void WriteToolOutput(string message)
        {
            TestContext.Out.WriteLine("---> " + message);
            //Console.WriteLine("-> " + message);
        }
    }
}