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
            EnvironmentHandler env = EnvironmentHandler.GetEnvironmentHandler();

            string bucketName = env.GetVariable("BucketName");
            string bucketRegionString = env.GetVariable("BucketRegion");
            string postsDirectory = env.GetVariable("PostsDirectory");
            string imagesDirectory = env.GetVariable("ImagesDirectory");
            string metaDirectory = env.GetVariable("MetaDirectory");
            
            BlogPostS3Access blogPostAccess = new BlogPostS3Access(bucketName, bucketRegionString);

            var blogPostResonse = blogPostAccess.GetBlogPost(blogPost);
            blogPostResonse.Wait();
            blogPost = blogPostResonse.Result;

            // get related posts
            TagFileS3Access tagFileAccess = new TagFileS3Access();
            var relatedPostsResponse = tagFileAccess.GetBlogPostIdsFromTags(blogPost.Metadata.Tags);
            relatedPostsResponse.Wait();
            blogPost.RelatedPosts = relatedPostsResponse.Result;

            // remove all related posts which are the current post
            blogPost.RelatedPosts.RemoveAll(b => b.Id == blogPost.Id);

            // populate related posts objects
            for (int i = 0; i < blogPost.RelatedPosts.Count; i++)
            {
                var relatedPostResponse = blogPostAccess.GetBlogPost(blogPost.RelatedPosts[i]);
                relatedPostResponse.Wait();
                blogPost.RelatedPosts[i] = relatedPostResponse.Result;
            }            

            return blogPost;
                        
        }
    }
}
