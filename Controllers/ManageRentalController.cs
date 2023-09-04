using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rentalmandu.Areas.Identity.Data;
using Rentalmandu.Models;

namespace Rentalmandu.Controllers
{
    public class ManageRentalController : Controller
    {
        private readonly HajurKoCarRentalDbContext _context;

        public ManageRentalController(HajurKoCarRentalDbContext context)
        {
            _context = context;
        }

        // GET: ViewRentals
        public async Task<IActionResult> Index()
        {
            var hajurKoCarRentalDbContext = _context.Rental.Include(r => r.CarInfo);
            return View(await hajurKoCarRentalDbContext.ToListAsync());
        }

        // GET: ViewRentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Rental == null)
            {
                return NotFound();
            }

            var rental = await _context.Rental
                .Include(r => r.CarInfo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // GET: ViewRentals/Create
        public IActionResult Create()
        {
            ViewData["CarID"] = new SelectList(_context.CarInfo, "id", "CarName");
            return View();
        }

        // POST: ViewRentals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CarID,UserID,AuthorizedBy,date,Fee,RentalStatus")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rental);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarID"] = new SelectList(_context.CarInfo, "id", "CarName", rental.CarID);
            return View(rental);
        }

        // GET: ViewRentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Rental == null)
            {
                return NotFound();
            }

            var rental = await _context.Rental.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }
            ViewData["CarID"] = new SelectList(_context.CarInfo, "id", "CarName", rental.CarID);
            return View(rental);
        }

        // POST: ViewRentals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CarID,UserID,AuthorizedBy,date,Fee,RentalStatus")] Rental rental)
        {
            if (id != rental.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rental);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarID"] = new SelectList(_context.CarInfo, "id", "CarName", rental.CarID);
            return View(rental);
        }

        // GET: ViewRentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Rental == null)
            {
                return NotFound();
            }

            var rental = await _context.Rental
                .Include(r => r.CarInfo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: ViewRentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Rental == null)
            {
                return Problem("Entity set 'HajurKoCarRentalDbContext.Rental'  is null.");
            }
            var rental = await _context.Rental.FindAsync(id);
            if (rental != null)
            {
                _context.Rental.Remove(rental);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ViewRentals/AcceptRequest/5
        public async Task<IActionResult> AcceptRequest(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rental.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            if (rental.RentalStatus == 0)
            {
                rental.RentalStatus = 1; // Update RentalStatus to "accepted"
                rental.AuthorizedBy = User.Identity.Name; // Set AuthorizedBy to the username of the user who approved the request
                // Create a new notification
                var notification = new Notification
                {
                    Title = "Rental Request Accepted",
                    Message = $"Your rental request with ID {rental.Id} has been accepted successfully.",
                    RentalID = rental.Id,
                    UserID = rental.UserID
                };

                _context.Notification.Add(notification);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ViewRentals/ReturnRent/5
        public async Task<IActionResult> ReturnRent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rental.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            if (rental.RentalStatus == 1)
            {
                rental.RentalStatus = 2; // Update RentalStatus to "returned"
                await _context.SaveChangesAsync();

                // Update the IsAvailable property of the returned car to true
                var returnedCar = await _context.CarInfo.FindAsync(rental.CarID);
                if (returnedCar != null)
                {
                    returnedCar.IsAvailable = true;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }




        private bool RentalExists(int id)
        {
          return (_context.Rental?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
