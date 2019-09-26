using System.ComponentModel.DataAnnotations;

namespace IdeasTracker.Models
{
    public class CreateIdeaModel
    {
        [Display(Name = "Raised By")]
        public string RaisedBy { get; set; }
        [Display(Name = "Customer Problem")]
        [Required]
        public string CustomerProblem { get; set; }
        [Display(Name = "Problem Description")]
        [Required]
        public string ProblemDescription { get; set; } 
    }
}
