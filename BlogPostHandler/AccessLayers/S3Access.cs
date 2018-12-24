using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using BlogPostHandler.Utility;
using BlogPostHandler.Models.Response;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlogPostHandler.AccessLayers
{
    public class S3Access : IS3Access, IDisposable
    {
        public IMyAmazonS3Client S3Client { get; set; }
        public AmazonS3Config S3Config { get; set; }

        protected string BucketName;

        public S3Access()
        {
        }

        public S3Access(string bucketName, string bucketRegionString)
        {
            BucketName = bucketName;

            S3Config = new AmazonS3Config();
            RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(bucketRegionString);
            S3Config.RegionEndpoint = bucketRegion;

            S3Client = new MyAmazonS3Client(S3Config);
        }

        public virtual async Task<string> GetObject(GetObjectRequest request)
        {
            string content = string.Empty;
            
            try
            {
                using (GetObjectResponse response = await S3Client.GetObjectAsync(request)) {
                    using (Stream responseStream = response.ResponseStream)
                    {
                        using (var reader = new StreamReader(response.ResponseStream, Encoding.ASCII))
                        {
                            if (response.HttpStatusCode != HttpStatusCode.OK)
                            {
                                // do what here? log warning once I get CloudWatch or equivalent set up
                            }
                            
                            content = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (AmazonS3Exception s3Ex)
            {
               LambdaLogger.Log(ExceptionLogFormatter.FormatExceptionLogMessage(request, s3Ex));
                throw new Exception("Exception - " + s3Ex.Message, s3Ex);
            } 
            catch (Exception ex)
            {
                LambdaLogger.Log(ExceptionLogFormatter.FormatExceptionLogMessage(ex));
                throw new Exception("Exception - " + ex.Message, ex);
            }

            return content;
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

            return content;
        }

        public async Task<Metadata> GetMetadata(Metadata metadata, string metaDirectory, string keyName)
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

            metadata = Metadata.ToMetadata(metadata, content);
            return metadata;
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
