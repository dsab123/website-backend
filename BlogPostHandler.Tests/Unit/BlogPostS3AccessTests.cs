using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using BlogPostHandler.AccessLayers;
using BlogPostHandler.Models;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace BlogPostHandler.Tests.Unit
{
    #region Fakes

    // using extract and override
    public class FakeBlogPostS3Access : BlogPostS3Access
    {
        public string Expected { get; set; }

        public async override Task<string> GetObject(GetObjectRequest request)
        {
            return await Task.FromResult(Expected);
        }
    }

    public class FakeMyAmazonS3Client : MyAmazonS3Client
    {
        public FakeMyAmazonS3Client(AmazonS3Config config) : base(config)
        {
        }

        public override Task<GetObjectResponse> GetObjectAsync(GetObjectRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<GetObjectResponse>(null);
        }
    }

    #endregion

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


        #endregion

        #region GetMetadata Tests

        public static object[] GetMetadata_Inputs = {
           "title\ntag1,tag2,tag3",
           "A Very Long Title that is long\nTag A, Tag B, Tag 100"
        };

        [Test]
        [TestCaseSource("GetMetadata_Inputs")]
        public void GetMetadata_SimpleString_ReturnsTrue(string expectedResult)
        {
            // Arrange
            FakeBlogPostS3Access access = new FakeBlogPostS3Access
            {
                Expected = expectedResult
            };

            // Act
            var response = access.GetBlogPostMetadata(new Metadata(1), "test", "test");
            response.Wait();
            Metadata result = response.Result;

            // Assert
            Assert.That(result.Equals(Metadata.ToMetadata(result, expectedResult)));
        }

        [Test]
        public void GetMetadata_NullMetaDirectory_ThrowsException()
        {
            // Arrange
            BlogPostS3Access access = new BlogPostS3Access();

            // Act/Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => access.GetBlogPostMetadata(new Metadata(1), "test", null));
        }

        [Test]
        public void GetMetadata_NullKeyName_ThrowsException()
        {
            // Arrange
            BlogPostS3Access access = new BlogPostS3Access();

            // Act/Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => access.GetBlogPostMetadata(new Metadata(1), null, "test"));
        }

        #endregion

        #region GetObject Tests

        [Test]
        //[Ignore("need to stub out AmazonS3Client and AmazonS3Config to test this rightly")]
        public void GetObject_NullInput_ReturnsString()
        {
            // Arrange
            var fakeS3Client = new FakeMyAmazonS3Client(Utility.GetS3Config("test"));

            BlogPostS3Access access = new BlogPostS3Access("test", "test")
            {
                S3Client = fakeS3Client
            };

            // if I decide to return a string instead of throw an exception...
            // Act
            //var response = access.GetObject(new GetObjectRequest
            //{
            //    BucketName = Arg.Any<string>(),
            //    Key = Arg.Any<string>()
            //});
            //response.Wait();
            //var expectedResult = response.Result;
            //
            //// Assert
            //Assert.That(BlogPostException.ErrorBlogPostContents.Equals(expectedResult));

            // Act/Assert
            Assert.ThrowsAsync<Exception>(() => access.GetObject(new GetObjectRequest
            {
                BucketName = Arg.Any<string>(),
                Key = Arg.Any<string>()
            }));
        }

        #endregion

    }
}
