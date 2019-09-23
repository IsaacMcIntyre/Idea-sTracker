using System;

namespace IdeasTracker.Models
{
    public class BackLog
    {
        public int Id { get; set; }
        public string Artist { get; set; }
        public string Venue { get; set; }
        public DateTime ShowDate { get; set; }
        public string EnteredBy { get; set; }
    }
}