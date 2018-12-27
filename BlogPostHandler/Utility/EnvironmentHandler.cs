using System;
using BlogPostHandler.Utility;

namespace BlogPostHandler.Utility
{
    public class EnvironmentHandler : IEnvironmentHandler
    {
        public string BucketName;
        public string BucketRegionString;
        public string PostsDirectory;
        public string ImagesDirectory;
        public string MetaDirectory;

        public string GetVariable(string variable)
        {
            string result = null;// string.Empty;
            try
            {
                result = Environment.GetEnvironmentVariable(variable);
            }
            catch (Exception e)
            {
            }

            return result;
        }

        public void SetVariable(string variable, string value)
        {
            Environment.SetEnvironmentVariable(variable, value);
        }

        public EnvironmentHandler()
        {
        }

        public EnvironmentHandler(string bucketName, string bucketRegionString, string postsDirectory,
            string imagesDirectory, string metaDirectory)
        {
            BucketName = bucketName;
            BucketRegionString = bucketRegionString;
            PostsDirectory = postsDirectory;
            ImagesDirectory = imagesDirectory;
            MetaDirectory = metaDirectory;
        }
    }
}
