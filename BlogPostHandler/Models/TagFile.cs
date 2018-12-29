using System;
using System.Collections.Generic;

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

        protected Dictionary<string, SortedSet<string>> dictionary;

        public TagFile()
        {
            dictionary = new Dictionary<string, SortedSet<string>>();
        }

        //TODO if necessary - move the following functions to a TagFileLogic layer,
        // they don't really fit in the Model if I'm being super-strict

        // don't want to use a comparer just yet
        public class Comparer : IEqualityComparer<string>
        {
            bool IEqualityComparer<string>.Equals(string x, string y)
            {
                return Int32.Parse(x) < Int32.Parse(y);
            }

            int IEqualityComparer<string>.GetHashCode(string obj)
            {
                return Int32.Parse(obj);
            }
        }

        // need a way to WRITE these contents to file, duh
        public void AddEntry(string tag, SortedSet<string> ids)
        {
            if (!dictionary.ContainsKey(tag))
            {
                dictionary.Add(tag, ids);
            }
            else
            {
                dictionary[tag].UnionWith(ids);
            }
        }

        public SortedSet<string> GetIdsFromTag(string tag)
        {
            if (!dictionary.ContainsKey(tag))
            {
                return null;
            }

            return dictionary.GetValueOrDefault(tag);
        }

        public List<string> GetIdsFromTags(string[] tags)
        {
            List<string> ids = null; 
            foreach (var tag in tags)
            {
                if (dictionary.ContainsKey(tag))
                {
                    if (ids == null)
                    {
                        ids = new List<string>();
                    }

                    ids.AddRange(dictionary.GetValueOrDefault(tag));
                }
            }

            return ids;
        }

    }
}
