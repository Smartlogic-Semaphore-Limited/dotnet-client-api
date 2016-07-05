using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smartlogic.Semaphore.Api
{
    public interface ILogger
    {
        void WriteException(Exception ex);
        void WriteError(string message, params object[] parameters);
        void WriteHigh(string message, params object[] parameters);
        void WriteMedium(string message, params object[] parameters);
        void WriteLow(string message, params object[] parameters);
    }
}
