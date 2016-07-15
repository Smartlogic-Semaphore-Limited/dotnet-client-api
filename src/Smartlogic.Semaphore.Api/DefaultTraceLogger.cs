using System;
using System.Diagnostics;

namespace Smartlogic.Semaphore.Api
{
    /// <summary>
    /// Provides a default ILogger implementation that makes use of <cref>System.Diagnostics.Trace</cref>.
    /// </summary>
    public sealed class DefaultTraceLogger : ILogger
    {
        /// <summary>
        /// The WriteError method is called when an expected error occurs.
        /// </summary>
        /// <param name="message">Contains a format string</param>
        /// <param name="parameters">Contains replacement paramters for the format string passed in message or null if no paramters are required</param>
        public void WriteError(string message, params object[] parameters)
        {
            if (parameters != null)
            {
                Trace.WriteLine(string.Format(message, parameters));
            }
            else
            {
                Trace.WriteLine(message);
            }
            
        }

        /// <summary>
        /// The WriteException method is called when an unexpected error occurs.
        /// </summary>
        /// <param name="ex"></param>
        public void WriteException(Exception ex)
        {
            Trace.WriteLine(ex.Message);
        }

        /// <summary>
        /// The WriteHigh method is called to log and important event.
        /// </summary>
        /// <param name="message">Contains a format string</param>
        /// <param name="parameters">Contains replacement paramters for the format string passed in message or null if no paramters are required</param>
        public void WriteHigh(string message, params object[] parameters)
        {
            if (parameters != null)
            {
                Trace.WriteLine(string.Format(message, parameters));
            }
            else
            {
                Trace.WriteLine(message);
            }
        }

        /// <summary>
        /// The WriteLow method is called to log low level information/debug level events.
        /// </summary>
        /// <param name="message">Contains a format string</param>
        /// <param name="parameters">Contains replacement paramters for the format string passed in message or null if no paramters are required</param>
        public void WriteLow(string message, params object[] parameters)
        {
            if (parameters != null)
            {
                Trace.WriteLine(string.Format(message, parameters));
            }
            else
            {
                Trace.WriteLine(message);
            }
        }

        /// <summary>
        /// The WriteMedium method is called to log significant events that occur during processing.
        /// </summary>
        /// <param name="message">Contains a format string</param>
        /// <param name="parameters">Contains replacement paramters for the format string passed in message or null if no paramters are required</param>
        public void WriteMedium(string message, params object[] parameters)
        {
            if (parameters != null)
            {
                Trace.WriteLine(string.Format(message, parameters));
            }
            else
            {
                Trace.WriteLine(message);
            }
        }
    }
}