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

        public static EnvironmentHandler Instance;

        public static EnvironmentHandler GetEnvironmentHandler()
        {
            if (Instance != null)
            {
                return Instance;
            }
            else
            {
                Instance = new EnvironmentHandler();
                return Instance;
            }
        }

        public EnvironmentHandler()
        {
        }

        public EnvironmentHandler(string bucketName, string bucketRegionString, string postsDirectory,
            string imagesDirectory, string metaDirectory)
        {
            Instance = new EnvironmentHandler();

            Instance.BucketName = bucketName;
            Instance.BucketRegionString = bucketRegionString;
            Instance.PostsDirectory = postsDirectory;
            Instance.ImagesDirectory = imagesDirectory;
            Instance.MetaDirectory = metaDirectory;
        }

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
    }
}
