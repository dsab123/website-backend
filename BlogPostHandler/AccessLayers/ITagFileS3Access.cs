using BlogPostHandler.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlogPostHandler.AccessLayers
{
    public interface ITagFileS3Access
    {
        Task<TagFile> GetTagFile();

        Task<List<BlogPost>> GetBlogPostIdsFromTags(string[] tags);
    }
}
