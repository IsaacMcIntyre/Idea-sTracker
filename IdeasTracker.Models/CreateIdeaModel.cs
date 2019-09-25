using System.ComponentModel.DataAnnotations;

namespace IdeasTracker.Models
{
    public class CreateIdeaModel
    {
        [Display(Name = "Raised By")]
        public string RaisedBy { get; set; }
        [Display(Name = "Customer Problem")]
        public string CustomerProblem { get; set; }
        [Display(Name = "Problem Description")]
        public string ProblemDescription { get; set; } 
    }
}
