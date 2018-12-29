using System;
using Amazon.Lambda.Core;
using BlogPostHandler.AccessLayers;
using BlogPostHandler.Models;
using BlogPostHandler.Utility;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace BlogPostHandler
{
    public class Function
    {
        /// <summary>
        /// Entry point for the BlogPost Handler.
        /// </summary>
        /// <param name="blogPost">The Incoming BlogPost</param>
        /// <param name="context">The Lambda event context</param>
        /// <returns></returns>
        public BlogPost FunctionHandler(BlogPost blogPost, ILambdaContext context)
        {
            // not using a separate logic layer because this _is_ the logic layer
            LambdaLogger.Log("BlogPostHandler Lambda Started");

            // Config/Initialization
            EnvironmentHandler env = new EnvironmentHandler();

            string bucketName = env.GetVariable("BucketName");
            string bucketRegionString = env.GetVariable("BucketRegion");
            string postsDirectory = env.GetVariable("PostsDirectory");
            string imagesDirectory = env.GetVariable("ImagesDirectory");
            string metaDirectory = env.GetVariable("MetaDirectory");
            
            BlogPostS3Access blogPostAccess = new BlogPostS3Access(bucketName, bucketRegionString);
            
            // get post contents
            string keyName = blogPost.Id.ToString();
            var contents = blogPostAccess.GetBlogPostContent(blogPost, postsDirectory, keyName); //TODO remove keyName since its the id of the blogpost
            contents.Wait();
            blogPost.Content = contents.Result;

            // get post metadata
            var metadata = blogPostAccess.GetBlogPostMetadata(blogPost, metaDirectory, keyName);
            metadata.Wait();
            blogPost.Metadata = metadata.Result;

            // get related posts
            TagFileS3Access tagFileAccess = new TagFileS3Access();
            var relatedPosts = tagFileAccess.GetBlogPostIdsFromTags(blogPost.Metadata.Tags);
            
            return blogPost;
                        
        }
    }
}
