using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Rentalmandu.Models;


namespace Rentalmandu.Models
{
    public class CarInfo
    {
        [Key]
        [Display(Name = "Car ID")]
        [Required(ErrorMessage = "Car ID is must required")]
        public int id { get; set; }

       
        [Display(Name = "Car Image")]
        public string CarImage { get; set; }


        [Display(Name = "Car Name")]
        [Required(ErrorMessage = "Car Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Car name must be in between 3 and 50 characters")]
        public string CarName { get; set; }

        [Display(Name = "Car Brand")]
        [Required(ErrorMessage = "Car Brand is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Car Brand must be in between 3 and 50 characters")]
        public string Brand { get; set; }


        [Display(Name = "Car Model")]
        [Required(ErrorMessage = "Car Model is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Car Model must be in between 3 and 50 characters")]
        public string Model { get; set; }


        [Display(Name = "Price Per Day")]
        public decimal PricePerDay { get; set; }


        [Display(Name = "Is Available")]

        public bool IsAvailable { get; set; }






    }
}
