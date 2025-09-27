using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class Topic
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser? Author { get; set; }

        public List<Post> Posts { get; set; } = new List<Post>();
    }
}