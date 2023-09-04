using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.IO;
using Rentalmandu.Areas.Identity.Data;

namespace Rentalmandu.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<HajurKoCarRentalUser> _signInManager;
        private readonly UserManager<HajurKoCarRentalUser> _userManager;
        private readonly IUserStore<HajurKoCarRentalUser> _userStore;
        private readonly IUserEmailStore<HajurKoCarRentalUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const long MaxFileSize = (long)(1.5 * 1024 * 1024); // 1.5 MB


        public RegisterModel(
            UserManager<HajurKoCarRentalUser> userManager,
            IUserStore<HajurKoCarRentalUser> userStore,
            SignInManager<HajurKoCarRentalUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
        }

        private string GetUniqueFileName(string fileName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var randomString = Path.GetRandomFileName().Replace(".", "");
            var uniqueFileName = $"{fileNameWithoutExtension}_{timestamp}_{randomString}{extension}";
            return uniqueFileName;
        }
        private bool IsSupportedFileType(string fileName, string[] supportedExtensions)
        {
            var fileExtension = Path.GetExtension(fileName);
            return supportedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }


            [DataType(DataType.Upload)]
            [Display(Name = "Driving License")]
            public IFormFile DrivingLicense { get; set; }


            [DataType(DataType.Upload)]
            [Display(Name = "Citizen Paper")]
            public IFormFile CitizenPaper { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {

            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("/");
            }

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Check driving license file size and format
            if (Input.DrivingLicense != null && Input.DrivingLicense.Length > MaxFileSize)
            {
                ModelState.AddModelError(string.Empty, "The driving license file size should not exceed 1.5 MB.");
            }

            if (Input.DrivingLicense != null && !IsSupportedFileType(Input.DrivingLicense.FileName, new[] { ".pdf", ".png" }))
            {
                ModelState.AddModelError(string.Empty, "The driving license should be in PDF or PNG format.");
            }

            // Check citizen paper file size and format
            if (Input.CitizenPaper != null && Input.CitizenPaper.Length > MaxFileSize)
            {
                ModelState.AddModelError(string.Empty, "The citizen paper file size should not exceed 1.5 MB.");
            }

            if (Input.CitizenPaper != null && !IsSupportedFileType(Input.CitizenPaper.FileName, new[] { ".pdf", ".png" }))
            {
                ModelState.AddModelError(string.Empty, "The citizen paper should be in PDF or PNG format.");
            }


            
                var user = CreateUser();

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.Address = Input.Address;
                user.PhoneNumber = Input.PhoneNumber;
                user.DrivingLicense = null;
                user.CitizenPaper = null;
                user.CitizenPaperFileName = null;
                user.DrivingLicenseFileName = null;
                

                // Upload driving license scan
                if (Input.DrivingLicense != null && Input.DrivingLicense.Length > 0)
                {
                    if (Input.DrivingLicense.Length <= MaxFileSize)
                    {
                        // Generate a unique filename for the driving license
                        var drivingLicenseFileName = GetUniqueFileName(Input.DrivingLicense.FileName);

                        // Construct the file path to save the driving license in the wwwroot/documents folder
                        var drivingLicenseFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "documents", drivingLicenseFileName);

                        // Save the driving license file
                        using (var fileStream = new FileStream(drivingLicenseFilePath, FileMode.Create))
                        {
                            await Input.DrivingLicense.CopyToAsync(fileStream);
                        }
                        var license = new byte[Input.DrivingLicense.Length];

                        await Input.DrivingLicense.OpenReadStream().ReadAsync(user.DrivingLicense);
                    

                    // Set the user's driving license properties
                    user.DrivingLicense = license;
                    user.DrivingLicenseFileName = drivingLicenseFileName;
                    
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The driving license file size exceeds the maximum limit.");
                        return Page();
                    }
                }

                // Upload citizen paper scan
                if (Input.CitizenPaper != null && Input.CitizenPaper.Length > 0)
                {
                    if (Input.CitizenPaper.Length <= MaxFileSize)
                    {
                        // Generate a unique filename for the citizen paper
                        var citizenPaperFileName = GetUniqueFileName(Input.CitizenPaper.FileName);

                        // Construct the file path to save the citizen paper in the wwwroot/documents folder
                        var citizenPaperFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "documents", citizenPaperFileName);

                        // Save the citizen paper file
                        using (var fileStream = new FileStream(citizenPaperFilePath, FileMode.Create))
                        {
                            await Input.CitizenPaper.CopyToAsync(fileStream);
                        }

                        var CitizenPaper = new byte[Input.CitizenPaper.Length];

                        
                        await Input.CitizenPaper.OpenReadStream().ReadAsync(user.CitizenPaper);

                        // Set the user's citizen paper properties
                        user.CitizenPaper = CitizenPaper;
                        user.CitizenPaperFileName = citizenPaperFileName;
                       
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The citizen paper file size exceeds the maximum limit.");
                        return Page();
                    }
                }






                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // Assign the "Customer" role to the registered user
                    await _userManager.AddToRoleAsync(user, "customer");

                    _logger.LogInformation("User created a new account with password.");
                    

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            

            // If we got this far, something failed, redisplay form
            return Page();
        }


        private HajurKoCarRentalUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<HajurKoCarRentalUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(HajurKoCarRentalUser)}'. " +
                    $"Ensure that '{nameof(HajurKoCarRentalUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<HajurKoCarRentalUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<HajurKoCarRentalUser>)_userStore;
        }
    }
}
