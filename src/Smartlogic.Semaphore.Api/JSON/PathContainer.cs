using System.Collections.Generic;
using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    /// <summary>
    /// </summary>
    /// <remarks></remarks>
    public class PathContainer
    {
        /// <summary>
        ///     Gets or sets the fields.
        /// </summary>
        /// <value>The fields.</value>
        /// <remarks></remarks>
        [JsonProperty("path")]
        public List<FieldContainer> Fields { get; set; }
    }
}