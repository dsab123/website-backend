using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Threading.Tasks;

namespace BlogPostHandler.AccessLayers
{
    public class MyAmazonS3Client : AmazonS3Client, IMyAmazonS3Client, IDisposable
    {
        public MyAmazonS3Client(AmazonS3Config config) : base(config)
        {
        }

        public MyAmazonS3Client()
        {
        }

        public virtual async Task<GetObjectResponse> GetObjectAsync(GetObjectRequest request)
        {
            return await base.GetObjectAsync(request);
        }
    }
}
