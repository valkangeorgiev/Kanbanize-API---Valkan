using Newtonsoft.Json;

namespace Kanbanize_API___Valkan
{
    public class CardResponse
    {
        [JsonProperty("data")]
        public List<Body> data { get; set; }

       
    }
    public class CardResponseObject
    {
        [JsonProperty("data")]
        public Body data { get; set; }


    }
}