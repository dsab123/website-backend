using Amazon.S3.Model;
using BlogPostHandler.Models;

using System.Threading.Tasks;

namespace BlogPostHandler.AccessLayers
{
    public interface IBlogPostS3Access
    {
        Task<string> GetObject(GetObjectRequest request);

        Task<string> GetBlogPostContent(BlogPost post, string postsDirectory, string keyName);

        Task<Metadata> GetBlogPostMetadata(BlogPost post, string metaDirectory, string keyName);
    }
}
