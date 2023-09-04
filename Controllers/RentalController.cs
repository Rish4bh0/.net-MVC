using Rentalmandu.Areas.Identity.Data;
using Rentalmandu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;
using System.Linq;
using System.Globalization;





namespace Rentalmandu.Controllers
{
    public class RentalController : Controller
    {
        private readonly HajurKoCarRentalDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<HajurKoCarRentalUser> _userManager;

        public RentalController(HajurKoCarRentalDbContext dbContext, IWebHostEnvironment webHostEnvironment, UserManager<HajurKoCarRentalUser> userManager)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        // GET: Rental/Rent/5
        public IActionResult Rent(int id)
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect the user to the login page
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Check if the logged in user has uploaded the documents
            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            // Check if the user has uploaded the documents
            if (user.DrivingLicenseFileName == null || user.CitizenPaperFileName == null)
            {
                // Redirect the user to upload the documents
                return RedirectToAction("UploadDocuments");
            }

            // Get the car info by ID
            var car = _dbContext.CarInfo.FirstOrDefault(c => c.id == id);

            if (car == null)
            {
                return NotFound();
            }


          

            return View(car);
        }

        // POST: Rental/Rent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rent(DateTime date, int id)
        {
            

            // Check if the logged in user has uploaded the documents
            var user = await _userManager.GetUserAsync(User);

           

            if (ModelState.IsValid)
            {
                var car = _dbContext.CarInfo.First(x=> x.id ==id);

                car.IsAvailable = false;

                
                Rental rental = new Rental();
                // Update rental properties
                rental.UserID = user.Id;
                rental.Fee = CalculateRentalFee(id, user.IsRegularCustomer);
                rental.RentalStatus = 0;
                rental.CarID = id;
                rental.AuthorizedBy = "none";
                rental.date = date;
                rental.CreatedAt = DateTime.Now;
               

                _dbContext.Rental.Add(rental);
                await _dbContext.SaveChangesAsync();

               

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        private decimal CalculateRentalFee(int carId, bool isRegularCustomer)
        {
            var car = _dbContext.CarInfo.FirstOrDefault(c => c.id == carId);

            if (car == null)
            {
                throw new InvalidOperationException("Invalid car ID.");
            }

            decimal rentalFee = car.PricePerDay;

            if (isRegularCustomer)
            {
                rentalFee *= 0.9m; // 10% discount for regular customers
            }

            return rentalFee;
        }

       


        // GET: Rental/Confirmation
        public IActionResult Confirmation()
        {
            return View();
        }

        // GET: Rental/UploadDocuments
        [Authorize]
        public IActionResult UploadDocuments()
        {
            // Display the view to upload the documents
            return View();
        }

        // POST: Rental/UploadDocuments
        [HttpPost]
        public async Task<IActionResult> UploadDocuments(DocumentUploadViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save the driving license file
                if (model.DrivingLicense != null && model.DrivingLicense.Length > 0)
                {
                    string drivingLicenseFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.DrivingLicense.FileName);
                    string drivingLicenseFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "documents", drivingLicenseFileName);

                    using (var stream = new FileStream(drivingLicenseFilePath, FileMode.Create))
                    {
                        await model.DrivingLicense.CopyToAsync(stream);
                    }

                    // Update the user's driving license details in the database
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _userManager.FindByIdAsync(userId);
                    user.DrivingLicenseFileName = drivingLicenseFileName;
                    
                    await _userManager.UpdateAsync(user);
                }

                // Save the citizen paper file
                if (model.CitizenPaper != null && model.CitizenPaper.Length > 0)
                {
                    string citizenPaperFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CitizenPaper.FileName);
                    string citizenPaperFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "documents", citizenPaperFileName);

                    using (var stream = new FileStream(citizenPaperFilePath, FileMode.Create))
                    {
                        await model.CitizenPaper.CopyToAsync(stream);
                    }

                    // Update the user's citizen paper details in the database
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _userManager.FindByIdAsync(userId);
                    user.CitizenPaperFileName = citizenPaperFileName;
                    
                    await _userManager.UpdateAsync(user);
                }

                return RedirectToAction("Index", "Home");
            }

            // If the model is invalid, return the view with the model errors
            return View(model);
        }


        // GET: Rental/ViewRequestedRental
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RentalHistory()
        {
            // Get the logged-in user
            var user = await _userManager.GetUserAsync(User);

            // Get the rentals for the logged-in user
            var rentals = _dbContext.Rental
                .Include(r => r.CarInfo)
                .Where(r => r.UserID == user.Id)
                .ToList();

            return View(rentals);
        }

        // GET: Rental/Cancel/5
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            // Get the rental by ID
            var rental = await _dbContext.Rental
                .Include(r => r.CarInfo)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
            {
                return NotFound();
            }

            // Check if the rental is in a cancelable state (status 0 or 1)
            if (rental.RentalStatus == 0 || rental.RentalStatus == 1)
            {
                return View(rental);
            }

            return RedirectToAction("RentalHistory");
        }
        //Action method to retrieve frequently rented cars
        public IActionResult FrequentlyRentedCars()
        {
            //Joining CarInfo and Rental tables and grouping by car to get rental count
            var frequentlyRentedCars = _dbContext.CarInfo
                .Join(_dbContext.Rental, c => c.id, r => r.CarID, (c, r) => new { Car = c, Rental = r })
                .GroupBy(x => x.Car)
                .OrderByDescending(g => g.Count())
                .Select(g => new CarRentalInfo
                {
                    CarInfo = g.Key,
                    RentalCount = g.Count(),
                    Rentals = g.Select(x => x.Rental).ToList()
                })
                .ToList();
            //Returning the view with the data
            return View(frequentlyRentedCars);
        }

        //Action method to retrieve frequently not rented cars
        public IActionResult FrequentlyNotRentedCars()
        {
            //Group joining CarInfo and Rental tables and filtering out rentals that happened within last 90 days
            var frequentlyNotRentedCars = _dbContext.CarInfo
            .GroupJoin(_dbContext.Rental, c => c.id, r => r.CarID, (c, r) => new { Car = c, Rentals = r })
            .SelectMany(x => x.Rentals.DefaultIfEmpty(), (car, rental) => new { Car = car.Car, Rental = rental })
            .Where(x => x.Rental == null || x.Rental.date < DateTime.Today.AddDays(-90))
            .GroupBy(x => x.Car)
            .OrderByDescending(g => g.Count())
            .Select(g => new CarRentalInfo
            {
                CarInfo = g.Key,
                RentalCount = g.Count(),
                Rentals = new List<Rental>()
            })
            .Take(10)
            .ToList();
            //Returning the view with the data
            return View(frequentlyNotRentedCars);
        }

        //Action method to retrieve all customer users
        public IActionResult AllUsers()
        {
            //Retrieving all users and filtering out non-customer ones
            var users = _dbContext.Users.ToList();
            var userList = new List<HajurKoCarRentalUser>();
            foreach (var user in users)
            {
                var userRoles = _userManager.GetRolesAsync(user).Result;
                if (userRoles.Contains("customer"))
                {
                    userList.Add(user);
                }
            }

            return View(userList);
        }

        //Action method to retrieve rental history of an individual user
        public IActionResult indiRentalHistory(string userId)
        {
            //Retrieving user with the given id and returning 404 if not found
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            //Retrieving rentals for the user and including related car info

            var rentals = _dbContext.Rental.Include(r => r.CarInfo).Where(r => r.UserID == userId).ToList();

            ViewData["User"] = user;

            return View(rentals);
        }

        //Action method to retrieve all rentals
        public IActionResult Allrentals()
        {
            //Retrieving all rentals and including related car info and user
            var rentals = _dbContext.Rental.Include(r => r.CarInfo).Include(r => r.User).ToList();
            return View(rentals);
        }

        //Action method to retrieve inactive users
        public IActionResult InactiveUsers()
        {
            //Retrieving users who haven't rented a car in the last three months
            var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);

            var InactiveUsers = from r in _dbContext.Rental
                                join u in _dbContext.Users
                                on r.UserID equals u.Id
                                where r.date < threeMonthsAgo
                                select u;

            return View(InactiveUsers.ToList());
        }


        //Action method to retrieve active customer users
        public IActionResult activeUsers()
        {
            //Retrieving all customer users who are marked as regular customers
            var users = _dbContext.Users
        .Where(u => u.IsRegularCustomer)
        .ToList();

            return View(users);

        }

        // POST: Rental/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            // Get the rental by ID
            var rental = await _dbContext.Rental.FindAsync(id);

            if (rental == null)
            {
                return NotFound();
            }

            // Check if the rental is in a cancelable state (status 0 or 1)
            if (rental.RentalStatus == 0 || rental.RentalStatus == 1)
            {
                // Delete the rental record from the database
                _dbContext.Rental.Remove(rental);
                await _dbContext.SaveChangesAsync();

                // Update the availability of the rented car
                var car = await _dbContext.CarInfo.FindAsync(rental.CarID);
                if (car != null)
                {
                    car.IsAvailable = true;
                    _dbContext.CarInfo.Update(car);
                    await _dbContext.SaveChangesAsync();
                }

                return RedirectToAction("RentalHistory");
            }

            return RedirectToAction("Index", "Home");
        }



    }


}