using System;
using System.Diagnostics.CodeAnalysis;

namespace Smartlogic.Semaphore.Api.Tests
{
    [ExcludeFromCodeCoverage]
    internal class TestLogger : ILogger
    {
        #region ILogger Members

        public void WriteException(Exception ex)
        {
            Console.WriteLine(ex);
        }

        public void WriteError(string message,
                               params object[] parameters)
        {
            Console.WriteLine(message, parameters);
        }

        public void WriteHigh(string message,
                              params object[] parameters)
        {
            Console.WriteLine(message, parameters);
        }

        public void WriteMedium(string message,
                                params object[] parameters)
        {
            Console.WriteLine(message, parameters);
        }

        public void WriteLow(string message,
                             params object[] parameters)
        {
            Console.WriteLine(message, parameters);
        }

        #endregion
    }
}