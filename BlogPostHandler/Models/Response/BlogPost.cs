
using Newtonsoft.Json;

namespace BlogPostHandler.Models.Response
{
    public class BlogPost
    {
        public int Id { get; set; }

        public string Content { get; set; } // surely pulling down a huge string as a blogpost will prove poor performance; I'll move this into its own object at some point

        public bool Blurb { get; set; }

        public Metadata Metadata;

        public static int BlurbLength = 50;

        public BlogPost(int id, string title, string content, string[] tags, bool blurb = false)
        {
            Id = id;
            Content = content;
            Blurb = blurb;

            Metadata = new Metadata(id)
            {
                Title = title,
                Tags = tags
            };
        }

        [JsonConstructor]
        public BlogPost(int id, bool blurb = false)
        {
            Id = id;
            Blurb = blurb;

            Metadata = new Metadata(id);
        }
    }
}
