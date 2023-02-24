using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanbanize_API___Valkan
{

    public class ErrorBox<T>
    {
        [JsonProperty("error")]
        public ErrorModel<T> error { get; set; }



    }

    public class ErrorModel<T>
    {
        public string message { get; set; }

        public Details<T> details { get; set; }
        
    }


    public class Details<T>
    {
        [JsonProperty("lane_id")]
        public T lane_id { get; set; }

        [JsonProperty("column_id")]
        public T column_id { get; set; }

        [JsonProperty("position")]
        public T position { get; set; }

        [JsonProperty("title")]
        public T title { get; set; }

        [JsonProperty("color")]
        public T color { get; set; }

        [JsonProperty("priority")]
        public T priority { get; set; }
    }


    

}
