using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    /// <summary>
    /// </summary>
    /// <remarks></remarks>
    public class Facet
    {
        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks></remarks>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks></remarks>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}