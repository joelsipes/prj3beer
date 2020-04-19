using System.ComponentModel.DataAnnotations;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace prj3beer.Models
{
    public class Beverage
    {
        [Key]
        [Required (ErrorMessage = "Beverage ID is Required")]
        [JsonProperty("id")]
        [Range(1,999,ErrorMessage = "ID Range must be between 1 and 999")]
        public int BeverageID { get; set; }

        [Required (ErrorMessage = "Beverage Name is Required")]
        [JsonProperty("name")]
        [MaxLength(40,ErrorMessage ="Beverage Name Too Long, 40 Characters Maximum")]
        [MinLength (3,ErrorMessage = "Beverage Name Too Short, 3 Characters Minimum")]
        public string Name { get; set; }

        [Required (ErrorMessage = "Brand is Required")]
        [JsonProperty("brand")]
        [Range(1, 200, ErrorMessage = "Brand ID must be a positive number less than 200")]
        [ForeignKey("BrandID")]
        public int? BrandID { get; set; }

        [Required(ErrorMessage = "Type is Required")]
        [JsonProperty("type")]
        public Type? Type { get; set; }

        [Required(ErrorMessage = "Temperature is Required")]
        [Range(-30, 30, ErrorMessage = "Target Temperature cannot be below -30C or above 30C")]
        [JsonProperty("temperature")]
        public double? Temperature { get; set; }

        //[MaxLength(150, ErrorMessage = "Image URL is too large")]
        [RegularExpression("(http(s?):)([/|.|\\w|\\s|-])*\\.(?:jpg|gif|png|jpeg)", ErrorMessage = "Image URL is not actually an image URL")]
        [JsonProperty("image")]
        public string ImageURL { get; set; }

    }
}