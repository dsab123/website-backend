﻿
namespace BlogPostHandler.Models.Request
{
    public class BlogPostInput
    {
        public int Id { get; set; }
        public bool Blurb {get; set; }

        public BlogPostId(int id, bool blurb) 
        {
            Id = id;
            Blurb = blurb;
        }
    }
}