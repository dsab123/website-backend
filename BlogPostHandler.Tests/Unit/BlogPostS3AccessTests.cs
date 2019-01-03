using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using BlogPostHandler.AccessLayers;
using BlogPostHandler.Models;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace BlogPostHandler.Tests.Unit
{

    #region Utility

    public static class Utility
    {
        public static AmazonS3Config GetS3Config(string bucketRegionString)
        {
            AmazonS3Config S3Config = new AmazonS3Config();
            RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(bucketRegionString);
            S3Config.RegionEndpoint = bucketRegion;

            return S3Config;
        }
    }

    #endregion


    [TestFixture]
    public class BlogPostS3AccessTests
    {
        [OneTimeSetUp] 
        public void SetEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("PostsDirectory", "test");
            Environment.SetEnvironmentVariable("MetaDirectory", "test");
            Environment.SetEnvironmentVariable("BucketRegion", "test");
        }

        #region GetBlogPostContents Tests

        public static object[] GetBlogPostContent_Inputs = { "expectedResult",
        "A very long input that is very long", "1"};

        [Test]
        [TestCaseSource("GetBlogPostContent_Inputs")]
        public void GetBlogPostContents_SimpleString_ReturnsTrue(string expectedResult)
        {
            // Arrange
            FakeBlogPostS3Access access = new FakeBlogPostS3Access
            {
                Expected = expectedResult
            };

            // Act
            var response = access.GetBlogPostContent(new BlogPost(1), "test", "test");
            response.Wait();
            string result = response.Result;

            // Assert
            Assert.That(result.Equals(expectedResult));
        }

        [Test]
        public void GetBlogPostContents_NullKeyName_ThrowsException()
        {
            // Arrange
            BlogPostS3Access access = new BlogPostS3Access();

            // Act/Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => access.GetBlogPostContent(new BlogPost(1), "test", null));
        }

        [Test]
        public void GetBlogPostContents_NullPostsDirectory_ThrowsException()
        {
            // Arrange
            BlogPostS3Access access = new BlogPostS3Access();
            
            // Act/Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => access.GetBlogPostContent(new BlogPost(1), null, "test"));
        }



        static object[] GetBlogPostContents_BlurbSetToTrue_ReturnsBlurbOfContents_inputs = 
            {
            new object[] { "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 ", true },// 100 char string
            new object[] { "this is Bo", false } // 10 char string
        };

        [Test]
        [TestCaseSource("GetBlogPostContents_BlurbSetToTrue_ReturnsBlurbOfContents_inputs")]
        public void GetBlogPostContents_BlurbSetToTrue_ReturnsBlurbOfContents(string fakeBlogPostContents, bool isEqualToOrLessThanBlurbLength)
        {
            // Arrange
            FakeBlogPostS3Access access = new FakeBlogPostS3Access();
            // I'd like to make the blurb length not hardcoded to 50 here; else if I change it this test will break
            access.Expected = fakeBlogPostContents;

            // Act
            var response = access.GetBlogPostContent(new BlogPost(1, true), "test", "test");
            response.Wait();
            string result = response.Result;

            // Assert
            if (isEqualToOrLessThanBlurbLength)
            {
                Assert.That(result.Length == BlogPost.BlurbLength);
            }
            else
            {
                Assert.That(result.Length <= BlogPost.BlurbLength);
            }
        }

        // NEED TO WRITE THIS TEST
        [Test]
        [Ignore("I need to write this when I have 5 minutes")]
        public void GetBlogPostContent_NullFromGetObject_ReturnsNull()
        {
            // Arrange

            // Act

            // Assert
        }

        #endregion

        #region GetBlogPostMetadata Tests

        public static object[] GetBlogPostMetadata_Inputs = {
           "title\ntag1,tag2,tag3",
           "A Very Long Title that is long\nTag A, Tag B, Tag 100"
        };

        [Test]
        [TestCaseSource("GetBlogPostMetadata_Inputs")]
        public void GetBlogPostMetadata_SimpleString_ReturnsTrue(string expectedResult)
        {
            // Arrange
            FakeBlogPostS3Access access = new FakeBlogPostS3Access
            {
                Expected = expectedResult
            };

            // Act
            var response = access.GetBlogPostMetadata(new BlogPost(1), "test", "test");
            response.Wait();
            Metadata result = response.Result;

            // Assert
            Assert.That(result.Equals(Metadata.ToMetadata(result, expectedResult)));
        }

        [Test]
        public void GetBlogPostMetadata_NullMetaDirectory_ThrowsException()
        {
            // Arrange
            BlogPostS3Access access = new BlogPostS3Access();

            // Act/Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => access.GetBlogPostMetadata(new BlogPost(1), "test", null));
        }

        [Test]
        public void GetBlogPostMetadata_NullKeyName_ThrowsException()
        {
            // Arrange
            BlogPostS3Access access = new BlogPostS3Access();

            // Act/Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => access.GetBlogPostMetadata(new BlogPost(1), null, "test"));
        }

        #endregion

        #region ToMetadata Tests

        [Test]
        public void ToMetadata_NullInput_ReturnsNull()
        {
            // Arrange/Act/Assert
            Assert.That(Metadata.ToMetadata(new Metadata(-1), null) == null);
        }

        #endregion


        #region GetBlogPost Tests 

        [Test]
        public void GetBlogPost_NormalInput_ReturnsTrue()
        {
            // Arrange
            var FakeBlogPostAccess = Substitute.ForPartsOf<BlogPostS3Access>(new object[] { "test", "test" });

            FakeBlogPostAccess.Configure().GetBlogPostContent(new BlogPost(1), "test", "test").ReturnsForAnyArgs(Task.FromResult("Some sweer content, yo"));

            var meta = Task.FromResult<Metadata>(new Metadata(1, "title", new string[] { "tag1", "tag2" }));
            FakeBlogPostAccess.Configure().GetBlogPostMetadata(new BlogPost(1), "test", "test").ReturnsForAnyArgs(meta);

            // Act
            var response = FakeBlogPostAccess.GetBlogPost(new BlogPost(1));
            response.Wait();
            var result = response.Result;

            // Assert
            Assert.That(result.Content.Equals("Some sweer content, yo"));
            Assert.That(result.Metadata.Title.Equals("title"));
            Assert.That(result.Metadata.Tags.SequenceEqual(new string[] { "tag1", "tag2" }));
        }

        #endregion

    }
}
