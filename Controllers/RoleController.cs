using System.Threading.Tasks;
using IdeasTracker.Business.Uows.Interfaces;
using IdeasTracker.Constants;
using IdeasTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdeasTracker.Controllers
{
    [Authorize(Roles = Roles.ClubTenzing)]
    public class RoleController : Controller
    { 
        private readonly IUserUow _userUow;

        public RoleController(IUserUow userUow)
        { 
            _userUow = userUow;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _userUow.GetUsersAsync());
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
                await _userUow.CreateUserAsync(userModel);
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

            var user = await _userUow.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: BackLogItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userUow.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
