using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Rentalmandu.Areas.Identity.Data;
using Rentalmandu.Models;


namespace Rentalmandu.Models
{
    public class CarRentalInfo
    {
        public CarInfo CarInfo { get; set; }
        public int RentalCount { get; set; }
        public List<Rental> Rentals { get; set; }
    }
}
