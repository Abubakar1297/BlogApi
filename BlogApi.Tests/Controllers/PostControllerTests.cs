using BlogApi.Controllers;
using BlogApi.Data;
using BlogApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BlogApi.Tests
{
    public class PostsControllerTests
    {
        private readonly PostsController _controller;
        private readonly BlogDbContext _context;
        private readonly IMemoryCache _cache;

        public PostsControllerTests()
        {
            var options = new DbContextOptionsBuilder<BlogDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new BlogDbContext(options);
            _cache = new MemoryCache(new MemoryCacheOptions());
            _controller = new PostsController(_context, _cache);
        }

        [Fact]
        public async Task GetPosts_ReturnsOkResult_WithListOfPosts()
        {
            _context.BlogPosts.AddRange(
                new BlogPost { Id = 3, Title = "Post 1", Author = "Author 1", Content = "Content 1", Quote = "Quote 1" },
                new BlogPost { Id = 2, Title = "Post 2", Author = "Author 2", Content = "Content 2", Quote = "Quote 2" }
            );
            await _context.SaveChangesAsync();

            var result = await _controller.GetAllPosts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var posts = Assert.IsAssignableFrom<IEnumerable<BlogPost>>(okResult.Value);
            Assert.Equal(2, posts.Count());
        }

        [Fact]
        public async Task GetPostById_ValidId_ReturnsOkResult_WithPost()
        {
            var post = new BlogPost { Id = 1, Title = "Post 1", Author = "Author 1", Content = "Content 1", Quote = "Quote 1" };
            _context.BlogPosts.Add(post);
            await _context.SaveChangesAsync();

            var result = await _controller.GetPostById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPost = Assert.IsType<BlogPost>(okResult.Value);
            Assert.Equal("Post 1", returnedPost.Title);
        }

        [Fact]
        public async Task CreatePost_ValidPost_ReturnsCreatedAtActionResult()
        {
            var post = new BlogPost { Title = "New Post", Author = "New Author", Content = "New Content", Quote = "New Quote" };

            var result = await _controller.CreatePost(post);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetPostById", createdResult.ActionName);
        }
    }
}
