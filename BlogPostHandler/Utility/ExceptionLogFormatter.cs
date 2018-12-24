using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogPostHandler.Utility
{
    public static class ExceptionLogFormatter
    {

        public static string FormatExceptionLogMessage(Exception ex, StringBuilder builder = null)
        {
            builder = builder ?? new StringBuilder();

            builder.Append($"\tMessage: {ex.Message}");
            builder.AppendLine();

            builder.Append($"\tStacktace: {ex.StackTrace}");
            builder.AppendLine();

            if (ex.InnerException != null)
            {
                builder.Append($"\tInner Exception: {ex.InnerException}");
                builder.AppendLine();
            }


            return builder.ToString();
        }

        public static string FormatExceptionLogMessage(GetObjectRequest request, AmazonS3Exception ex, StringBuilder builder = null)
        {
            builder = builder ?? new StringBuilder();

            builder.Append($"{ex.GetType()} - bucketName: {request.BucketName},  key: {request.Key}.");
            builder.AppendLine();

            builder.Append($"\tAmazon ID2 Token: {ex.AmazonId2}");
            builder.AppendLine();

            builder.Append($"\tStatus Code: {ex.StatusCode}");
            builder.AppendLine();

            builder.Append($"\tError Code: {ex.ErrorCode}");
            builder.AppendLine();


            return ExceptionLogFormatter.FormatExceptionLogMessage(ex, builder);
        }
    }
}
