using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdeasTracker.Data;
using IdeasTracker.Models;
using Microsoft.AspNetCore.Authorization;
using IdeasTracker.Business.Enums;

namespace IdeasTracker.Controllers
{
    [Authorize]
    public class BackLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BackLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = Roles.ClubTenzing)] 
        public async Task<IActionResult> Index()
        {

            return View(await _context.BackLogs.ToListAsync());
        }

        // GET: BackLogItem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backLogItem = await _context.BackLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (backLogItem == null)
            {
                return NotFound();
            }

            return View(backLogItem);
        }

        // GET: BackLogItem/Create 
        public IActionResult Create()
        {
            return View();
        }

        // POST: BackLogItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Create([Bind("CustomerProblem,ProblemDescription")] BackLog backLog)
        {
            backLog.RaisedBy = User.Identity.Name;
            backLog.Status = "Pending";


            if (ModelState.IsValid)
            {
                _context.Add(backLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(backLog);
        }

        // GET: BackLogItem/Edit/5 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backLogItem = await _context.BackLogs.FindAsync(id);
            if (backLogItem == null)
            {
                return NotFound();
            }
            return View(backLogItem);
        }

        // POST: BacklogItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Edit(int id, [Bind("Id,Artist,Venue,ShowDate")] BackLog backLog)
        {
            backLog.RaisedBy = User.Identity.Name;
            if (id != backLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(backLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BackLogItemExists(backLog.Id))
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
            return View(backLog);
        }

        // GET: BackLogItem/Delete/5 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backLogItem = await _context.BackLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (backLogItem == null)
            {
                return NotFound();
            }

            return View(backLogItem);
        }

        // POST: BackLogItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var backLogItem = await _context.BackLogs.FindAsync(id);
            _context.BackLogs.Remove(backLogItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BackLogItemExists(int id)
        {
            return _context.BackLogs.Any(e => e.Id == id);
        }
    }
}
