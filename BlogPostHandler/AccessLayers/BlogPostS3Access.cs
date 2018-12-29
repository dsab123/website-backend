using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using BlogPostHandler.Models;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BlogPostHandler.AccessLayers
{
    public class BlogPostS3Access : S3Access, IBlogPostS3Access, IDisposable
    {
        public AmazonS3Config S3Config { get; set; }

        protected string BucketName;

        public BlogPostS3Access()
        {
        }

        public BlogPostS3Access(string bucketName, string bucketRegionString)
        {
            BucketName = bucketName;

            S3Config = new AmazonS3Config();
            RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(bucketRegionString);
            S3Config.RegionEndpoint = bucketRegion;

            S3Client = new MyAmazonS3Client(S3Config);
        }

        public async Task<string> GetBlogPostContent(BlogPost post, string postsDirectory, string keyName)
        {
            if (postsDirectory == null || keyName == null)
            {
                throw new ArgumentNullException("postsDirectory or keyName cannot be null.");
            }

            var getRequest = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = postsDirectory + keyName + ".md", // NEED A FILE TYPE RESOLVER class somehow
            };

            var content = await GetObject(getRequest);

            // check blurb qs value
            if (post.Blurb == true)
            {
                content = content.Substring(0, content.Length > BlogPost.BlurbLength ? BlogPost.BlurbLength : content.Length);
            }

            return content;
        }

        public async Task<Metadata> GetBlogPostMetadata(BlogPost blogPost, string metaDirectory, string keyName)
        {
            if (metaDirectory == null || keyName == null)
            {
                throw new ArgumentNullException("metaDirectory or keyName cannot be null.");
            }

            var getRequest = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = metaDirectory + keyName + ".md",
            };

            var content = await GetObject(getRequest);

            blogPost.Metadata = Metadata.ToMetadata(blogPost.Metadata, content);
            return blogPost.Metadata;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool freeManagedResources)
        {
            if (freeManagedResources)
            {
                
                if (this.S3Client!= null)
                {
                    this.S3Client.Dispose();
                    this.S3Client = null;
                }
            }
        }
    }
}
