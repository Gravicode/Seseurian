using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Pdf.Content.Objects;
using Redis.OM;
using Redis.OM.Searching;
using Seseurian.Data;
using Seseurian.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seseurian.Data
{
    public class PostService : ICrud<Post>
    {
        //SeseurianDB db;
        TrendingService trendingSvc;
        UserProfileService userSvc;
        RedisConnectionProvider provider;
        IRedisCollection<Post> db; 
        public PostService(TrendingService trendingservice, UserProfileService userprofileservice,RedisConnectionProvider provider)
        {
            this.provider = provider;
            userSvc = userprofileservice;
            trendingSvc = trendingservice;
            //provider = new RedisConnectionProvider(AppConstants.RedisCon);
            db = this.provider.RedisCollection<Post>();
        }
        public bool DeleteData(Post item)
        {
            db.Delete(item);
            return true;
        }
       
        public List<Post> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.Message.Contains(Keyword)); 
            return data.ToList();
        }
       
        public List<Post> GetAllData()
        {
            return db.ToList();
        }

        public Post GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(Post data)
        {
            try
            {
                db.Insert(data);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;

        }

        public bool UpdateData(Post data)
        {
            try
            {
                db.Update(data);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public List<Post> GetTrendingPosts(int Number = 25)
        {

            var listTrend = trendingSvc.GetTrending();
            var hashtags = new HashSet<string>();
            listTrend.ForEach(x => hashtags.Add(x.Hashtag));
            var data = db.ToList().Where(x => LikeHashtag(x.Hashtags)).ToList();
            return data.Take(Number).ToList();
            bool LikeHashtag(string? posthashtag)
            {
                if (posthashtag == null) return false;
                foreach (var hashtag in posthashtag.Split(';'))
                {
                    if (hashtags.Contains(hashtag)) return true;
                }
                return false;
            }
        }
        public List<Post> GetPostMentions(string Username, int Count=100)
        {
            if (string.IsNullOrEmpty(Username)) return default;

            var data = db.Where(x => x.Mentions.Contains(Username)).OrderByDescending(x => x.CreatedDate).Take(Count).ToList();
            return data;
        }

        public List<Post> GetLikedPosts(string Username, int Count = 100)
        {
            if (string.IsNullOrEmpty(Username)) return default;

            var data = db.Where(x => x.PostLikes.Any(z => z.LikedByUser.Username == Username)).OrderByDescending(x => x.CreatedDate).Take(Count).ToList();
            return data;

        }
        public List<Post> GetMyTimeline(string Username, int Count = 100)
        {
            if (string.IsNullOrEmpty(Username)) return default;

            var data = db.Where(x => x.UserName == Username).OrderByDescending(x => x.CreatedDate).Take(Count).ToList();
            return data;

        }
        public List<Post> GetTimeline(string Username,int Count=100)
        {
            if (string.IsNullOrEmpty(Username))
            {
                var data = db.OrderByDescending(x => x.CreatedDate);
                return data.Take(Count).ToList();
            }
            else
            {
                var data = db.Where(x => x.UserName == Username).OrderByDescending(x => x.CreatedDate).Take(Count).ToList();
                var followedUser = userSvc.GetFollows(  Username).Select(x => x.Username);
                var data2 = db.Where(x => followedUser.Contains(x.UserName)).OrderByDescending(x => x.CreatedDate);
                var union = data.Union(data2).OrderByDescending(x => x.CreatedDate);
                return union.Take(Count).ToList();
            }
        }

        public bool UnLikePost(string username, string postid)
        {
            try
            {
                var removePost = db.Where(x=>x.Id == postid).FirstOrDefault();
                if (removePost != null)
                {
                    var selItem = removePost.PostLikes.Where(x=>x.LikedByUser.Username == username).FirstOrDefault();
                    if (selItem != null)
                    {
                        removePost.PostLikes.Remove(selItem);
                        db.Update(removePost);
                        return true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }


        public bool LikePost(string username, string postid)
        {
            try
            {
                var selUser = userSvc.GetUserByEmail(username);
                var selPost = GetDataById(postid);
                var newLike = new PostLike() { CreatedDate = DateHelper.GetLocalTimeNow(), LikedByUser = selUser};
                if (!selPost.PostLikes.Any(x => x.LikedByUser.Username == username))
                {
                    selPost.PostLikes.Add(newLike);
                    return true;
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }
    }

}
