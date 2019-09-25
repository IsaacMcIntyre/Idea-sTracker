using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdeasTracker.Business.Enums;
using IdeasTracker.Business.Uows.Interfaces;
using IdeasTracker.Data;
using IdeasTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdeasTracker.Controllers
{
    [Authorize(Roles = Roles.ClubTenzing)]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoleUow _roleUow;

        public RoleController(ApplicationDbContext context, IRoleUow roleUow)
        {
            _context = context;
            _roleUow = roleUow;
        }

        public async Task<IActionResult> Index()
        {
            var users = new List<UserModel>();
            await _context.Users.ForEachAsync(user =>
            {
                users.Add(new UserModel
                {
                    Email = user.Email,
                    Id = user.Id,
                    Name = user.Name,
                    Role = user.Role
                });
            });
            return View(users);
        } 
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Create([Bind("Name, Email, Role")] UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(new User
                {
                    Email = userModel.Email,
                    Name = userModel.Name,
                    Role = userModel.Role
                });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(userModel);
        } 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(new UserModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role
            });
        }

        // POST: BackLogItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
