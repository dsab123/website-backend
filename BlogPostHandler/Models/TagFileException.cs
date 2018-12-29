using System;
using System.Collections.Generic;
using System.Text;

namespace BlogPostHandler.Models
{
    public class TagFileException : Exception
    {
        public TagFileException(string message) : base(message)
        {
        }
    }
}
