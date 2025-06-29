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
        public async Task<IActionResult> Update(int id, [Bind("UserName,Email,Password,CreatedDate")] User updatedUser)
        {

            if (id != updatedUser.UserId) return NotFound();


            var existingUser = await _context.User.FindAsync(id);
            if (existingUser == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {

                    existingUser.UserName = existingUser.UserName;
                    existingUser.Password = existingUser.Password;
                    existingUser.Email = existingUser.Email;
                    existingUser.CreatedDate = existingUser.CreatedDate;


                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    Console.WriteLine("No user found");
                }

                return RedirectToAction(nameof(Index));
            }

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
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var user = await _context.User
     .FirstOrDefaultAsync(t => t.UserId == id);
            user.isActive = false; // Set isActive to false instead of deleting
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        private bool UsertExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }

    }
}
