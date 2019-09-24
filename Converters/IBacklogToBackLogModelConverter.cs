using System;
using IdeasTracker.Models;
namespace IdeasTracker.Converters
{
    public interface IBacklogToBackLogModelConverter
    {
		BacklogModel Convert(BackLog entity);
	}
}
