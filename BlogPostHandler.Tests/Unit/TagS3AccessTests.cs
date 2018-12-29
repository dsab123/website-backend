using Amazon.S3.Model;
using BlogPostHandler.AccessLayers;
using BlogPostHandler.Models;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlogPostHandler.Tests.Unit
{
    [TestFixture]
    public class TagS3AccessTests
    {
        [OneTimeTearDown]
        public void AfterAll()
        {
            foreach (var writer in streamWriters)
            {
                writer.Dispose();
            }
        }


        #region Helpers

        public static List<StreamWriter> streamWriters = new List<StreamWriter>();

        public static GetObjectResponse WriteContentToStream(string expectedTagFile)
        {
            var fakeResponse = new GetObjectResponse();
            fakeResponse.Key = "fakeKey";
            fakeResponse.BucketName = "fakeBucket";
            fakeResponse.ContentLength = expectedTagFile.Length;
            MemoryStream stream = new MemoryStream();

            StreamWriter writer = new StreamWriter(stream);
            {
                try
                {
                    writer.Write(expectedTagFile);
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

            // we can't dispose this writer, so we'll add it to an array and dispose them all at the end
            streamWriters.Add(writer);

            return fakeResponse;
        }

        #endregion

        
        [Test]
        public void GetTagFile_NormalInput_ReturnsTagFile()
        {
            // Arrange
            TagFileS3Access fakeTagFileS3Access = new TagFileS3Access();

            var fakeS3Client = Substitute.For<IMyAmazonS3Client>();

            fakeTagFileS3Access.S3Client = fakeS3Client;
            var getObjectRequest = S3AccessTests.GetGetObjectRequest("test", "test");

            // we write some fake content to the stream
            string expectedTagFile = "scripture-1\ntheology-1\nfun-1,2\nbunny-2\npaul-2";

            GetObjectResponse fakeResponse = WriteContentToStream(expectedTagFile);
            

            fakeS3Client.GetObjectAsync(getObjectRequest, default(System.Threading.CancellationToken)).ReturnsForAnyArgs(fakeResponse);

            // Act
            var response =  fakeTagFileS3Access.GetTagFile();
            response.Wait();
            TagFile result = response.Result;

            // Assert
            // I'm generally not a fan of multiple assertions, but I don't know how to test an object as complex as a TagFile
            Assert.That(result != null);
            Assert.That(result.Count == 5);
            Assert.That(result.GetValue("fun").Count == 2);
        }

        public static object[] malformedTagFiles = {
            "",
            "woohoo, this is ---malformed -dedede"
        };

        [Test]
        [TestCaseSource("malformedTagFiles")]
        public void GetTagFile_MalformedTagFiles_ThrowsException(string malformedTagFile)
        {
            // Arrange
            TagFileS3Access access = new TagFileS3Access();

            var fakeS3Client = Substitute.For<IMyAmazonS3Client>();

            access.S3Client = fakeS3Client;
            var getObjectRequest = S3AccessTests.GetGetObjectRequest("test", "test");

            GetObjectResponse fakeResponse = WriteContentToStream(malformedTagFile);

            fakeS3Client.GetObjectAsync(getObjectRequest, default(System.Threading.CancellationToken)).ReturnsForAnyArgs(fakeResponse);

            // Act/Assert
            Assert.ThrowsAsync<TagFileException>(() => access.GetTagFile());
        }
    }
}
