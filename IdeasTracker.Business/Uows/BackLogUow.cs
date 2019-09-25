using System.Collections.Generic;
using IdeasTracker.Business.Converters.Interfaces;
using IdeasTracker.Business.Uows.Interfaces;
using IdeasTracker.Database.Context;
using IdeasTracker.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IdeasTracker.Database.Entities;

namespace IdeasTracker.Business.Uows
{
    public class BackLogUow:IBackLogUow
    {
        private readonly ApplicationDbContext _context;
        private readonly IBacklogToBackLogModelConverter _backlogToBackLogModelConverter;


        public BackLogUow(ApplicationDbContext context, IBacklogToBackLogModelConverter backlogToBackLogModelConverter)
        {
            _context = context;
            _backlogToBackLogModelConverter = backlogToBackLogModelConverter;
        }

        public async Task<List<BacklogModel>> GetAllBackLogItemsAsync()
        {
            var backLogItems = new List<BacklogModel>();
            var backLogEntities = await _context.BackLogs.ToListAsync();
            backLogEntities.ForEach(item => backLogItems.Add(_backlogToBackLogModelConverter.Convert(item)));
            return backLogItems;
        }

        public async Task<BacklogModel> GetBackLogItemAsync(int? id)
        {
            var backLogItem = await _context.BackLogs.FirstOrDefaultAsync(m => m.Id == id);
            return _backlogToBackLogModelConverter.Convert(backLogItem);
        }

        public async Task EditBackLogItemAsync(BacklogModel backlogModel)
        {
            var backlogItem = await _context.BackLogs.FirstAsync(x => x.Id == backlogModel.Id);

            backlogItem.ProductOwner = backlogModel.ProductOwner;
            backlogItem.Status = backlogModel.Status;
            backlogItem.BootcampAssigned = backlogModel.BootcampAssigned;
            backlogItem.SolutionDescription = backlogModel.SolutionDescription;
            backlogItem.Links = backlogModel.Links;


            if (!string.IsNullOrWhiteSpace(backlogModel.ProductOwner) && string.IsNullOrWhiteSpace(backlogModel.BootcampAssigned))
            {
                backlogItem.Status = "PO Assigned";
            }

            if (!string.IsNullOrWhiteSpace(backlogModel.BootcampAssigned) && string.IsNullOrWhiteSpace(backlogModel.ProductOwner))
            {
                backlogItem.Status = "Bootcamp Assigned";
            }

            if (!string.IsNullOrWhiteSpace(backlogModel.BootcampAssigned) && !string.IsNullOrWhiteSpace(backlogModel.ProductOwner))
            {
                backlogItem.Status = "In Bootcamp";
            }


            if (!string.IsNullOrWhiteSpace(backlogModel.BootcampAssigned) &&
                !string.IsNullOrWhiteSpace(backlogModel.ProductOwner) &&
                !string.IsNullOrWhiteSpace(backlogModel.SolutionDescription) &&
                !string.IsNullOrWhiteSpace(backlogModel.Links))
            {
                backlogItem.Status = "Project Adoptable";
            }

            if (string.IsNullOrWhiteSpace(backlogModel.BootcampAssigned) &&
                string.IsNullOrWhiteSpace(backlogModel.ProductOwner) &&
                !backlogItem.Status.Equals("Idea Pending"))
            {
                backlogItem.Status = "Idea Accepted";
            }

            _context.Update(backlogItem);
            await _context.SaveChangesAsync();

        }

        public async Task CreateBackLogItemAsync(CreateIdeaModel createIdeaModel)
        {
            _context.Add(new BackLog
            {
                ProblemDescription = createIdeaModel.ProblemDescription,
                CustomerProblem = createIdeaModel.CustomerProblem,
                RaisedBy = createIdeaModel.RaisedBy,
                Status = "Idea Pending"
            });
            await _context.SaveChangesAsync();
        }

        public async Task AdoptIdeaAsync(BacklogModel backlogModel)
        { 
            var backlogiem = await _context.BackLogs.FirstAsync(x => x.Id == backlogModel.Id);
            if (backlogiem != null)
            {
                backlogiem.AdoptedBy = backlogModel.AdoptedBy;
                backlogiem.AdoptionValue = backlogModel.AdoptionValue;
                backlogiem.AdoptionReason = backlogModel.AdoptionReason;
                backlogiem.Status = "Adoption Requested";
                _context.Update(backlogiem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteBackLogItem(int? id)
        {
            var backLogItem = await _context.BackLogs.FirstOrDefaultAsync(m => m.Id == id);
            if (backLogItem != null)
            {
                _context.Remove(backLogItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsBackLogItemExistsAsync(int id)
        {
            return await _context.BackLogs.AnyAsync(e => e.Id == id);
        }
    }
}
