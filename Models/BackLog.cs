using System;
using System.ComponentModel.DataAnnotations;

namespace IdeasTracker.Models
{
    public class BackLog
    {
        //[Display(Name="Id")]
        public int Id { get; set; }
        public string RaisedBy { get; set; }
        public string CustomerProblem { get; set; }
        public string ProblemDescription { get; set; }
        public string ProductOwner { get; set; }
        public string Status { get; set; }
        public string BootcampAssigned { get; set; }
        public string SolutionDescription { get; set; }
        public string Links { get; set; }
        public int IsAdopted { get; set; }
        public String AdoptedBy { get; set; }
        public String AdoptionValue { get; set; }
        public String AdoptionReason { get; set; }

    }
}