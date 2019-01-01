
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BlogPostHandler.Models
{
    public class BlogPost
    {
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int Id { get; set; }

        [DataMember(Name = "content", EmitDefaultValue = false)]
        public string Content { get; set; } // surely pulling down a huge string as a blogpost will prove poor performance; I'll move this into its own object at some point

        [DataMember(Name = "blurb", EmitDefaultValue = false)]
        public bool Blurb { get; set; }

        [DataMember(Name = "metadata", EmitDefaultValue = false)]
        public Metadata Metadata;

        [DataMember(Name = "relatedPosts", EmitDefaultValue = false)]
        public List<BlogPost> RelatedPosts;

        public static int BlurbLength = 50; // how many chars to show in blurbs

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
