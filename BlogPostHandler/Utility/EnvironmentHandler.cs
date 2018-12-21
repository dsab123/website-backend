using System;
using BlogPostHandler.Utility;

namespace BlogPostHandler.Models.Utility
{
    public class EnvironmentHandler : IEnvironmentHandler
    {
        public string GetVariable(string variable)
        {
            string result = null;// string.Empty;
            try
            {
                result = Environment.GetEnvironmentVariable(variable);
            }
            catch (Exception e)
            {
            }

            return result;
        }

        public void SetVariable(string variable, string value)
        {
            Environment.SetEnvironmentVariable(variable, value);
        }
    }
}
