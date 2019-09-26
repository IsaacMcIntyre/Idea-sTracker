using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdeasTracker.Models;
using Microsoft.AspNetCore.Authorization;
using IdeasTracker.Business.Uows.Interfaces;
using IdeasTracker.Constants;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System;

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerProblem,ProblemDescription,ProductOwner,Status,BootcampAssigned,SolutionDescription,Links,AdoptionEmailAddress")] BacklogModel backLogModel)
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
            var adoptableItem = await _backlogUow.GetBackLogAdoptableItem(id);
            if (adoptableItem == null)
            {
                return NotFound();
            }
            return View(adoptableItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Adopt([Bind("Id, CustomerProblem,ProblemDescription,Status,ProductOwner,Links,BootcampAssigned,SolutionDescription,AdoptedBy, AdoptionValue, AdoptionReason")] AdoptRequestModel adoptRequestModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    adoptRequestModel.Status = IdeaStatuses.AdoptionRequest;
                    string userEmail = HttpContext.User.Claims.ToList()[2].Value;
                    await _backlogUow.AdoptIdeaAsync(adoptRequestModel, userEmail);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _backlogUow.IsBackLogItemExistsAsync(adoptRequestModel.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(adoptRequestModel);
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AdoptAccept(int Id, [Bind("Id, CustomerProblem,ProblemDescription,Status,ProductOwner,Links,BootcampAssigned,SolutionDescription,AdoptedBy, AdoptionValue, AdoptionReason")] AdoptRequestModel adoptRequestModel)
        { 
            if (ModelState.IsValid)
            {
                if (Id != adoptRequestModel.Id)
                {
                    return NotFound();
                }

                try
                {
                   await _backlogUow.AcceptAdoption(adoptRequestModel);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _backlogUow.IsBackLogItemExistsAsync(adoptRequestModel.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AdoptReject(int Id, [Bind("Id, CustomerProblem,ProblemDescription,Status,ProductOwner,Links,BootcampAssigned,SolutionDescription,AdoptedBy, AdoptionValue, AdoptionReason")] AdoptRequestModel adoptRequestModel)
        {
            if (ModelState.IsValid)
            {
                if (Id != adoptRequestModel.Id)
                {
                    return NotFound();
                }

                try
                {
                    await _backlogUow.RejectAdoption(adoptRequestModel);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _backlogUow.IsBackLogItemExistsAsync(adoptRequestModel.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public async Task<IActionResult> AdoptReject([Bind("Id, AdoptedBy, AdoptionValue, AdoptionReason")] BacklogModel backlogModel)
        //{



        //}



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

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _backlogUow.DeleteBackLogItem(id);
            return RedirectToAction(nameof(Index));
        }



        [Authorize]
        public async Task<IActionResult> Export()
        {
            var exportItem = await _backlogUow.ExportBacklog();
            return File(exportItem.Item1,"text/csv", exportItem.Item2);
        } 

    }
}
