using Amazon.S3;
using Amazon.S3.Model;
using BlogPostHandler.AccessLayers;
using BlogPostHandler.Models;
using System.Threading;
using System.Threading.Tasks;

namespace BlogPostHandler.Tests.Unit
{
    // using extract and override for most of these
    public class FakeBlogPostS3Access : BlogPostS3Access
    {
        public string Expected { get; set; }

        public async override Task<string> GetObject(GetObjectRequest request)
        {
            return await Task.FromResult(Expected);
        }
    }

    public class FakeMyAmazonS3ClientThatReturnsNull : MyAmazonS3Client
    {
        public FakeMyAmazonS3ClientThatReturnsNull(AmazonS3Config config) : base(config)
        {
        }

        public override Task<GetObjectResponse> GetObjectAsync(GetObjectRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<GetObjectResponse>(null);
        }
    }

    public class FakeS3Access : S3Access
    {
        public FakeS3Access()
        {
            //nothing, hoo ha
        }
    }
}
