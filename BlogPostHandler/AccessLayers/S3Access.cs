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

        public S3Access()
        {
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
    }
}
