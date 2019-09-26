using System.Threading.Tasks;
using IdeasTracker.Business.Uows.Interfaces;
using IdeasTracker.Constants;
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
        public async Task<IActionResult> Edit(int? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Role")] UserModel userModel)
        {
            if (ModelState.IsValid)
            { 
                if (id != userModel.Id)
                {
                    return NotFound();
                }
                try
                {
                    await _userUow.EditUserAsync(userModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _userUow.IsUserExistsAsync(userModel.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userModel);
        }
    }
}
