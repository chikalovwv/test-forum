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
    public class TopicsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int TopicsPageSize = 10; // темы на странице
        private const int PostsPageSize = 5;   // ответы на странице

        public TopicsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Этап 1: список тем (с пагинацией)
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            int totalTopics = await _context.Topics.CountAsync();
            int totalPages = (int)Math.Ceiling(totalTopics / (double)TopicsPageSize);

            System.Collections.Generic.List<Topic> topics = await _context.Topics
                .Include(t => t.Author)
                .Include(t => t.Posts)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * TopicsPageSize)
                .Take(TopicsPageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;

            return View(topics);
        }

        // Этап 1: создать тему
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Topic model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            string? userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            model.AuthorId = userId;
            model.CreatedAt = DateTime.UtcNow;

            _context.Topics.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = model.Id });
        }

        // Этап 2/3: детали темы + постраничные ответы
        public async Task<IActionResult> Details(int id, int page = 1)
        {
            Topic? topic = await _context.Topics
                .Include(t => t.Author)
                .Include(t => t.Posts)
                .ThenInclude(p => p.Author)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (topic == null)
            {
                return NotFound();
            }

            if (page < 1) { page = 1; }

            int totalPosts = topic.Posts.Count();
            int totalPages = (int)Math.Ceiling(totalPosts / (double)PostsPageSize);
            if (totalPages < 1) { totalPages = 1; }

            System.Collections.Generic.List<Post> posts = topic.Posts
                .OrderBy(p => p.CreatedAt)
                .Skip((page - 1) * PostsPageSize)
                .Take(PostsPageSize)
                .ToList();

            // Оставляем заголовок/контент темы и подставляем только нужные посты
            Topic viewModel = new Topic
            {
                Id = topic.Id,
                Title = topic.Title,
                Content = topic.Content,
                CreatedAt = topic.CreatedAt,
                AuthorId = topic.AuthorId,
                Author = topic.Author,
                Posts = posts
            };

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;

            return View(viewModel);
        }

        // AJAX: вернуть partial с постами (для динамической подгрузки страниц)
        [HttpGet]
        public async Task<IActionResult> LoadPosts(int topicId, int page = 1)
        {
            Topic? topic = await _context.Topics
                .Include(t => t.Posts)
                .ThenInclude(p => p.Author)
                .FirstOrDefaultAsync(t => t.Id == topicId);

            if (topic == null)
            {
                return NotFound();
            }

            if (page < 1) { page = 1; }

            System.Collections.Generic.List<Post> posts = topic.Posts
                .OrderBy(p => p.CreatedAt)
                .Skip((page - 1) * PostsPageSize)
                .Take(PostsPageSize)
                .ToList();

            return PartialView("~/Views/Posts/_PostsListPartial.cshtml", posts);
        }
    }
}