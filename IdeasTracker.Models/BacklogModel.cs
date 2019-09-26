using System;
using System.ComponentModel.DataAnnotations;
namespace IdeasTracker.Models
{
   
    public class BacklogModel
	{

        [Display(Name = "ID")]
		public int Id { get; set; }

		[Display(Name = "Raised by")]
		public string RaisedBy { get; set; }

		[Display(Name = "Customer Problem")]
        [Required]
        public string CustomerProblem { get; set; }

		[Display(Name = "Problem Description")]
        [Required]
        public string ProblemDescription { get; set; }

		[Display(Name = "Product Owner")]
		public string ProductOwner { get; set; }
        [Required]
        public string Status { get; set; }
        public string StatusClass { get { return Status.Replace(' ', '-'); } }

        [Display(Name = "Bootcamp Assigned")]
		public string BootcampAssigned { get; set; }

		[Display(Name = "Solution Description")]
		public string SolutionDescription { get; set; }
         
		public string Links { get; set; }

		[Display(Name = "Is Adopted")]
		public int IsAdopted { get; set; }

		[Display(Name = "Adopted By")]
		public String AdoptedBy { get; set; }

		[Display(Name = "What value can you add?")]
		public string AdoptionValue { get; set; }

		[Display(Name = "Adoption Reason")]
		public String AdoptionReason { get; set; }
         
	}
}
