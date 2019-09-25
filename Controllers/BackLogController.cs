using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdeasTracker.Data;
using IdeasTracker.Models;
using Microsoft.AspNetCore.Authorization;
using IdeasTracker.Business.Enums;
using IdeasTracker.Converters;
using IdeasTracker.Email;

namespace IdeasTracker.Controllers
{
    [Authorize]
    public class BackLogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBacklogToBackLogModelConverter _backlogToBackLogModelConverter;


        public BackLogController(ApplicationDbContext context, IBacklogToBackLogModelConverter backlogToBackLogModelConverter)
        {
            _context = context;
            _backlogToBackLogModelConverter = backlogToBackLogModelConverter;
        }

        [Authorize] 
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


        public IActionResult Create()
        {
            return View();
        }

        // POST: BackLogItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] 

        //[ValidateAntiForgeryToken]
        //[Authorize] 
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditIdea([Bind("Id,ProductOwner,BootcampAssigned,SolutionDescription, Status, Links")] BackLog backLog)
        {
            var backlogItem = await _context.BackLogs.FirstAsync(x => x.Id == backLog.Id);

            if (backLog.Status.Equals("Idea-Pending") && !string.IsNullOrWhiteSpace(backLog.ProductOwner))
            {
                backlogItem.Status = "Adopted";
                backlogItem.ProductOwner = backLog.ProductOwner;
            }
            else
            {
                backlogItem.ProductOwner = backLog.ProductOwner;
                backlogItem.Status = backLog.Status;
                backlogItem.BootcampAssigned = backLog.BootcampAssigned;
                backlogItem.SolutionDescription = backLog.SolutionDescription;
                backlogItem.Links = backLog.Links;
            }

            if (ModelState.IsValid)
            {
                _context.Update(backlogItem);
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
        // GET: BackLogItem/Edit/5
        [Authorize]
        public async Task<IActionResult> Adopt(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backLogItem = await _context.BackLogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (backLogItem == null)
            {
                return NotFound();
            }
            return View(_backlogToBackLogModelConverter.Convert(backLogItem));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Adopt([Bind("Id, AdoptedBy, AdoptionValue, AdoptionReason")] BacklogModel backLog) 
        {
            var userClaims = HttpContext.User.Claims;
            //string email = userClaims.
            MailSenderFeature.SendEmail("emailsendingtestaddress@gmail.com", "There has been a new request to adopt an idea.", "Adoption Request"); //email to club tensing
            //MailSenderFeature.SendEmail(userClaims); //email to user

            if (ModelState.IsValid)
            {
                try
                {

                    backLog.Status = "Adoption Requested";
                    var backlogiem = await _context.BackLogs.FirstAsync(x => x.Id == backLog.Id);
                    if (backlogiem == null)
                        NotFound();
                    backlogiem.AdoptedBy = backLog.AdoptedBy;
                    backlogiem.AdoptionValue = backLog.AdoptionValue;
                    backlogiem.AdoptionReason = backLog.AdoptionReason;
                    _context.Update(backlogiem);
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

        // POST: BacklogItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.


        // GET: BackLogItem/Delete/5 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backLogItem = await _context.BackLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (backLogItem != null)
            {
                _context.Remove(backLogItem);
                await _context.SaveChangesAsync();
            }

            return Redirect("/BackLog");
        }

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
