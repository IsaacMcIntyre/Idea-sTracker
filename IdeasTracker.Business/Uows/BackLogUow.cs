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
using System.Text;
using Newtonsoft.Json.Linq;

namespace IdeasTracker.Business.Uows
{
    public class BackLogUow:IBackLogUow
    {
        private readonly ApplicationDbContext _context;
        private readonly IBacklogToBackLogModelConverter _backlogToBackLogModelConverter;
        private readonly IEmailSender _emailSender;
        private readonly IBacklogToAdoptRequestModelConverter _backlogToAdoptRequestModelConverter;


        public BackLogUow(ApplicationDbContext context, IBacklogToBackLogModelConverter backlogToBackLogModelConverter, IEmailSender emailSender, IBacklogToAdoptRequestModelConverter backlogToAdoptRequestModelConverter)
        {
            _context = context;
            _backlogToBackLogModelConverter = backlogToBackLogModelConverter;
            _emailSender = emailSender;
            _backlogToAdoptRequestModelConverter = backlogToAdoptRequestModelConverter;
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
        public async Task<AdoptRequestModel> GetBackLogAdoptableItem(int? id)
        {
            var backLogItem = await _context.BackLogs.FirstOrDefaultAsync(m => m.Id == id);
            return _backlogToAdoptRequestModelConverter.Convert(backLogItem);
        }

        public async Task EditBackLogItemAsync(BacklogModel backlogModel)
        {
            var backlogItem = await _context.BackLogs.FirstAsync(x => x.Id == backlogModel.Id);

            backlogItem.ProductOwner = backlogModel.ProductOwner;
            backlogItem.Status = backlogModel.Status;
            backlogItem.BootcampAssigned = backlogModel.BootcampAssigned;
            backlogItem.SolutionDescription = backlogModel.SolutionDescription;
            backlogItem.Links = backlogModel.Links;
            backlogItem.AdoptionEmailAddress = backlogModel.AdoptionEmailAddress;


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


        public async Task AcceptAdoption(AdoptRequestModel adoptRequestModel, string userEmail)
        {
            var backlogItem = await _context.BackLogs.FirstAsync(x => x.Id == adoptRequestModel.Id);

            backlogItem.Status = IdeaStatuses.Adopted;
            backlogItem.AdoptedBy = adoptRequestModel.AdoptedBy;
            backlogItem.AdoptionValue = adoptRequestModel.AdoptionReason;
            backlogItem.AdoptionReason = adoptRequestModel.AdoptionReason;
            _context.Update(backlogItem);
            await _context.SaveChangesAsync();
            if (string.IsNullOrEmpty(backlogItem.AdoptionEmailAddress))
            {
                backlogItem.AdoptionEmailAddress = userEmail;
            }
            _emailSender.SendAdoptionAcceptanceEmail(backlogItem.AdoptionEmailAddress);
        }

        public async Task RejectAdoption(AdoptRequestModel adoptRequestModel, string userEmail)
        {
            var backlogItem = await _context.BackLogs.FirstAsync(x => x.Id == adoptRequestModel.Id);

            backlogItem.Status = IdeaStatuses.ProjectAdoptable;
            backlogItem.AdoptedBy = string.Empty;
            backlogItem.AdoptionValue = string.Empty;
            backlogItem.AdoptionReason = string.Empty;
            _context.Update(backlogItem);
            await _context.SaveChangesAsync();
            if (string.IsNullOrEmpty(backlogItem.AdoptionEmailAddress))
            {
                backlogItem.AdoptionEmailAddress = userEmail;
            }
            _emailSender.SendAdoptionRejectionEmail(backlogItem.AdoptionEmailAddress);
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

        public async Task AdoptIdeaAsync(AdoptRequestModel adoptRequestModel, string userEmail)
        { 
            var backlogiem = await _context.BackLogs.FirstAsync(x => x.Id == adoptRequestModel.Id);
            if (backlogiem != null)
            {
                backlogiem.AdoptedBy = adoptRequestModel.AdoptedBy;
                backlogiem.AdoptionValue = adoptRequestModel.AdoptionValue;
                backlogiem.AdoptionReason = adoptRequestModel.AdoptionReason;
                backlogiem.AdoptionEmailAddress = userEmail;
                backlogiem.Status = adoptRequestModel.Status;
                _context.Update(backlogiem);
                await _context.SaveChangesAsync(); 
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
        public async Task<(byte[],string)> ExportBacklog()
        {
            var backlogList = await GetAllBackLogItemsAsync();
            StringBuilder builder = new StringBuilder();

            bool isFirstLine = true;

            foreach (var backlogItem in backlogList)
            {
                bool isFirstCol = true;
                JObject backlogProperties = JObject.FromObject(backlogItem);
                if (isFirstLine)
                {
                    foreach (JProperty property in backlogProperties.Properties())
                    {
                        string value = property.Name;
                        builder = ExportHelper(builder, value, isFirstCol);
                        isFirstCol = false;
                    }
                    isFirstCol = true;
                    isFirstLine = false;
                    builder.Append(Environment.NewLine);
                }


                foreach (JProperty property in backlogProperties.Properties())
                {
                    string value = property.Value.ToString();

                    builder = ExportHelper(builder, value, isFirstCol);

                    isFirstCol = false;
                }

                builder.Append(Environment.NewLine); 

            } 
            var bytes = new UTF8Encoding().GetBytes(builder.ToString());
            return (bytes,$"{DateTime.Now.ToString("dd-MM-yyyy")}-ideas-backlog");
        }
        
        public async Task<bool> IsBackLogItemExistsAsync(int id)
        {
            return await _context.BackLogs.AnyAsync(e => e.Id == id);
        }
        #region Private Methods
        private StringBuilder ExportHelper(StringBuilder builder, string value, bool isFirstCol)
        {

            if (!isFirstCol)
            {
                builder.Append(",");
            }

            if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
            {
                builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));

            }
            else
            {
                builder.Append(value);

            }

            return builder;
        } 
        #endregion
    }
}
