using System;
using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int TopicId { get; set; }
        public Topic? Topic { get; set; }

        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser? Author { get; set; }
    }
}