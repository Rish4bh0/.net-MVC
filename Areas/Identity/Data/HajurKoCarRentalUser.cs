using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace Rentalmandu.Areas.Identity.Data
{

    // Add profile data for application users by adding properties to the HajurKoCarRentalUser class
    public class HajurKoCarRentalUser : IdentityUser
    {
        [Required]
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [Required]
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        [Required]
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string Address { get; set; }

        [Required]
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string PhoneNumber { get; set; }



        [Column(TypeName = "varbinary(max)")]
        public byte[]? DrivingLicense { get; set; }

        [PersonalData]
        public string? DrivingLicenseFileName { get; set; }



        [PersonalData]
        [Column(TypeName = "varbinary(max)")]
        public byte[]? CitizenPaper { get; set; }

        [PersonalData]
        public string? CitizenPaperFileName { get; set; }


        public bool IsRegularCustomer { get; set; }

    }
}



