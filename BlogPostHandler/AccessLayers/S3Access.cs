using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using BlogPostHandler.Utility;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlogPostHandler.AccessLayers
{
    public abstract class S3Access
    {
        public IMyAmazonS3Client S3Client { get; set; }
        public ILambdaLogger Logger { get; set; }
        public AmazonS3Config S3Config { get; set; }

        public S3Access()
        {
            S3Config = new AmazonS3Config();
            RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(EnvironmentHandler.GetEnvironmentHandler().GetVariable("BucketRegion"));
            S3Config.RegionEndpoint = bucketRegion;

            S3Client = new MyAmazonS3Client(S3Config);
        }

        /// <summary>
        /// Base S3 Retrieval method; returns the contents of the file in string format.
        /// </summary>
        /// <param name="request">A GetObjectRequest containing BucketName and Key values</param>
        /// <returns></returns>
        public virtual async Task<string> GetObject(GetObjectRequest request)
        {
            string content = string.Empty;

            try
            {
                using (GetObjectResponse response = await S3Client.GetObjectAsync(request))
                {
                    using (Stream responseStream = response.ResponseStream)
                    {
                        using (var reader = new StreamReader(response.ResponseStream))
                        {
                            if (response.HttpStatusCode != HttpStatusCode.OK)
                            {
                                // do what here? log warning once I get CloudWatch or equivalent set up
                            }

                            content = await reader.ReadToEndAsync();
                        }
                    }
                }
            }
            catch (AmazonS3Exception s3Ex)
            {
                Logger.Log(ExceptionLogFormatter.FormatExceptionLogMessage(request, s3Ex));

                content = null;
            }
            catch (Exception ex)
            {
                Logger.Log(ExceptionLogFormatter.FormatExceptionLogMessage(ex));

                content = null;
            }
            
            return content;
        }
    }
}
