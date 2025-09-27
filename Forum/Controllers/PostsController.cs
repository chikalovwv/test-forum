using System;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int topicId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Сообщение не может быть пустым.");
            }

            string? userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            Post post = new Post
            {
                TopicId = topicId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                AuthorId = userId
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Подтянуть автора
            Post? created = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == post.Id);

            if (created == null)
            {
                return StatusCode(500);
            }

            // Вернуть частичное представление одного поста
            return PartialView("~/Views/Posts/_PostPartial.cshtml", created);
        }
    }
}