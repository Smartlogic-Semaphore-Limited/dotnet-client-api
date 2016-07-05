using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>

    public class Field
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks></remarks>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks></remarks>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>The frequency.</value>
        /// <remarks></remarks>
        [JsonProperty("freq")]
        public string Frequency { get; set; }

        /// <summary>
        /// Gets or sets the facets.
        /// </summary>
        /// <value>The facets.</value>
        /// <remarks></remarks>
        [JsonProperty("facets")]
        public List<Facet> Facets { get; set; }


        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        [JsonProperty("node")]
        public Object Node { get; set; }


        /// <summary>
        /// Gets or sets the termstore node query.
        /// </summary>
        /// <value>The termstore node query.</value>
        /// <remarks></remarks>
        [JsonProperty("termstoreNodeQuery")]
        [Obsolete("Included for backward compatibility")]
        public string TermstoreNodeQuery { get; set; }

        /// <summary>
        /// Gets or sets the tag profile query.
        /// </summary>
        /// <value>The tag profile query.</value>
        /// <remarks></remarks>
        [JsonProperty("tagProfileQuery")]
        [Obsolete("Included for backward compatibility")]
        public string TagProfileQuery { get; set; }

    }
}