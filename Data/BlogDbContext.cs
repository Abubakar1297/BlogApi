using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using BlogApi.Models;
namespace BlogApi.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

        public DbSet<BlogPost> BlogPosts { get; set; }
    }

}
