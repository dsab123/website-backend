
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BlogPostHandler.Models
{
    public class BlogPost
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; } // surely pulling down a huge string as a blogpost will prove poor performance; I'll move this into its own object at some point

        [JsonProperty(PropertyName = "blurb")]
        public bool Blurb { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata;

        [JsonProperty(PropertyName = "relatedPosts")]
        public List<BlogPost> RelatedPosts;

        public static int BlurbLength = 75; // how many chars to show in blurbs

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
