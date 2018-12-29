using NUnit.Framework;
using Amazon.Lambda.TestUtilities;
using BlogPostHandler.Models;
using BlogPostHandler.Utility;

namespace BlogPostHandler.Tests
{
    [TestFixture]
    public class FunctionTest
    {
        [Test]
        [Ignore("e2e tests not set up yet")]
        public void SanityTest_IdOfOne_ReturnsTrue()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            EnvironmentHandler.GetEnvironmentHandler().SetVariable("bucketRegion", "test");
            EnvironmentHandler.GetEnvironmentHandler().SetVariable("BucketName", "test");
            EnvironmentHandler.GetEnvironmentHandler().SetVariable("BucketRegion", "test");
            EnvironmentHandler.GetEnvironmentHandler().SetVariable("PostsDirectory", "test");
            EnvironmentHandler.GetEnvironmentHandler().SetVariable("ImagesDirectory", "test");
            EnvironmentHandler.GetEnvironmentHandler().SetVariable("MetaDirectory", "test");


            var idOfOne = function.FunctionHandler(new BlogPost(1), context);

            Assert.AreEqual(idOfOne, new BlogPost(1));
        }
        
    }
}
