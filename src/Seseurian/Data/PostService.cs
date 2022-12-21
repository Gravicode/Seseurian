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
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using static System.Formats.Asn1.AsnWriter;
using Raven.Client.Documents.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Seseurian.Data
{
    public class PostService : ICrud<Post>
    {
        //SeseurianDB db;
        TrendingService trendingSvc;
        UserProfileService userSvc;
        RedisConnectionProvider provider;
        //IRedisCollection<Post> db; 
        IDocumentSession db;
        public PostService(TrendingService trendingservice, UserProfileService userprofileservice, IDocumentStore store)
        {
            //this.provider = provider;
            userSvc = userprofileservice;
            trendingSvc = trendingservice;
            //provider = new RedisConnectionProvider(AppConstants.RedisCon);
            //db = this.provider.RedisCollection<Post>();
            db = store.OpenSession();

        }

        public void RefreshEntity(Post post)
        {
            try
            {
                db.Advanced.Refresh(post);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }


        }

        public bool AddComment(string Comment, UserProfile user, string PostId)
        {
            try
            {

                var post = db.Query<Post>().Where(x => x.Id == PostId).FirstOrDefault();
                var newComment = new PostComment() { CommentByUser = user, Comment = Comment, CreatedDate = DateHelper.GetLocalTimeNow() };
                post.PostComments.Add(newComment);
                db.Store(post);
                //user.PostComments.Add(newComment);
                //db.Store(user);
                var newNotif = new Notification() { LinkUrl = "#", LinkDesc = "None", CreatedDate = DateHelper.GetLocalTimeNow(), Action = "Comment on Post", IsRead = false, Message = $"{user.FullName} comment on your post {(post.Message.Length > 10 ? post.Message.Substring(0, 10) : post.Message)}", User = post.PostByUser, UserName = post.UserName };
                db.Store(newNotif);


                db.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

        }
        public bool UnLikePost(UserProfile user, string postid)
        {
            try
            {
                var post = db.Query<Post>().Where(x => x.Id == postid).FirstOrDefault();
                var likepost = post.PostLikes.Where(x => x.LikedByUser.Username == user.Username).FirstOrDefault();
                post.PostLikes.Remove(likepost);
                db.Store(post);

                var newNotif = new Notification() { LinkUrl = "#", LinkDesc = "None", CreatedDate = DateHelper.GetLocalTimeNow(), Action = "Unlike Post", IsRead = false, Message = $"{user.FullName} unlike your post {(post.Message.Length > 10 ? post.Message.Substring(0, 10) : post.Message)}", User = post.PostByUser, UserName = post.UserName };
                db.Store(newNotif);

                //user.PostLikes.Remove(likepost);
                //db.Store(user);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }


        public bool LikePost(UserProfile user, string postid)
        {
            try
            {
                var post = db.Query<Post>().Where(x => x.Id == postid).FirstOrDefault();
                var likepost = new PostLike() { CreatedDate = DateHelper.GetLocalTimeNow(), LikedByUser = user };
                post.PostLikes.Add(likepost);
                db.Store(post);

                var newNotif = new Notification() { LinkUrl = "#", LinkDesc = "None", CreatedDate = DateHelper.GetLocalTimeNow(), Action = "Like Post", IsRead = false, Message = $"{user.FullName} like your post { (post.Message.Length > 10 ? post.Message.Substring(0,10): post.Message)}", User = post.PostByUser, UserName = post.UserName };
                db.Store(newNotif);

                //user.PostLikes.Add(likepost);
                //db.Store(user);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }
        public bool DeleteData(Post item)
        {
            db.Delete(item);

            db.SaveChanges();
            return true;
        }

        public bool DeleteData(Post item, UserProfile user)
        {
            db.Delete(item);
            user.Posts.Remove(item.Id);
            db.Store(user);
            db.SaveChanges();
            return true;
        }

        public List<Post> FindByKeyword(string Keyword, int Limit = 100)
        {
            List<Post> data;
            if (string.IsNullOrEmpty(Keyword))
            {
                data = db.Query<Post>().Take(Limit).ToList();
            }
            else
            {
                data = db.Query<Post>().Search(x => x.Message, Keyword).Take(Limit).ToList();
            }
            return data;
        }
        public List<Post> FindByKeyword(string Keyword)
        {
            int Limit = 1000;
            List<Post> data;
            if (string.IsNullOrEmpty(Keyword))
            {
                data = db.Query<Post>().Take(Limit).ToList();
            }
            else
            {
                data = db.Query<Post>().Search(x => x.Message, Keyword).Take(Limit).ToList();
            }
            return data;
        }
        public List<Post> GetHighlights(string Username, int TakeCount = 10)
        {
            return db.Query<Post>().Where(x=>x.UserName == Username).OrderByDescending(x=>x.PostLikes.Count()).ThenByDescending(x => x.CreatedDate).Take(TakeCount).ToList();
        }
        public List<Post> GetLatestPost(int TakeCount = 101)
        {
            return db.Query<Post>().OrderByDescending(x => x.CreatedDate).Take(TakeCount).ToList();
        }
        
        public List<Post> GetLatestPost(string Username, int TakeCount = 100)
        {
            return db.Query<Post>().Where(x=>x.UserName==Username).OrderByDescending(x => x.CreatedDate).Take(TakeCount).ToList();
        }
        public List<Post> GetAllData()
        {
            return db.Query<Post>().ToList();
        }

        public Post GetDataById(string Id)
        {
            return db.Query<Post>().Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(Post data)
        {
            try
            {
                db.Store(data);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;

        }

        public bool InsertData(Post data, UserProfile user)
        {
            try
            {
                db.Store(data);
                user.Posts.Add(data.Id);
                db.Store(user);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;

        }

        public bool UpdateData(Post data)
        {
            try
            {
                db.Store(data);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
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
            var data = db.Query<Post>().ToList().Where(x => LikeHashtag(x.Hashtags)).ToList();
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
        public List<Post> GetPostMentions(string Username, int Count = 100)
        {
            if (string.IsNullOrEmpty(Username)) return default;

            var data = db.Query<Post>().Where(x => x.Mentions.Contains(Username)).OrderByDescending(x => x.CreatedDate).Take(Count).ToList();
            return data;
        }

        public List<Post> GetLikedPosts(string Username, int Count = 100)
        {
            if (string.IsNullOrEmpty(Username)) return default;

            var data = db.Query<Post>().Where(x => x.PostLikes.Any(z => z.LikedByUser.Username == Username)).OrderByDescending(x => x.CreatedDate).Take(Count).ToList();
            return data;

        }
        public List<Post> GetMyTimeline(string Username, int Count = 100)
        {
            if (string.IsNullOrEmpty(Username)) return default;

            var data = db.Query<Post>().Where(x => x.UserName == Username).OrderByDescending(x => x.CreatedDate).Take(Count).ToList();
            return data;

        }
        public List<Post> GetTimeline(string Username, int Count = 100)
        {
            db.Advanced.Clear();
            if (string.IsNullOrEmpty(Username))
            {
                var data = db.Query<Post>().OrderByDescending(x => x.CreatedDate).Take(Count).ToList();
                return data;
            }
            else
            {
                var data = db.Query<Post>().Where(x => x.UserName == Username).OrderByDescending(x => x.CreatedDate).Take(Count).ToList();
                var followedUser = userSvc.GetFollows(Username).Select(x => x.Username).ToList();
                var data2 = db.Query<Post>().Where(x => x.UserName.In(followedUser)).OrderByDescending(x => x.CreatedDate).ToList();
                var union = data.Union(data2).OrderByDescending(x => x.CreatedDate);
                return union.Take(Count).ToList();
            }
        }


    }

}
