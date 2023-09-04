using Rentalmandu.Areas.Identity.Data;
using Rentalmandu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Rentalmandu.Controllers
{




    public class HomeController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<HajurKoCarRentalUser> _userManager;

        private readonly HajurKoCarRentalDbContext _context;

     

        public HomeController(ILogger<HomeController> logger, UserManager<HajurKoCarRentalUser> userManager, HajurKoCarRentalDbContext context)
        {
            _context = context;
            _logger = logger;
            this._userManager = userManager;

        }

        public async Task<IActionResult> Index()
        {
            return _context.CarInfo != null ?
                        View(await _context.CarInfo.ToListAsync()) :
                        Problem("Entity set 'HajurKoCarRentalDbContext.CarInfo'  is null.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}