using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    
    public class TermContainer
    {
        /// <summary>
        /// Gets or sets the term.
        /// </summary>
        /// <value>The term.</value>
        /// <remarks></remarks>
        [JsonProperty("term")]
        public Term Term { get; set; }
    }
}