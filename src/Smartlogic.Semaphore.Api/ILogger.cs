using System;

namespace Smartlogic.Semaphore.Api
{
    public interface ILogger
    {
        void WriteError(string message, params object[] parameters);
        void WriteException(Exception ex);
        void WriteHigh(string message, params object[] parameters);
        void WriteLow(string message, params object[] parameters);
        void WriteMedium(string message, params object[] parameters);
    }
}