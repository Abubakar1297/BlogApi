using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string Content { get; set; }

        [MaxLength(100)]
        public string Quote { get; set; }
    }

}
