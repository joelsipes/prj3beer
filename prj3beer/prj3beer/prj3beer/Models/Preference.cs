using prj3beer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Windows;
using Xamarin.Forms;
using System.Drawing;

namespace prj3beer.Models
{
    public class Preference
    {
        #region Attributes

        [Key]
        [Required(ErrorMessage = "ID is required")]
        public int BeverageID { get; set; }

        [Required(ErrorMessage = "Favourite temperature is required")]
        [Range(-30, 30, ErrorMessage = "Target Temperature cannot be below -30C or above 30C")]
        public double? Temperature { get; set; }

        [DefaultValue("placeholder_can")]
        public string ImagePath { get; set; }

        [Required(ErrorMessage = "Favourite is required")]
        [DefaultValue(false)]
        public bool Favourite { get; set; }

        [NotMapped]
        public string BevName { get; set; }

        //const Uri uriImage = new Uri("placeholder_can");
        //UriImageSource test = new UriImageSource { Uri = uriImage };

        //[DefaultValue("")]
        //public UriImageSource savedImg { get; set; }
        #endregion

        #region Story 7 code
        /// <summary>
        /// This method will set a bool value to true when the image gets saved locally
        /// </summary>
        /// <returns></returns>
        public bool ImageSaved()
        {
            //throw new NotImplementedException();
            //if (ImagePath == null)
            if(ImagePath.Equals("") || ImagePath.Equals("placeholder_can") || ImagePath == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Gets and returns an image based on the passed in URL
        /// Assuming the image is not already cached, gets it from the internet
        /// Saves the image to cach if it did not have it - Saved for 7 days
        /// </summary>
        /// <param name="imageURL"> The url of the image to display. Bust be a valid image URL</param>
        /// <returns> Returns the newly saved image as it cannot be saved as a variable</returns>
        public Image SaveImage(String imageURL)
        {
            Image image = new Image();
            image.Source = "placeholder_can";
            if (!imageURL.Equals(""))
            {
                //ImagePath = imageURL;
                Uri uriImage = new Uri(imageURL);

                image.Source = new UriImageSource
                {
                    Uri = uriImage,
                    CachingEnabled = true,
                    CacheValidity = new TimeSpan(7, 0, 0, 0)
                };

                ImagePath = uriImage.AbsoluteUri;
            }
            else ImagePath = "placeholder_can";
            return image;
        }
        #endregion
    }
}