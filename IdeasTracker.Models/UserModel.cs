using System.ComponentModel.DataAnnotations;

namespace IdeasTracker.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
