using IdeasTracker.Database.Entities;
using IdeasTracker.Models;

namespace IdeasTracker.Business.Converters.Interfaces

{
    public interface IBacklogToAdoptRequestModelConverter
    {
            AdoptRequestModel Convert(BackLog entity);
        
    }
}
