using System.Text.RegularExpressions;

namespace BlogPostHandler.Utility
{
    public static class StringHelper
    {
        public static string StripMarkdownIdentifiers(string content)
        {
            content = content.Replace("*", "");
            return content;
        }
    }
}
