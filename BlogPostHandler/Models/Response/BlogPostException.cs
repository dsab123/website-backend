using Amazon.Lambda;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogPostHandler.Models.Response
{
    public class BlogPostException : AmazonLambdaException
    {
        
        public static string ErrorBlogPostTitle = "404, or something.";
        public static string ErrorBlogPostContents = "Looks like there was an issue somewhere upstream in service-layer land. Don't tell anybody.";

        public BlogPostException(string message) : base(message)
        {
        }

        public static BlogPost ReturnErrorBlogPost()
        {
            // I'd like to move these to config params, but don't want to access them from Env
            return new BlogPost(-1, ErrorBlogPostTitle, ErrorBlogPostContents, new string[] {});
        }

        public static string ReturnErrorBlogPostContents()
        {
            // I'd like to move these to config params, but don't want to access them from Env
            return ErrorBlogPostContents;
        }
    }
}
