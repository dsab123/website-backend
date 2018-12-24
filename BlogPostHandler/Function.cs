using System;
using Amazon.Lambda.Core;
using BlogPostHandler.AccessLayers;
using BlogPostHandler.Models.Request;
using BlogPostHandler.Models.Response;
using BlogPostHandler.Models.Utility;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace BlogPostHandler
{
    public class Function
    {
        /// <summary>
        /// Entry point for the BlogPost Handler.
        /// </summary>
        /// <param name="input">The Incoming BlogPostId</param>
        /// <param name="context">The Lambda event context</param>
        /// <returns></returns>
        public BlogPost FunctionHandler(BlogPostId input, ILambdaContext context)
        {
            // error handling? how? all I need is the input.Id which is an int..
            LambdaLogger.Log("BlogPostHandler Lambda Started");

            // Config/Initialization
            EnvironmentHandler env = new EnvironmentHandler();

            string bucketName = env.GetVariable("BucketName");
            string keyName = input.Id.ToString();
            string bucketRegionString = env.GetVariable("BucketRegion");
            string postsDirectory = env.GetVariable("PostsDirectory");
            string imagesDirectory = env.GetVariable("ImagesDirectory");
            string metaDirectory = env.GetVariable("MetaDirectory");
            
            S3Access access = new S3Access(bucketName, bucketRegionString);

            BlogPost post = new BlogPost(input.Id);

            // get post contents
            var contents = access.GetBlogPostContent(post, postsDirectory, keyName);
            contents.Wait();
            post.Content = contents.Result;

            // get post metadata
            var metadata = access.GetMetadata(post.Metadata, metaDirectory, keyName);
            metadata.Wait();
            post.Metadata = metadata.Result;

            if (post != null)
            {
                return post;
            }
            else
            {
                return null;
            }
                        
        }
    }
}
