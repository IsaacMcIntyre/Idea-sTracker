using System.Collections.Generic;
using IdeasTracker.Business.Converters.Interfaces;
using IdeasTracker.Business.Uows.Interfaces;
using IdeasTracker.Database.Context;
using IdeasTracker.Models; 
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IdeasTracker.Database.Entities;
using IdeasTracker.Business.Constants;
using System;
using System.Net.Mail;
using System.Net;
using IdeasTracker.Business.Email.Interfaces;

namespace IdeasTracker.Business.Uows
{
    public class BackLogUow:IBackLogUow
    {
        private readonly ApplicationDbContext _context;
        private readonly IBacklogToBackLogModelConverter _backlogToBackLogModelConverter;
        private readonly IEmailSender _emailSender;


        public BackLogUow(ApplicationDbContext context, IBacklogToBackLogModelConverter backlogToBackLogModelConverter, IEmailSender emailSender)
        {
            _context = context;
            _backlogToBackLogModelConverter = backlogToBackLogModelConverter;
            _emailSender = emailSender;
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
                backlogItem.Status = IdeaStatuses.PoAssigned;
            }

            if (!string.IsNullOrWhiteSpace(backlogModel.BootcampAssigned) && string.IsNullOrWhiteSpace(backlogModel.ProductOwner))
            {
                backlogItem.Status = IdeaStatuses.BootcampAssigned;
            }

            if (!string.IsNullOrWhiteSpace(backlogModel.BootcampAssigned) && !string.IsNullOrWhiteSpace(backlogModel.ProductOwner))
            {
                backlogItem.Status = IdeaStatuses.BootcampReady;
            }


            if (!string.IsNullOrWhiteSpace(backlogModel.BootcampAssigned) &&
                !string.IsNullOrWhiteSpace(backlogModel.ProductOwner) &&
                !string.IsNullOrWhiteSpace(backlogModel.SolutionDescription) &&
                !string.IsNullOrWhiteSpace(backlogModel.Links))
            {
                backlogItem.Status = IdeaStatuses.ProjectAdoptable;
            }

            if (string.IsNullOrWhiteSpace(backlogModel.BootcampAssigned) &&
                string.IsNullOrWhiteSpace(backlogModel.ProductOwner) &&
                !backlogItem.Status.Equals(IdeaStatuses.IdeaPending))
            {
                backlogItem.Status = IdeaStatuses.IdeaAccepted;
            }

            _context.Update(backlogItem);
            await _context.SaveChangesAsync();

        }


        public async Task AcceptAdoption(BacklogModel backlogModel)
        {
            var backlogItem = await _context.BackLogs.FirstAsync(x => x.Id == backlogModel.Id);

            backlogItem.Status = IdeaStatuses.Adopted;

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
                Status = IdeaStatuses.IdeaPending
            });
            await _context.SaveChangesAsync();
        }

        public async Task AdoptIdeaAsync(BacklogModel backlogModel, string userEmail)
        { 
            var backlogiem = await _context.BackLogs.FirstAsync(x => x.Id == backlogModel.Id);
            if (backlogiem != null)
            {
                backlogiem.AdoptedBy = backlogModel.AdoptedBy;
                backlogiem.AdoptionValue = backlogModel.AdoptionValue;
                backlogiem.AdoptionReason = backlogModel.AdoptionReason;
                _context.Update(backlogiem);
                await _context.SaveChangesAsync();
                //TODO: Send Email
                _emailSender.SendAdoptionEmail(userEmail);
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
