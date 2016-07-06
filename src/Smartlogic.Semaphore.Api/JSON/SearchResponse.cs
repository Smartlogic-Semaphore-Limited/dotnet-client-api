using System.Collections.Generic;
using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    /// <summary>
    ///     Class SearchResponse
    /// </summary>
    public class SearchResponse
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
        ///     Froms the json string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static SearchResponse FromJsonString(string value)
        {
            return JsonConvert.DeserializeObject<SearchResponse>(value);
        }

        /// <summary>
        ///     Toes the json string.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}