using BlogPostHandler.Utility;
using NUnit.Framework;

namespace BlogPostHandler.Tests.Unit
{
    [TestFixture]
    public class StringHelperTests
    {
        [Test]
        public void StripMarkdownTags_SimpleString_ReturnsStrippedString()
        {
            // Arrange
            string sampleString = "*Prayer* is communion with God.";

            // Act
            var result = StringHelper.StripMarkdownIdentifiers(sampleString);

            // Assert
            Assert.That(result.Equals("Prayer is communion with God."));

        }
    }
}
