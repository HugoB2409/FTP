using Newtonsoft.Json;

namespace FTP.Server.Model
{
    public class User
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        
        [JsonProperty("password")]
        public string Password { get; set; }
        
        [JsonProperty("defaultRepository")]
        public string DefaultRepository { get; set; }
    }
}