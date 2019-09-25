using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdeasTracker.Models;
using Microsoft.AspNetCore.Authorization;
using IdeasTracker.Email;
using IdeasTracker.Business.Uows.Interfaces;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace IdeasTracker.Controllers
{
    [Authorize]
    public class BackLogController : Controller
    {
        private readonly IBackLogUow _backlogUow;


        public BackLogController(IBackLogUow backlogUow)
        {
            _backlogUow = backlogUow;
        }

        [Authorize] 
        public async Task<IActionResult> Index()
        {

            var items = await _backlogUow.GetAllBackLogItemsAsync();
            items.ForEach(x => x.Status = x.Status.Replace(' ', '-'));
            return View(items);
        }

        // GET: BackLogItem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backLogItem = await _backlogUow.GetBackLogItemAsync(id);
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
        public async Task<IActionResult> Create([Bind("CustomerProblem,ProblemDescription")] CreateIdeaModel createIdeaModel)
        {
            if (ModelState.IsValid)
            {
                createIdeaModel.RaisedBy = User.Identity.Name;
                await _backlogUow.CreateBackLogItemAsync(createIdeaModel);
                return RedirectToAction(nameof(Index));
            }
            return View(createIdeaModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> EditIdea([Bind("Id,ProductOwner,BootcampAssigned,SolutionDescription, Status, Links")] BacklogModel backLogModel)
        {
            if (ModelState.IsValid)
            {
                await _backlogUow.EditBackLogItemAsync(backLogModel);
                return RedirectToAction(nameof(Index));
            }
            return View(backLogModel);
        }

        // GET: BackLogItem/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backLogItem = await _backlogUow.GetBackLogItemAsync(id);
            if (backLogItem == null)
            {
                return NotFound();
            }
            return View(backLogItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Edit(int id, [Bind("Id,Artist,Venue,ShowDate")] BacklogModel backLogModel)
        {
            if (ModelState.IsValid)
            {
                backLogModel.RaisedBy = User.Identity.Name;
                if (id != backLogModel.Id)
                {
                    return NotFound();
                }
                try
                {
                    await _backlogUow.EditBackLogItemAsync(backLogModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _backlogUow.IsBackLogItemExistsAsync(backLogModel.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(backLogModel);
        }
        // GET: BackLogItem/Edit/5
        [Authorize]
        public async Task<IActionResult> Adopt(int? id)
        {
            var backLogItem = await _backlogUow.GetBackLogItemAsync(id);
            if (backLogItem == null)
            {
                return NotFound();
            }
            return View(backLogItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Adopt([Bind("Id, AdoptedBy, AdoptionValue, AdoptionReason")] BacklogModel backlogModel) 
        {
            List<System.Security.Claims.Claim> userClaims = HttpContext.User.Claims.ToList();
            string userEmail = userClaims[2].Value;

            MailSenderFeature.SendEmail("emailsendingtestaddress@gmail.com", "There has been a new request to adopt an idea.", "Adoption Request"); //email to club tensing
            MailSenderFeature.SendEmail(userEmail, "Thank you, your adoption request has been submitted.", "Adoption Request"); //email to adopter

            if (ModelState.IsValid)
            {
                try
                {

                    await _backlogUow.AdoptIdeaAsync(backlogModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _backlogUow.IsBackLogItemExistsAsync(backlogModel.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(backlogModel);
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

            await _backlogUow.DeleteBackLogItem(id);

            return Redirect("/BackLog");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _backlogUow.DeleteBackLogItem(id);
            return RedirectToAction(nameof(Index));
        } 
        
    }
}
