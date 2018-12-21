
namespace BlogPostHandler.Models.Request
{
    public class BlogPostId
    {
        public int Id { get; set; }

        public BlogPostId(int id)
        {
            Id = id;
        }
    }
}