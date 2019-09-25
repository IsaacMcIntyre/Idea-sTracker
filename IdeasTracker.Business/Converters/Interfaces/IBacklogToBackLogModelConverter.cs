using System;
using IdeasTracker.Database.Entities;
using IdeasTracker.Models;

namespace IdeasTracker.Business.Converters.Interfaces
{
    public interface IBacklogToBackLogModelConverter
    {
		BacklogModel Convert(BackLog entity);
	}
}
