using IdeasTracker.Business.Converters.Interfaces;
using IdeasTracker.Business.Uows.Interfaces;
using IdeasTracker.Database.Context;

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
    }
}
