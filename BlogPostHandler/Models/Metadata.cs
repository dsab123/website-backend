﻿
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace BlogPostHandler.Models
{
    public class Metadata
    {
        [JsonProperty(PropertyName = "id")]
        public int Id;

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "tags")]
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
            if (input == null)
            {
                return null;
            }

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
