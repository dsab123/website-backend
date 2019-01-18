using Amazon.Lambda.Core;
using BlogPostHandler.Models;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogPostHandler.Tests.Unit
{
    [TestFixture]
    public class FunctionTests
    {
        [Test]
        [Ignore("I will write integration tests when I die")]
        public void FunctionHandler_BlogPost1_ReturnsBlogPostAndRelatedPosts()
        {
            // Arrange
            BlogPost post = new BlogPost(1);

            Function function = new Function();

            var fakeLambdaContext = Substitute.For<ILambdaContext>();

            // Act
            function.FunctionHandler(post, fakeLambdaContext);

            // Assert
            
        }
    }
}
