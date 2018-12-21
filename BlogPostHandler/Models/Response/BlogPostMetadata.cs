
using System;
using System.Linq;

namespace BlogPostHandler.Models.Response
{
    public class Metadata
    {
        public int Id;
        public string Title { get; set; }
        public string[] Tags { get; set; }

        public Metadata(int id, string title, string[] tags)
        {
            Id = id;
            Title = title;
            Tags = tags;
        }

        public Metadata(int id)
        {
            Id = id;
        }

        public static Metadata ToMetadata(Metadata metadata, string input)
        {
            metadata = metadata ?? new Metadata(-1);

            string[] parsedInput = input.Split('\n');
            metadata.Title = parsedInput[0];
            metadata.Tags = parsedInput[1].Split(',');

            return metadata;
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Metadata b = (Metadata)obj;
                return (Id == b.Id) && (Title == b.Title) && (Enumerable.SequenceEqual(Tags, b.Tags));
            }
        }
    }
}
