using System.ComponentModel.DataAnnotations;

namespace CodeQuestBackend.Models
{
    public class Bookmark
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
