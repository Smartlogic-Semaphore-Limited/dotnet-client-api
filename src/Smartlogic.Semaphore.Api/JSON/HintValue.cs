using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    /// <summary>
    ///     Class HintValue
    /// </summary>
    public class HintValue
    {
        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        ///     Gets or sets the pre emphasis text.
        /// </summary>
        /// <value>The pre emphasis text.</value>
        [JsonProperty("pre_em")]
        public string PreEmphasisText { get; set; }

        /// <summary>
        ///     Gets or sets the emphasised text.
        /// </summary>
        /// <value>The emphasised text.</value>
        [JsonProperty("em")]
        public string EmphasisedText { get; set; }

        /// <summary>
        ///     Gets or sets the post emphasis text.
        /// </summary>
        /// <value>The post emphasis text.</value>
        [JsonProperty("post_em")]
        public string PostEmphasisText { get; set; }

        /// <summary>
        ///     Gets or sets the type of the match.
        /// </summary>
        /// <value>The type of the match.</value>
        [JsonProperty("nature")]
        public string MatchType { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}