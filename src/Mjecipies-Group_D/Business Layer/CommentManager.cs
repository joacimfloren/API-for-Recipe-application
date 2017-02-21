using System;
using System.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Business.DataLayer;
using Microsoft.AspNetCore.Http;
using Mjecipies_Group_D.Models;
using Mjecipies_Group_D.Models.ViewModels;

namespace Mjecipies_Group_D.Business_Layer
{
    public class CommentManager : ICommentManager
    {
        private MjecipiesContext DbContext = new MjecipiesContext();

        public Comment Get(int id)
        {
            try
            {
                return DbContext.Comments.Where(c => c.Id == id).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(int id)
        {
            try
            {
                var comment = DbContext.Comments.Where(c => c.Id == id).First();
                DbContext.Remove(comment);
                DbContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(CommentViewModel c,int id)
        {
            try
            {
                var comment = Get(id);
                comment.Text = c.Text;
                comment.Grade = c.Grade;
                DbContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UploadImage(IFormFile image, Comment c)
        {
            string existingBucketName = "mjecipiesbucket";
            string keyName = "comment." + c.Id.ToString();
            IAmazonS3 client = new AmazonS3Client("AKIAJ7ZFUZ4GDMJBY7JQ", "PT3soHEs6mpUMxlpq2uTuADeUFc3fVrI9kJf8FAZ", Amazon.RegionEndpoint.EUCentral1);

            using (client)
            {
                try
                {
                    if (c.Image != null)
                    {
                        DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
                        {
                            BucketName = existingBucketName,
                            Key = keyName,
                        };
                        client.DeleteObjectAsync(deleteObjectRequest);
                        c.Image = null;
                    }
                    TransferUtility fileTransferUtility = new TransferUtility(client);
                    fileTransferUtility.Upload(image.OpenReadStream(), existingBucketName, keyName);              
                    c.Image = "https://s3.eu-central-1.amazonaws.com/mjecipiesbucket/" + keyName;
                    DbContext.SaveChanges();
                }
                catch (AmazonS3Exception s3Exception)
                {
                    Console.WriteLine(s3Exception.Message,
                                      s3Exception.InnerException);
                }
            }
        }

        public bool HasUserAlreadyCommented(string commenterId, int recipeId) {
            try
            {
                return DbContext.Comments.Where(c => c.RecipeId == recipeId && c.AccountId == commenterId).Any();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
