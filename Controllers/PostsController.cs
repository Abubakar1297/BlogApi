using BlogApi.Data;
using BlogApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly BlogDbContext _context;
        private readonly IMemoryCache _cache;

        public PostsController(BlogDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET /api/posts
        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            if (!_cache.TryGetValue("BlogPosts", out List<BlogPost> posts))
            {
                posts = await _context.BlogPosts.Select(p => new BlogPost
                {
                    Id = p.Id,
                    Title = p.Title,
                    Author = p.Author,
                    Quote = p.Quote.Length > 100 ? p.Quote.Substring(0, 100) : p.Quote
                }).ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set("BlogPosts", posts, cacheOptions);
            }
            return Ok(posts);
        }

        // GET /api/posts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null) return NotFound();
            return Ok(post);
        }

        // POST /api/posts
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] BlogPost post)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.BlogPosts.Add(post);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPosts([FromQuery] string title, [FromQuery] string author)
        {
            var query = _context.BlogPosts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(p => p.Title.Contains(title));
            }

            if (!string.IsNullOrWhiteSpace(author))
            {
                query = query.Where(p => p.Author.Contains(author));
            }

            var result = await query
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Author,
                    p.Quote
                })
                .ToListAsync();

            return Ok(result);
        }

    }

}
