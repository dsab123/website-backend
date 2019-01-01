using Amazon.Lambda.Core;
using Amazon.S3.Model;
using BlogPostHandler.Models;
using BlogPostHandler.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogPostHandler.AccessLayers
{
    public class TagFileS3Access : S3Access, ITagFileS3Access, IDisposable
    {
        private static string TagFileName = "tagfile";
        

        public TagFileS3Access()
        {
        }

        // what is the wisdom of my base object retrieval class getting things as strings?
        public virtual async Task<TagFile> GetTagFile()
        {
            // use GetObject to retrieve the TagFile
            string tagFileContents = await GetObject(new GetObjectRequest
            {
                BucketName = EnvironmentHandler.GetEnvironmentHandler().GetVariable("BucketName"),
                Key = TagFileS3Access.TagFileName
            });

            // parse string into TagFile
            // each line is one entry
            string[] lines = tagFileContents.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            if (tagFileContents == string.Empty)
            {
                throw new TagFileException("Exception - Empty TagFile.");
            }

            TagFile tagFile = new TagFile();

            for(int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                // parse each line - split on '-'; LHS is tag, RHS is array of ids
                string[] lineContents = line.Split('-');

                if (lineContents.Length != 2)
                {
                    throw new TagFileException($"there is an issue with tagfile format on line {i}.");
                }

                List<string> ids = lineContents[1].Split(',').ToList();

                tagFile.AddEntry(lineContents[0], new SortedSet<string>(ids));
            }

            return tagFile;
        }

        public async Task<List<BlogPost>> GetBlogPostIdsFromTag(string tag)
        {
            TagFile tagFile = await GetTagFile();
            List<BlogPost> posts = new List<BlogPost>();
            foreach(var id in tagFile.GetIdsFromTag(tag))
            {
                posts.Add(new BlogPost(Int32.Parse(id)));
            }

            return posts;
        }

        public async Task<List<BlogPost>> GetBlogPostIdsFromTags(string[] tags)
        {
            TagFile tagFile = await GetTagFile();
            List<BlogPost> posts = new List<BlogPost>();
            foreach (var id in tagFile.GetIdsFromTags(tags))
            {
                BlogPost newPost = new BlogPost(Int32.Parse(id));
                newPost.Blurb = true;

                posts.Add(newPost);
            }

            return posts;
        }


        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool freeManagedResources)
        {
            if (freeManagedResources)
            {

                if (this.S3Client != null)
                {
                    this.S3Client.Dispose();
                    this.S3Client = null;
                }
            }
        }
    }
}
