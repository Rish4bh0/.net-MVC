using Rentalmandu.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentalmandu.Models
{
    public class Notification
    {
        [Key]
        [Display(Name = "Notification ID")]
        [Required(ErrorMessage = "Notification ID is must required")]
        public int Id { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is must required")]
        public string Title { get; set; }

        [Display(Name = "Message")]
        [Required(ErrorMessage = "Message is must required")]
        public string Message { get; set; }

        [Display(Name = "User ID")]
        [Required(ErrorMessage = "User ID is required")]
        [ForeignKey("User")]
        public string UserID { get; set; }

        [Display(Name = "Rental")]
        

        [ForeignKey("Rental")]

        public int RentalID { get; set; }

        [Display(Name = "Created At")]

        public DateTime CreatedAt { get; set; }

        public virtual Rental Rental { get; set; }

        public virtual HajurKoCarRentalUser User { get; set; }


    }
}
