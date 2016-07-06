using System.Collections.Generic;
using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    /// <summary>
    ///     Class PrefixResponse
    /// </summary>
    public class PrefixResponse
    {
        /// <summary>
        ///     Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        /// <remarks></remarks>
        [JsonProperty("parameters")]
        public Dictionary<string, string> Parameters { get; set; }


        /// <summary>
        ///     Gets or sets the term hints.
        /// </summary>
        /// <value>The term hints.</value>
        [JsonProperty("termHints")]
        public List<TermContainer> TermHints { get; set; }

        /// <summary>
        ///     Gets or sets the total.
        /// </summary>
        /// <value>The total.</value>
        [JsonProperty("total")]
        public int Total { get; set; }

        /// <summary>
        ///     Deserializes an object from a JSON string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static PrefixResponse FromJsonString(string value)
        {
            return JsonConvert.DeserializeObject<PrefixResponse>(value);
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