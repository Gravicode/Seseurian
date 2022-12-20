using GemBox.Document;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Redis.OM.Modeling;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Reflection;

namespace Seseurian.Models
{
    #region helpers model 
    public class Inbox
    {
        public UserProfile User { get; set; }
        public MessageBox Message { get; set; }

    }
    public record TrendingTag(string Tag,long Count);
    public class TempFollow
    {

        public bool IsFollow
        {
            get; set;
        }
        public UserProfile User { get; set; }
    }
    public class StorageObject
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string FileUrl { get; set; }
        public string ContentType { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime? LastAccess { get; set; }
    }
    public class StorageSetting
    {
        public string EndpointUrl { get; set; } = "https://is3.cloudhost.id";
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; } = "USWest1";
        public string Bucket { get; set; }
        public string BaseUrl { get; set; }
        public bool Ssl { get; set; } = true;
        public StorageSetting()
        {

        }
        public StorageSetting(string Endpoint, string Accesskey, string Secretkey, string Region, string Bucket)
        {
            this.EndpointUrl = Endpoint;
            this.AccessKey = Accesskey;
            this.SecretKey = Secretkey;
            this.Region = Region;
            this.Bucket = Bucket;
            GenerateBaseUrl();
        }
        public void GenerateBaseUrl()
        {
            this.BaseUrl = EndpointUrl + "/{bucket}/{key}";
        }
    }
    public class PeopleByJob
    {
        public string Job { get; set; }
        public List<UserProfile> Users { get; set; }
    }
    public record PopularPeople(string Username, int TotalFollower, UserProfile User);
    public class OutputCls
    {
        public OutputCls()
        {
            this.IsSucceed = false;
            this.Message = "";
        }
        public object Data { get; set; }
        public string Message { get; set; }
        public bool IsSucceed { get; set; }
    }
    #endregion
    public class ProductReview
    {
        [Indexed]
        public string Username { set; get; }
        [Indexed]
        public string UserId { set; get; }
        [Indexed(Sortable = true)]
        public DateTime CreatedDate { set; get; }
        [Indexed]
        public string Message { set; get; }
        [Indexed(Sortable = true)]
        public int Rating { get; set; } = 0;
      

    }

    [Document(Prefixes = new[] { "Product" })]
   
    public class Product
    {

        [RedisIdField] public string Id { get; set; }

        [Indexed(Sortable = true)]
        public string UserId { get; set; }

        [Indexed(Sortable = true)]
        public string Name { get; set; }

        [Indexed(Sortable = true)]
        public string Desc { get; set; }
        [Indexed(Sortable = true)]
        public string Category { get; set; }
        [Indexed(Sortable = true)]
        public double Price { get; set; }
        [Searchable(Sortable = true)]
        public string Tipe { set; get; }
        [Indexed(Sortable = true)]
        public string Currency { set; get; }
        [Indexed(Sortable = true)]
        public DateTime CreatedDate { set; get; }
        [Indexed(Sortable = true)]
        public string? PicUrl { set; get; }
        [Indexed]
        public DateTime LastUpdate { set; get; }
        [Indexed]
        public bool ApproveTerm { set; get; } = false;
        [Indexed]
        public int Rating { set; get; } = 0;
        [Indexed]
        public string Username { set; get; } 
       
        public List<ProductReview> Reviews { get; set; } = new();
    }


    [Document(Prefixes = new[] { "MessageBox" })]
  
    public class MessageBox
    {
       
        [RedisIdField] public string Id{ get; set; }

        [Indexed(Sortable = true)]
        public string UserId { get; set; }

        [Indexed(Sortable = true)]
        public string ToUserId { get; set; }

        [Indexed(Sortable = true)]
        public string Uid { get; set; }
        [Indexed(Sortable = true)]
        public string Username { get; set; }  
        [Indexed(Sortable = true)]
        public string ToUsername { get; set; }
        [Searchable(Sortable =true)]
        public string? Title { set; get; }
        [Indexed]
        public string? Desc { set; get; }
        [Indexed(Sortable =true)]
        public DateTime CreatedDate { set; get; }
        public string? LastMessage { set; get; }
        [Indexed]
        public DateTime LastUpdate { set; get; }
        [Indexed]
        public bool IsArchived { set; get; } = false;
        [Indexed]
        public bool IsRead { set; get; } = false;
        [Indexed]
        //public string? WallpaperUrl { set; get; }
        public bool IsMuted { set; get; } = false;
        [Indexed]
        public bool IsBlocked { set; get; } = false;
        public List<MessageDetail> Chats { get; set; } = new();
    }
    public class MessageDetail
    {
        public UserProfile FromUser { set; get; }
        public DateTime CreatedDate { set; get; }
        [Indexed]
        public string Message { set; get; }
        public bool HasAttachment { get; set; } = false;
        public List<MessageAttachment> Attachments { get; set; } = new();

    }
    public class MessageAttachment
    {
        [Indexed]
        public string Title { set; get; }
        [Indexed]
        public string? Url { set; get; }
        [Indexed]
        public string? Desc { set; get; }
        public string? Longitude { set; get; }
        public string? Latitude { set; get; }
       
        public AttachmentTypes AttachmentType { set; get; }
        public DateTime CreatedDate { set; get; }
    }
    public enum AttachmentTypes { Doc, Video, Audio, Link, Location };

    [Document(Prefixes = new[] { "Trending" })]
   
    public class Trending
    {      
        [RedisIdField] public string Id{ get; set; }
        [Indexed(Sortable = true)]
        public string Hashtag { set; get; }
        public DateTime CreatedDate { set; get; }
        [Searchable]
        public string? Location { set; get; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
    }
    public class Follow
    {             
        public string FollowUser { set; get; }
        public string FollowUsername { get; set; }
        public DateTime FollowDate { set; get; }
    }

    public class PostLike
    {
       
        public UserProfile LikedByUser { set; get; }
        public DateTime CreatedDate { set; get; }
    }
    public class PostComment
    {
       
        [Indexed]
        public string Comment { set; get; }
        public DateTime CreatedDate { set; get; }
        public UserProfile CommentByUser { set; get; }
        public List<CommentLike> CommentLikes { get; set; } = new();

    }
    public class CommentLike
    {
        public UserProfile LikedByUser { set; get; }
        public DateTime CreatedDate { set; get; }
    }

   

    [Document(Prefixes = new[] { "Post" })]
    public class Post
    {
        [RedisIdField] public string Id{ get; set; }
        [Indexed(Sortable = true)]
        public string UserName { set; get; }
        [Indexed]
        public DateTime CreatedDate { set; get; }
        [Searchable]
        public string Message { set; get; }
        [Searchable]
        public string? Mentions { set; get; }
        public string? LinkUrls { set; get; }
        public string? VideoUrls { set; get; }
        public string? DocUrls { set; get; }
        public string? ImageUrls { set; get; }
        [Searchable]
        public string? Hashtags { set; get; }
        [Indexed]
        public bool IsRepost { get; set; } = false;
        [Indexed]

        public bool DisableComment { get; set; }

        public UserProfile PostByUser { get; set; }
        //public string PostByUser { get; set; }

        public List<PostLike> PostLikes { get; set; } = new();
        public List<PostComment> PostComments { get; set; } = new();

    }

    [Document(Prefixes = new[] { "Notification" })]
   
    public class Notification
    {
       
        [RedisIdField] public string Id{ get; set; }
        [Indexed(Sortable = true)]
        public DateTime CreatedDate { set; get; }
        [Indexed]
        public string UserName { set; get; }
        public UserProfile User { set; get; }
        [Searchable]
        public string Action { set; get; }
        public string LinkUrl { set; get; }
        public string LinkDesc { set; get; }
        [Searchable]
        public string Message { set; get; }
        [Indexed]
        public bool IsRead { set; get; } = false;
    }
    public enum LogCategory
    {
        Info, Error, Warning
    }
    [Document(Prefixes = new[] { "Log" })]
  
    public class Log
    {
    
        [RedisIdField] public string Id{ get; set; }
        public string CreatedBy { get; set; }
        [Indexed]
        public DateTime LogDate { get; set; }
        [Indexed]
        public string Message { get; set; }
        [Indexed]
        public LogCategory Category { get; set; }
    }
    [Document(Prefixes = new[] { "UserProfile" })]
   
    public class UserProfile:ICloneable
    {
        public object Clone()
        {
            return MemberwiseClone();
        }

        [RedisIdField] public string Id{ get; set; }
        [Indexed(Sortable =true)]
        public string Username { get; set; }
        public string Password { get; set; }
        [Searchable]
        public string FullName { get; set; }
        [Indexed]
        public string? Phone { get; set; }
        [Indexed]
        public string? Pekerjaan { get; set; } = "Pengangguran";
        [Indexed]
        public string? Relationship { get; set; }
        [Indexed]
        public string? Email { get; set; }
        [Indexed]
        public string? Alamat { get; set; }
        [Indexed]
        public string? KTP { get; set; }
        public string? PicUrl { get; set; }
        public bool Aktif { get; set; } = true;
        public string? Daerah { get; set; }
        public string? Desa { get; set; }
        public string? Kelompok { get; set; }
        public Genders Gender { get; set; } 
        public Roles Role { set; get; } = Roles.User;
        public DateTime CreatedDate { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        public string? FirstName { set; get; }
        public string? LastName { set; get; }
      
        public string? AboutMe { set; get; }
        public string? Tags { set; get; }

        public DateTime? Birthday { get; set; }

        public List<Follow> Follows { get; set; } = new();
        public List<Follow> Followers { get; set; } = new();
        //public List<PostLike> PostLikes { get; set; } = new();
        //public List<PostComment> PostComments { get; set; } = new();
        public List<string> Posts { get; set; } = new();
        public List<string> PostFavorites { get; set; } = new();

    }
    public enum Genders { Male, Female, NonBinary}
    public enum Roles { Admin, User, Pengurus }


}
