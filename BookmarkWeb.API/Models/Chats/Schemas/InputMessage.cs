using Newtonsoft.Json;

namespace BookmarkWeb.API.Models.Chats.Schemas
{
    public class InputMessage
    {
        [JsonProperty("id")]
        public string Id { set; get; }

        [JsonProperty("content")]
        public string Content { set; get; }
    }
}