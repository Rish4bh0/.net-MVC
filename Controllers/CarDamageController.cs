using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rentalmandu.Areas.Identity.Data;
using Rentalmandu.Models;
using System.Security.Claims;

namespace Rentalmandu.Controllers
{
    public class CarDamageController : Controller
    {
        private readonly HajurKoCarRentalDbContext _context;

        public CarDamageController(HajurKoCarRentalDbContext context)
        {
            _context = context;
        }

        // GET: CarDamage
        public async Task<IActionResult> Index()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the ID of the logged-in user
            var carDamages = await _context.CarDamage
                .Include(c => c.Rental)
                .Where(c => c.Rental.UserID == loggedInUserId) // Filter car damages by the logged-in user ID
                .ToListAsync();

            return View(carDamages);

            
        }


        // GET: CarDamage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CarDamage == null)
            {
                return NotFound();
            }

            var carDamage = await _context.CarDamage
                .Include(c => c.Rental)
                .FirstOrDefaultAsync(m => m.id == id);
            if (carDamage == null)
            {
                return NotFound();
            }

            return View(carDamage);
        }

        // GET: CarDamage/Create

        public IActionResult Create()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the ID of the logged-in user
            var rentals = _context.Rental.Where(r => r.UserID == loggedInUserId); // Fetch rentals for the logged-in user
            ViewData["RentalID"] = new SelectList(rentals, "Id", "Id");
            return View();
        }


        // POST: CarDamage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,RentalID,charge")] CarDamage carDamage)
        {
            
                _context.Add(carDamage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
        

        // GET: CarDamage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CarDamage == null)
            {
                return NotFound();
            }

            var carDamage = await _context.CarDamage.FindAsync(id);
            if (carDamage == null)
            {
                return NotFound();
            }
            ViewData["RentalID"] = new SelectList(_context.Rental, "Id", "Id", carDamage.RentalID);
            return View(carDamage);
        }

        // POST: CarDamage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,description,RentalID,charge")] CarDamage carDamage)
        {
            if (id != carDamage.id)
            {
                return NotFound();
            }

            
                try
                {
                    _context.Update(carDamage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarDamageExists(carDamage.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
           
        }

        // GET: CarDamage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CarDamage == null)
            {
                return NotFound();
            }

            var carDamage = await _context.CarDamage
                .Include(c => c.Rental)
                .FirstOrDefaultAsync(m => m.id == id);
            if (carDamage == null)
            {
                return NotFound();
            }

            return View(carDamage);
        }

        // POST: CarDamage/Paid/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Paid(int id)
        {
            var carDamage = await _context.CarDamage.FindAsync(id);
            if (carDamage == null)
            {
                return NotFound();
            }

            carDamage.IsPaid = true;
            _context.Update(carDamage);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        // POST: CarDamage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CarDamage == null)
            {
                return Problem("Entity set 'HajurKoCarRentalDbContext.CarDamage'  is null.");
            }
            var carDamage = await _context.CarDamage.FindAsync(id);
            if (carDamage != null)
            {
                _context.CarDamage.Remove(carDamage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarDamageExists(int id)
        {
            return (_context.CarDamage?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
