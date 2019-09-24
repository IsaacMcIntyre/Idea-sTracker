using System;
using System.ComponentModel.DataAnnotations;

namespace IdeasTracker.Models
{
    public class BackLog
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Raised by")]
        public string RaisedBy { get; set; }

        [Display(Name = "Customer Problem")]
        public string CustomerProblem { get; set; }

        [Display(Name = "Problem Description")]
        public string ProblemDescription { get; set; }

        [Display(Name = "Product Owner")]
        public string ProductOwner { get; set; }

        //[Display(Name = "Raised by")]
        public string Status { get; set; }

        [Display(Name = "Bootcamp Assigned")]
        public string BootcampAssigned { get; set; }

        [Display(Name = "Solution Description")]
        public string SolutionDescription { get; set; }

        //[Display(Name = "Links")]
        public string Links { get; set; }

        //[Display(Name = "Raised by")]
        public int IsAdopted { get; set; }

        [Display(Name = "Adopted By")]
        public String AdoptedBy { get; set; }

        [Display(Name = "What value can you add?")]
        public String AdoptionValue { get; set; }

        [Display(Name = "Adoption Reason")]
        public String AdoptionReason { get; set; }

    }
}