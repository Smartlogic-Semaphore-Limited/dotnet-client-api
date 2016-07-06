using System.Collections.Generic;
using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    /// <summary>
    ///     Class BrowseResponse
    /// </summary>
    public class BrowseResponse
    {
        /// <summary>
        ///     Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        /// <remarks></remarks>
        [JsonProperty("parameters")]
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        ///     Gets or sets the terms.
        /// </summary>
        /// <value>The terms.</value>
        /// <remarks></remarks>
        [JsonProperty("terms")]
        public List<TermContainer> Terms { get; set; }


        /// <summary>
        ///     Gets or sets the terms.
        /// </summary>
        /// <value>The terms.</value>
        /// <remarks></remarks>
        [JsonProperty("browseTerms")]
        public List<TermContainer> BrowseTerms { get; set; }

        /// <summary>
        ///     Deserializes an object from a JSON string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static BrowseResponse FromJsonString(string value)
        {
            return JsonConvert.DeserializeObject<BrowseResponse>(value);
        }

        /// <summary>
        ///     Serializes an oject to a JSON string.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}