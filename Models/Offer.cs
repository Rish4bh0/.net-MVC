using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Rentalmandu.Models;


namespace Rentalmandu.Models
{
    public class Offer
    {
        [Key]
        [Display(Name = "Offer ID")]
        [Required(ErrorMessage = "Offer ID is must required")]
        public int id { get; set; } 

        [Display(Name = "Title")]
        public string Title { get; set; }


        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        
        public string Description { get; set; }

        [Display(Name = "Car Name")]
        [Required(ErrorMessage = "Car Name is must required")]
        [ForeignKey("CarInfo")]
        public int CarID { get; set; }

        [Display(Name = "Price Cut Off")]
        [Required(ErrorMessage = "Price Cut ")]

        public double Discount { get; set; }


        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start Date is required")]
       
        public DateTime StartDate { get; set; }

        
        

        [Display(Name = "End Date")]
        [Required(ErrorMessage = "End Date is required")]

        public DateTime EndDate { get; set; }

        public virtual CarInfo CarInfo { get; set; }


    }
}
