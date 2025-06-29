using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;

namespace OmintakProduction.Controllers
{
    
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.User.Where(u=> u.isActive==true).ToList();

            return View(users);
        }
        public IActionResult GetIndividualUser(int id)
        {
            var user = _context.User.Where(u => u.UserId == id).ToList();

            return View(user);
        }

        public IActionResult GetAllUsers()
        {
            var users = _context.User.ToList();

            return View(users);
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.User.FindAsync(id);

            if (user == null) return NotFound();

            return View("~/Views/User/Update.cshtml", user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("UserId,UserName,Email,Password,CreatedDate,isActive")] User updatedUser) // Make sure UserId is bound
        {
            if (id != updatedUser.UserId) return NotFound();

            var existingUser = await _context.User.FindAsync(id);
            if (existingUser == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Correctly assign values from updatedUser to existingUser
                    existingUser.UserName = updatedUser.UserName;
                    existingUser.Email = updatedUser.Email;
                    existingUser.Password = updatedUser.Password; // Be careful with password handling in real apps (hashing!)
                    existingUser.CreatedDate = updatedUser.CreatedDate;
                    existingUser.isActive = updatedUser.isActive; // Assuming you want to update isActive as well

                    _context.Update(existingUser); // Or _context.Entry(existingUser).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsertExists(updatedUser.UserId)) // Check if the user still exists
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Re-throw if it's another concurrency issue
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // If ModelState is not valid, return the view with the invalid model
            return View("~/Views/User/Update.cshtml", updatedUser);
        }



        public IActionResult Delete()
        {

            return View();
        }

        [HttpGet]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.User
                .FirstOrDefaultAsync(t => t.UserId == id);

            return user == null ? NotFound() : View("~/Views/User/Delete.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null) // <-- This is the crucial null check
            {
                return NotFound(); // Or RedirectToAction(nameof(Index)); with a message
            }

            user.isActive = false;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool UsertExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }

    }
}
