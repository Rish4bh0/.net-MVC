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
    public class OfferController : Controller
    {
        private readonly HajurKoCarRentalDbContext _context;

        public OfferController(HajurKoCarRentalDbContext context)
        {
            _context = context;
        }

        // GET: Offer
        public async Task<IActionResult> Index()
        {
            var hajurKoCarRentalDbContext = _context.Offer.Include(o => o.CarInfo);
            return View(await hajurKoCarRentalDbContext.ToListAsync());
        }

        // GET: Offer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Offer == null)
            {
                return NotFound();
            }

            var offer = await _context.Offer
                .Include(o => o.CarInfo)
                .FirstOrDefaultAsync(m => m.id == id);
            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // GET: Offer/Create
        public IActionResult Create()
        {
            ViewData["CarID"] = new SelectList(_context.CarInfo, "id", "CarName");
            return View();
        }

        // POST: Offer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Title,Description,CarID,Discount,StartDate,EndDate")] Offer offer)
        {
            
                _context.Add(offer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            
        }

        // GET: Offer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Offer == null)
            {
                return NotFound();
            }

            var offer = await _context.Offer.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }
            ViewData["CarID"] = new SelectList(_context.CarInfo, "id", "CarName", offer.CarID);
            return View(offer);
        }

        // POST: Offer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Title,Description,CarID,Discount,StartDate,EndDate")] Offer offer)
        {
            if (id != offer.id)
            {
                return NotFound();
            }

            
                try
                {
                    _context.Update(offer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfferExists(offer.id))
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

        // GET: Offer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Offer == null)
            {
                return NotFound();
            }

            var offer = await _context.Offer
                .Include(o => o.CarInfo)
                .FirstOrDefaultAsync(m => m.id == id);
            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // POST: Offer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Offer == null)
            {
                return Problem("Entity set 'HajurKoCarRentalDbContext.Offer'  is null.");
            }
            var offer = await _context.Offer.FindAsync(id);
            if (offer != null)
            {
                _context.Offer.Remove(offer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OfferExists(int id)
        {
          return (_context.Offer?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
