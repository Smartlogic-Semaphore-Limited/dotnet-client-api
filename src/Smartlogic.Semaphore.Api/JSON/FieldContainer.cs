using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    /// <summary>
    /// </summary>
    /// <remarks></remarks>
    public class FieldContainer
    {
        /// <summary>
        ///     Gets or sets the field.
        /// </summary>
        /// <value>The field.</value>
        /// <remarks></remarks>
        [JsonProperty("field")]
        public Field Field { get; set; }
    }
}