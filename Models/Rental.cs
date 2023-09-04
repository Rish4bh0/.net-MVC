using Rentalmandu.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentalmandu.Models
{
    public class Rental
    {
        [Key]
        [Display(Name = "Rental ID")]
        [Required(ErrorMessage = "Rental ID is required")]
        public int Id { get; set; }

        [Display(Name = "Car ID")]
        [Required(ErrorMessage = "Car ID is required")]
        [ForeignKey("CarInfo")]
        public int CarID { get; set; }


        [Display(Name = "User ID")]
        [Required(ErrorMessage = "User ID is required")]
        [ForeignKey("User")]
        public string UserID { get; set; }

        [Display(Name = "Authorized By")]
        public string AuthorizedBy { get; set; }

        [Display(Name = "Rental Date")]
        [Required(ErrorMessage = "Rental Date is required")]
        public DateTime date { get; set; }


        [Display(Name = "Fee")]
        [Required(ErrorMessage = "Fee is required")]
        public decimal Fee { get; set; }

        [Display(Name = "Rental Status")]
        [Required(ErrorMessage = "Rental Status is required")]

        public int RentalStatus { get; set; }

        [Display(Name = "Created At")]
        

        public DateTime CreatedAt { get; set; }

        public virtual CarInfo CarInfo { get; set; }
        public virtual HajurKoCarRentalUser User { get; set; }




    }
}
