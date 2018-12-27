
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlogPostHandler.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        public string Content { get; set; } // surely pulling down a huge string as a blogpost will prove poor performance; I'll move this into its own object at some point

        public bool Blurb { get; set; }

        public Metadata Metadata;

        public static int BlurbLength = 50; // how many chars to show in blurbs

        public IEnumerable<BlogPost> RelatedPosts;

        [JsonConstructor]
        public BlogPost(int id, bool blurb = false)
        {
            Id = id;
            Blurb = blurb;

            Metadata = new Metadata(id);

            RelatedPosts = new List<BlogPost>();
        }
    }
}
