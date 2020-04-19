using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace prj3beer.Models
{
     /// <summary>
     /// This is our Brand Entity, this will store an ID and a name for each brand pulled from the API. 
     /// </summary>
    public class Brand
    {
        [Key] // Primary Key
        [JsonProperty("id")] // JSON Property
        //[Required(ErrorMessage = "Brand ID is Required")] // Validation - Required. If not set, will return error
        [Range(1,200, ErrorMessage = "Brand ID must be a positive number less than 200")] //Brand ID must be a positive number between 1 and 200 (for now)
        public int BrandID { get; set; }    

        [Required(ErrorMessage ="Brand Name Required")] // Validation - Required. If not set, will return error
        [MaxLength(40,ErrorMessage = "Brand Name Too Long, 40 Characters Maximum")] // Validation - Max Length of 60
        [MinLength(3,ErrorMessage = "Brand Name Too Short, 3 Characters Minimum")] // Validation - Min Length of 3
        [JsonProperty("name")] // JSON property
        public string Name { get; set; }

        public ICollection<Beverage> Beverages { get; set; }
        // Need to figure out how to populate this... https://stackoverflow.com/questions/11365007/entity-framework-one-to-many
    }
}
