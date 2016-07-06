using System.Collections.Generic;
using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    /// <summary>
    /// </summary>
    /// <remarks></remarks>
    public class Relationship
    {
        /// <summary>
        ///     Gets or sets the type id.
        /// </summary>
        /// <value>The type id.</value>
        /// <remarks></remarks>
        [JsonProperty("typeId")]
        public string TypeId { get; set; }

        /// <summary>
        ///     Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        /// <remarks></remarks>
        [JsonProperty("qty")]
        public string Quantity { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks></remarks>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the abbreviation.
        /// </summary>
        /// <value>The abbreviation.</value>
        /// <remarks></remarks>
        [JsonProperty("abbr")]
        public string Abbreviation { get; set; }

        /// <summary>
        ///     Gets or sets the fields.
        /// </summary>
        /// <value>The fields.</value>
        /// <remarks></remarks>
        [JsonProperty("fields")]
        public List<FieldContainer> Fields { get; set; }
    }
}