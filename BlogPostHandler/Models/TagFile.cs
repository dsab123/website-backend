using BlogPostHandler.AccessLayers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogPostHandler.Models
{
    /// <summary>
    /// A TagFile is a reference file that maps tags on the website to a list of posts tagged wih that tag.
    /// It's just a wrapper of a Dictionary<string, List<string>>.
    /// </summary>
    public class TagFile
    {
        public int Count {
            get { 
                return dictionary.Count;
            }
        }

        protected Dictionary<string, List<string>> dictionary;
        
        public TagFile()
        {
            dictionary = new Dictionary<string, List<string>>();
        }

        public void AddEntry(string tag, List<string> ids)
        {
            dictionary.Add(tag, ids);
        }

        public List<string> GetValue(string tag)
        {
            if (!dictionary.ContainsKey(tag))
            {
                return null;
            }

            return dictionary.GetValueOrDefault(tag);
        }
        
    }
}
