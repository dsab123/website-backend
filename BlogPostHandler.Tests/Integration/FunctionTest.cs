using NUnit.Framework;
using Amazon.Lambda.TestUtilities;
using BlogPostHandler.Models;

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
            var idOfOne = function.FunctionHandler(new BlogPost(1), context);

            Assert.AreEqual(idOfOne, new BlogPost(1));
        }
        
    }
}
