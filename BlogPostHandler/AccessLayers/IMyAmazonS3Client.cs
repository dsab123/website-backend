using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlogPostHandler.AccessLayers
{
    public interface IMyAmazonS3Client
    {
        Task<GetObjectResponse> GetObjectAsync(GetObjectRequest request, CancellationToken cancellationToken = default(CancellationToken));

        void Dispose();
    }
}
