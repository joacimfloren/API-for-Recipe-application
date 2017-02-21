using System;
using System.Collections.Generic;
using System.Linq;
using Business.DataLayer;
using Microsoft.AspNetCore.Http;
using Mjecipies_Group_D.Models;
using Mjecipies_Group_D.Models.ViewModels;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;

namespace Mjecipies_Group_D.Business_Layer
{
    public class RecipeManager : IRecipeManager
    {
        private MjecipiesContext DbContext = new MjecipiesContext();

        public Recipe Add(RecipeViewModel.Create r)
        {
            try
            {
                Recipe recipe = new Recipe();
                recipe.Name = r.Name;
                recipe.Description = r.Description;
                recipe.Directions = r.Directions;
                recipe.CreatorId = r.CreatorId;
                recipe.Created = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                DbContext.Add(recipe);
                DbContext.SaveChanges();
                return recipe;
            }
            catch (Exception)
            {
                throw;
            }         
        }

        public Recipe Get(int id)
        {
            try
            {
                return DbContext.Recipes.Where(r => r.Id == id).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Recipe> GetPage(int page, int perPage = 10)
        {
            try
            {
                return DbContext.Recipes.Skip((page - 1) * perPage).Take(perPage).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Comment> GetComments(int id)
        {
            try
            {
                return DbContext.Comments.Where(c => c.RecipeId == id).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Recipe> Search(string term)
        {
            try
            {
                return (from r in DbContext.Recipes
                        where r.Description.Contains(term) || r.Name.Contains(term)
                        select r).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Recipe Update(RecipeViewModel.Update rvm, int id)
        {
            try
            {
                var directions = DbContext.Directions.Where(d => d.RecipeId == id).ToList();
                foreach (var d in directions)
                {
                    DbContext.Remove(d);
                }
                DbContext.SaveChanges();
                var recipe = DbContext.Recipes.Where(r => r.Id == id).First();
                recipe.Name = rvm.Name;
                recipe.Description = rvm.Description;
                recipe.Directions = rvm.Directions;
                DbContext.SaveChanges();
                return recipe;
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
                var directions = DbContext.Directions.Where(d => d.RecipeId == id).ToList();
                var favorites = DbContext.Favorites.Where(f => f.RecipeId == id).ToList();
                foreach (var f in favorites)
                {
                    DbContext.Remove(f);
                }
                foreach (var d in directions)
                {
                    DbContext.Remove(d);
                }
                var recipe = DbContext.Recipes.Where(r => r.Id == id).First();
                DbContext.Remove(recipe);
                DbContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Comment AddComment(int id, string accountId, CommentViewModel comment)
        {
            try
            {
                Comment c = new Comment();
                c.AccountId = accountId;
                c.Text = comment.Text;
                c.Grade = comment.Grade;
                c.Created = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                c.RecipeId = id;
                DbContext.Add(c);
                DbContext.SaveChanges();
                return c;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UploadImage(IFormFile image, Recipe r)
        {
            string existingBucketName = "mjecipies";
            string keyName = "recipe." + r.Id.ToString();
            IAmazonS3 client = new AmazonS3Client("AKIAIPFGUS2H426R4RXQ", "00FsSyzL0c+9aD6sepJA5ZZiULbB1vGvvqvYRgqy", Amazon.RegionEndpoint.EUCentral1);
            
                using (client)
                {
                    try
                    {
                        if (r.Image != null)
                        {
                            DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
                            {
                                BucketName = existingBucketName,
                                Key = keyName,
                            };
                            client.DeleteObjectAsync(deleteObjectRequest);
                            r.Image = null;
                        }
                        TransferUtility fileTransferUtility = new TransferUtility(client);
                        fileTransferUtility.Upload(image.OpenReadStream(), existingBucketName, keyName);
                        r.Image = "https://s3-website.eu-central-1.amazonaws.com/mjecipies/" + keyName;
                        DbContext.SaveChanges();
                    }
                    catch (AmazonS3Exception s3Exception)
                    {
                        Console.WriteLine(s3Exception.Message, s3Exception.InnerException);
                    }   
                }
            }
    }
}

