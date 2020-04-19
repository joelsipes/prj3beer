using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xamarin.Forms;

namespace prj3beer.Models
{
    public class Friend
    {
        // Email field for friend, should be a unique key
        [Key]
        [JsonProperty("id")]
        public string ID { get; set; }

        // Name field for a friend
        [JsonProperty("name")]
        public string Name { get; set; }

        // Friend's Profile URI
        [JsonProperty("image")]
        public string ImageURI { get; set; }
    }
}
