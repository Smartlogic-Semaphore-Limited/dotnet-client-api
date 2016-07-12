using Newtonsoft.Json;

namespace Smartlogic.Semaphore.Api.JSON
{
    public class TokenErrorResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("error_description")]
        public string Description { get; set; }
    }
}