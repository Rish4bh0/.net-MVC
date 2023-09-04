using Rentalmandu.Areas.Identity.Data;
using Rentalmandu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Linq;
using System.Threading.Tasks;

namespace Rentalmandu.Controllers
{
    //[Authorize(Roles = "Admin")] // Restrict access to Admin role
    public class StaffController : Controller
    {
        private readonly UserManager<HajurKoCarRentalUser> _userManager;

        public StaffController(UserManager<HajurKoCarRentalUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var staff = await _userManager.GetUsersInRoleAsync("staff");
            var admins = await _userManager.GetUsersInRoleAsync("admin");

            var staffWithRoles = new List<(HajurKoCarRentalUser User, IList<string> Roles)>();

            foreach (var user in staff)
            {
                var roles = await _userManager.GetRolesAsync(user);
                staffWithRoles.Add((user, roles));
            }

            foreach (var user in admins)
            {
                var roles = await _userManager.GetRolesAsync(user);
                staffWithRoles.Add((user, roles));
            }

            return View(staffWithRoles);
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            
                var user = new HajurKoCarRentalUser
                {
                    EmailConfirmed = true,
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    IsRegularCustomer = false // Default to false for staff members
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (model.role == "admin")
                    {
                        await _userManager.AddToRoleAsync(user, "admin");
                    }

                    if (model.role == "staff")
                    {
                        await _userManager.AddToRoleAsync(user, "staff");
                    }

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            

            // If there are any validation errors, return the view with the model
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new RegisterViewModel
            {
                Id = id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, RegisterViewModel model)
        {
            
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Remove existing roles and add new roles based on the model values
                    var roles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, roles);

                    if (model.role == "admin")
                    {
                        await _userManager.AddToRoleAsync(user, "admin");
                    }

                    if (model.role == "staff")
                    {
                        await _userManager.AddToRoleAsync(user, "staff");
                    }

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            

            // If there are any validation errors, return the view with the model
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new RegisterViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // If there is an error, return the view with the user
            return View(user);
        }
    }
}

