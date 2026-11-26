using System.Text.Json.Serialization;

namespace UserAuthApp.Models
{
    class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}