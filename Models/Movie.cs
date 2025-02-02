using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dioflix.Models
{
    public class Movie
    {
        [JsonProperty("id")]
        public string Id { get => Guid.NewGuid().ToString(); }

        [JsonProperty("title")] 
        public string Title { get; set; }

        [JsonProperty("year")] 
        public string Year { get; set; }
        
        [JsonProperty("video")] 
        public string Video { get; set; }
        
        [JsonProperty("thumb")] 
        public string Thumb { get; set; }
    }
}
