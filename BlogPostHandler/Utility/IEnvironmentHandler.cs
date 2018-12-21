using System;
using System.Collections.Generic;
using System.Text;

namespace BlogPostHandler.Utility
{
    public interface IEnvironmentHandler
    {
        string GetVariable(string variable);

        void SetVariable(string variable, string value);
    }
}
