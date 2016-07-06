using System;

namespace Smartlogic.Semaphore.Api
{
    public abstract class LoggingProxyBase
    {
        protected ILogger Logger { get; }

        protected LoggingProxyBase()
        {
        }

        protected LoggingProxyBase(ILogger logger)
        {
            Logger = logger;
        }

        protected void WriteError(string message, params object[] parameters)
        {
            if (Logger != null) Logger.WriteError(message, parameters);
        }

        protected void WriteException(Exception ex)
        {
            if (Logger != null) Logger.WriteException(ex);
        }

        protected void WriteHigh(string message, params object[] parameters)
        {
            if (Logger != null) Logger.WriteHigh(message, parameters);
        }

        protected void WriteLow(string message, params object[] parameters)
        {
            if (Logger != null) Logger.WriteLow(message, parameters);
        }

        protected void WriteMedium(string message, params object[] parameters)
        {
            if (Logger != null) Logger.WriteMedium(message, parameters);
        }
    }
}