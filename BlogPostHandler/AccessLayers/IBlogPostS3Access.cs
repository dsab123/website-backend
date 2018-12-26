using System.Threading.Tasks;
using Amazon.S3.Model;
using BlogPostHandler.Models.Response;

namespace BlogPostHandler.AccessLayers
{
    public interface IBlogPostS3Access
    {
        Task<string> GetObject(GetObjectRequest request);

        Task<string> GetBlogPostContent(BlogPost post, string postsDirectory, string keyName);

        Task<Metadata> GetMetadata(Metadata metadata, string metaDirectory, string keyName);
    }
}
