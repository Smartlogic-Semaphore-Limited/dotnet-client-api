using System;
using System.Runtime.Serialization;

namespace Smartlogic.Semaphore.Api
{
 
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    [Serializable]
    public class SemaphoreConnectionException : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SemaphoreConnectionException"/> class.
        /// </summary>
        /// <remarks></remarks>
        public SemaphoreConnectionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemaphoreConnectionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        public SemaphoreConnectionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemaphoreConnectionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks></remarks>
        public SemaphoreConnectionException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemaphoreConnectionException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        ///   </exception>
        ///   
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        ///   </exception>
        /// <remarks></remarks>
        protected SemaphoreConnectionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    [Serializable]
    public class ClassificationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassificationException"/> class.
        /// </summary>
        /// <remarks></remarks>
        public ClassificationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassificationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        public ClassificationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassificationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks></remarks>
        public ClassificationException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        ///   </exception>
        ///   
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        ///   </exception>
        /// <remarks></remarks>
        protected ClassificationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}