using System.Collections.Generic;
using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    /// <summary>
    /// </summary>
    /// <remarks></remarks>
    public class TermInformationResponse
    {
        /// <summary>
        ///     Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        /// <remarks></remarks>
        [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        ///     Gets or sets the terms.
        /// </summary>
        /// <value>The terms.</value>
        /// <remarks></remarks>
        [JsonProperty("terms")]
        public List<TermContainer> Terms { get; set; }

        /// <summary>
        ///     Deserializes an onject from a JSON string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static TermInformationResponse FromJsonString(string value)
        {
            if (string.IsNullOrEmpty(value)) return new TermInformationResponse();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.DeserializeObject<TermInformationResponse>(value, settings);
        }

        /// <summary>
        ///     Serializes an object to a JSON string.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ToJsonString()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return JsonConvert.SerializeObject(this, settings);
        }
    }
}