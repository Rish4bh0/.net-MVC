using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Rentalmandu.Areas.Identity.Data;
using Rentalmandu.Models;


namespace Rentalmandu.Models
{
    public class CarDamage
    {
        [Key]
        [Display(Name = "Damage ID")]
        [Required(ErrorMessage = "Damage ID is must required")]
        public int id { get; set; }

        [Display(Name = "Damage Description")]
        [Required(ErrorMessage = "Damage Description is must required")]
        public string? description { get; set; }


        [Display(Name = "Rental")]
        [Required(ErrorMessage = "Rental ID is required")]
        [ForeignKey("Rental")]

        public int RentalID { get; set; }


        [Display(Name = "Damage Charge")]
        public double? charge { get; set; }

        [Display(Name = "Damage Payment")]
        public bool IsPaid { get; set; } = false;


       
          public virtual Rental Rental { get; set; }



    }
}
