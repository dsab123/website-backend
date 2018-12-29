using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using BlogPostHandler.AccessLayers;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BlogPostHandler.Tests.Unit
{
    [TestFixture]
    public class S3AccessTests
    {
        public static GetObjectRequest GetGetObjectRequest(string bucketName = null, string key = null)
        {
            return new GetObjectRequest
            {
                BucketName = bucketName ?? "",
                Key = key ?? ""
            };
        }

        [Test]
        public void GetObject_NullInput_ThrowsException()
        {
            // Arrange
            var fakeS3Client = new FakeMyAmazonS3ClientThatReturnsNull(Utility.GetS3Config("test"));

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
            Assert.ThrowsAsync<Exception>(() => access.GetObject(S3AccessTests.GetGetObjectRequest()));
        }

        [Test]
        public void GetObject_NormalInput_ReturnsContent()
        {
            // Arrange
            var fakeS3Access = new FakeS3Access();
            var fakeS3Client = Substitute.For<IMyAmazonS3Client>();
            
            fakeS3Access.S3Client = fakeS3Client;
            var getObjectRequest = S3AccessTests.GetGetObjectRequest("test", "test");

            // we write some fake content to the stream
            string expectedContent = "This is some fake content";
            var fakeResponse = new GetObjectResponse();
            fakeResponse.Key = "fakeKey";
            fakeResponse.BucketName = "fakeBucket";
            fakeResponse.ContentLength = expectedContent.Length;
            MemoryStream stream = new MemoryStream();
            
            StreamWriter writer = new StreamWriter(stream);
            {
                try
                {
                    writer.Write(expectedContent);
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Position = 0;
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception ex)
                {
                    Assert.Fail("Writing to Stream to set test up failed!");
                }
            }

            fakeResponse.ResponseStream = stream;

            fakeS3Client.GetObjectAsync(getObjectRequest, default(System.Threading.CancellationToken)).ReturnsForAnyArgs(fakeResponse);
            
            // Act
            var response = fakeS3Access.GetObject(getObjectRequest);
            response.Wait();
            var actualContent = response.Result;

            // we have to do this here because if we dispose it in a using() on line 79, we can't read from
            // the stream! or at least I _think_ that's why
            writer.Dispose();

            // Assert
            Assert.That(expectedContent.Equals(actualContent));


        }
        

    }
}
