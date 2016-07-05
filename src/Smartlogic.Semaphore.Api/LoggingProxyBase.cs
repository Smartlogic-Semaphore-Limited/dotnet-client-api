using System;
using System.Diagnostics.CodeAnalysis;

namespace Smartlogic.Semaphore.Api
{
    
    public abstract class LoggingProxyBase
    {
        private readonly ILogger _logger;

        protected LoggingProxyBase()
        {
            
        }

        protected LoggingProxyBase(ILogger logger)
        {
            _logger = logger;
        }

        protected void WriteError(string message, params object[] parameters)
        {
            if (_logger != null) _logger.WriteError(message, parameters);
        }

        protected void WriteException(Exception ex)
        {
            if (_logger != null) _logger.WriteException(ex);
        }

        protected void WriteHigh(string message, params object[] parameters)
        {
            if (_logger != null) _logger.WriteHigh(message, parameters);
        }

        protected void WriteLow(string message, params object[] parameters)
        {
            if (_logger != null) _logger.WriteLow(message, parameters);
        }

        protected void WriteMedium(string message, params object[] parameters)
        {
            if (_logger != null) _logger.WriteMedium(message, parameters);
        }
    }
}