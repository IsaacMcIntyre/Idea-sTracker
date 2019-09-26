using System;
namespace IdeasTracker.Database.Entities
{
    public class BackLog
    {
        public int Id { get; set; }

        public string RaisedBy { get; set; }

        public string CustomerProblem { get; set; }

        public string ProblemDescription { get; set; }

        public string ProductOwner { get; set; }

        //[Display(Name = "Raised by")]
        public string Status { get; set; }

        public string BootcampAssigned { get; set; }

        public string SolutionDescription { get; set; }

        public string Links { get; set; }

        public int IsAdopted { get; set; }

        public string AdoptedBy { get; set; }

        public string AdoptionValue { get; set; }

        public string AdoptionReason { get; set; }

        public string AdoptionEmailAddress { get; set; }
    }
}
