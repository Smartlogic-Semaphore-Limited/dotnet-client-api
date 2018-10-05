using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    /// <summary>
    /// </summary>
    /// <remarks></remarks>
    public class Term
    {
        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks></remarks>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        /// <remarks></remarks>
        [JsonProperty("class")]
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets the deprecated field.
        /// </summary>
        [JsonProperty("deprecated")]
        public bool IsDeprecated { get; set; }

        /// <summary>
        ///     Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        [JsonProperty("node")]
        public object Node { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks></remarks>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets the frequency.
        /// </summary>
        /// <value>The frequency.</value>
        /// <remarks></remarks>
        [JsonProperty("freq")]
        public string Frequency { get; set; }

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
        [JsonProperty("attributes")]
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        ///     Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        /// <remarks></remarks>
        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        /// <summary>
        ///     Gets or sets the custom properties.
        /// </summary>
        /// <value>The custom properties.</value>
        /// <remarks></remarks>
        [JsonProperty("customProperties")]
        public Dictionary<string, string> CustomProperties { get; set; }

        /// <summary>
        ///     Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        /// <remarks></remarks>
        [JsonProperty("index")]
        public string Index { get; set; }

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        /// <remarks></remarks>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the last updated date.
        /// </summary>
        /// <value>The last updated date.</value>
        /// <remarks></remarks>
        [JsonProperty("lastUpdatedDate")]
        [JsonConverter(typeof(SesDateTimeConverter))]
        public DateTime? LastUpdatedDate { get; set; }

        /// <summary>
        ///     Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        /// <remarks></remarks>
        [JsonProperty("createdDate")]
        [JsonConverter(typeof(SesDateTimeConverter))]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        ///     Gets or sets the created by.
        /// </summary>
        /// <value>The created by.</value>
        /// <remarks></remarks>
        [JsonProperty("creator")]
        public string CreatedBy { get; set; }

        /// <summary>
        ///     Gets or sets the updated by.
        /// </summary>
        /// <value>The updated by.</value>
        /// <remarks></remarks>
        [JsonProperty("updator")]
        public string UpdatedBy { get; set; }

        /// <summary>
        ///     Gets or sets the status id.
        /// </summary>
        /// <value>The status id.</value>
        /// <remarks></remarks>
        [JsonProperty("status")]
        public string StatusId { get; set; }

        /// <summary>
        ///     Gets or sets the documents.
        /// </summary>
        /// <value>The documents.</value>
        /// <remarks></remarks>
        [JsonProperty("documents")]
        public string Documents { get; set; }

        /// <summary>
        ///     Gets or sets the hierarchy.
        /// </summary>
        /// <value>The hierarchy.</value>
        /// <remarks></remarks>
        [JsonProperty("hierarchy")]
        public List<Relationship> Hierarchy { get; set; }

        /// <summary>
        ///     Gets or sets the associated.
        /// </summary>
        /// <value>The associated.</value>
        /// <remarks></remarks>
        [JsonProperty("associated")]
        public List<Relationship> Associated { get; set; }

        /// <summary>
        ///     Gets or sets the equivalence.
        /// </summary>
        /// <value>The equivalence.</value>
        /// <remarks></remarks>
        [JsonProperty("equivalence")]
        public List<Relationship> Equivalence { get; set; }

        /// <summary>
        ///     Gets or sets the paths.
        /// </summary>
        /// <value>The paths.</value>
        /// <remarks></remarks>
        [JsonProperty("paths")]
        public List<PathContainer> Paths { get; set; }

        /// <summary>
        ///     Gets or sets the termstore node query.
        /// </summary>
        /// <value>The termstore node query.</value>
        /// <remarks></remarks>
        [JsonProperty("termstoreNodeQuery")]
        [Obsolete("Included for backward compatibility")]
        public string TermstoreNodeQuery { get; set; }

        /// <summary>
        ///     Gets or sets the tag profile query.
        /// </summary>
        /// <value>The tag profile query.</value>
        /// <remarks></remarks>
        [JsonProperty("tagProfileQuery")]
        [Obsolete("Included for backward compatibility")]
        public string TagProfileQuery { get; set; }
    }
}