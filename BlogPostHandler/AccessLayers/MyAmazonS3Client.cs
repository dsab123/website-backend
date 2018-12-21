using Amazon.S3;
using System;

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
    }
}
