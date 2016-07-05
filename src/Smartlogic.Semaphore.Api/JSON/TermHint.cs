using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    /// <summary>
    ///     Class TermHint
    /// </summary>
    
    public class TermHint
    {
        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks></remarks>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks></remarks>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        [JsonProperty("index")]
        public string Index { get; set; }

        /// <summary>
        ///     Gets or sets the facets.
        /// </summary>
        /// <value>The facets.</value>
        /// <remarks></remarks>
        [JsonProperty("facets")]
        public List<Facet> Facets { get; set; }

        /// <summary>
        ///     Gets or sets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        /// <remarks></remarks>
        [JsonProperty("values")]
        public List<HintValue> Values { get; set; }
    }
}