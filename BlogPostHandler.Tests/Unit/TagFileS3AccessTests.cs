using Amazon.S3.Model;
using BlogPostHandler.AccessLayers;
using BlogPostHandler.Models;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlogPostHandler.Tests.Unit
{
    [TestFixture]
    public class TagFileS3AccessTests
    {
        [OneTimeSetUp]
        public void SetEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("PostsDirectory", "test");
            Environment.SetEnvironmentVariable("MetaDirectory", "test");
            Environment.SetEnvironmentVariable("BucketRegion", "test");
        }

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

        public TagFile CreateTagFile(string contents)
        {
            return null;
        }

        #endregion

        #region GetTagFile Tests

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
            var response = fakeTagFileS3Access.GetTagFile();
            response.Wait();
            TagFile result = response.Result;

            // Assert
            // I'm generally not a fan of multiple assertions, but I am drawing a blank here
            Assert.That(result != null);
            Assert.That(result.Count == 5);
            Assert.That(result.GetIdsFromTag("fun").Count == 2);
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

        #endregion

        [Test]
        public void GetBlogPostIdsFromTags_NormalInput_ReturnsIds()
        {
            // Arrange
            var fakeAccess = Substitute.ForPartsOf<TagFileS3Access>();

            TagFile fakeTagFile = new TagFile();
            fakeTagFile.AddEntry("test1", new SortedSet<string> { "1", "2", "8" });
            fakeTagFile.AddEntry("test2", new SortedSet<string> { "1", "2", "9" });
            fakeTagFile.AddEntry("test3", new SortedSet<string> { "1", "4", "10" });

            fakeAccess.Configure().GetTagFile().ReturnsForAnyArgs(fakeTagFile);

            // Act
            var response = fakeAccess.GetBlogPostIdsFromTags(new string[] { "test1" });
            response.Wait();
            List<BlogPost> result = response.Result;

            // Assert
            var ids = result.Select(r => r.Id).ToList();
            Assert.That(String.Join(",", ids).Equals(String.Join(",", new List<int> { 1, 2, 8 })));
        }

        [Test]
        public void GetBlogPostIdsFromTags_AddMultipleEntriesForOneTag_ReturnsIds()
        {
            // Arrange
            var fakeAccess = Substitute.ForPartsOf<TagFileS3Access>();

            TagFile fakeTagFile = new TagFile();
            fakeTagFile.AddEntry("test1", new SortedSet<string> { "1", "2", "8" });
            fakeTagFile.AddEntry("test2", new SortedSet<string> { "1", "2", "9" });
            fakeTagFile.AddEntry("test1", new SortedSet<string> { "1", "4", "10" });

            fakeAccess.Configure().GetTagFile().ReturnsForAnyArgs(fakeTagFile);

            // Act
            var response = fakeAccess.GetBlogPostIdsFromTags(new string[] { "test1" });
            response.Wait();
            List<BlogPost> result = response.Result;

            // Assert
            var ids = result.Select(r => r.Id).ToList();
            ids.Sort();
            Assert.That(ids.All(new List<int> { 1, 2, 4, 8, 10 }.Contains));
        }

        [Test]
        public void GetBlogPostIdsFromTag_MultipleInstancesOfSamePost_NoDuplicates()
        {

            // Arrange
            var fakeAccess = Substitute.ForPartsOf<TagFileS3Access>();

            TagFile fakeTagFile = new TagFile();
            fakeTagFile.AddEntry("scripture", new SortedSet<string> { "1", "4" });
            fakeTagFile.AddEntry("theology", new SortedSet<string> { "1", "4" });

            fakeAccess.Configure().GetTagFile().ReturnsForAnyArgs(fakeTagFile);

            // Act
            var response = fakeAccess.GetBlogPostIdsFromTags(new string[] { "scripture", "theology" });
            response.Wait();
            List<BlogPost> result = response.Result;

            // Assert
            var ids = result.Select(r => r.Id).ToList();
            ids.Sort();
            Assert.That(ids.All(new List<int> { 1, 4}.Contains));
        }

        [Test]
        [Ignore("need to write!")]
        public void GetBlogPostIdsFromTag_TagNotInTagFile_ReturnsEmptyList()
        {

        }
    }
}
