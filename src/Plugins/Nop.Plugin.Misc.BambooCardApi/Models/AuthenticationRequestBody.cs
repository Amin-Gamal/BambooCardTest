using Newtonsoft.Json;

namespace Nop.Plugin.Misc.BambooCardApi.Models
{
    public class AuthenticationRequestBody
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("remember_me")]
        public bool RememberMe { get; set; }
    }
}