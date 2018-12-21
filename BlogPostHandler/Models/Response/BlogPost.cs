
namespace BlogPostHandler.Models.Response
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Content; // surely pulling down a huge string as a blogpost will prove poor performance; I'll move this into its own object
        public Metadata Metadata;

        public BlogPost(int id, string title, string content, string[] tags)
        {
            Id = id;
            Content = content;

            Metadata = new Metadata(id);
            Metadata.Title = title;
            Metadata.Tags = tags;
        }

        public BlogPost(int id)
        {
            Id = id;

            Metadata = new Metadata(id);
        }
    }
}
