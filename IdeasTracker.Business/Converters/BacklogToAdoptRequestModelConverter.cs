using System;
using IdeasTracker.Business.Converters.Interfaces;
using IdeasTracker.Database.Entities;
using IdeasTracker.Models;

namespace IdeasTracker.Business.Converters
{
    public class BacklogToAdoptRequestModelCoverter: IBacklogToAdoptRequestModelConverter
    {
        public AdoptRequestModel Convert(BackLog entity)
        {
            return new AdoptRequestModel
            {
                AdoptedBy = entity.AdoptedBy,
                Id = entity.Id,
                AdoptionReason = entity.AdoptionReason,
                AdoptionValue = entity.AdoptionValue,
                BootcampAssigned = entity.BootcampAssigned,
                CustomerProblem = entity.CustomerProblem,
                IsAdopted = entity.IsAdopted,
                Links = entity.Links,
                ProblemDescription = entity.ProblemDescription,
                ProductOwner = entity.ProductOwner,
                RaisedBy = entity.RaisedBy,
                SolutionDescription = entity.SolutionDescription,
                Status = entity.Status,
                AdoptionEmailAddress = entity.AdoptionEmailAddress
            };
        }
    }
}
