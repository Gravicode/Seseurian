﻿using GemBox.Document;
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

    [Document(Prefixes = new[] { "MessageBox" })]
  
    public class MessageBox
    {
       
        [RedisIdField] public string Id{ get; set; }
        [Indexed(Sortable = true)]
        public string Username { get; set; }
        [Searchable(Sortable =true)]
        public string? Title { set; get; }
        public string? Desc { set; get; }
        [Indexed]
        public DateTime CreatedDate { set; get; }
        public string? LastMessage { set; get; }
        [Indexed]
        public DateTime LastUpdate { set; get; }
        public bool IsArchived { set; get; } = false;
        public bool IsRead { set; get; } = false;
        //public string? WallpaperUrl { set; get; }
        public bool IsMuted { set; get; } = false;
        public bool IsBlocked { set; get; } = false;
        public ICollection<MessageDetail> Chats { get; set; }
    }
    public class MessageDetail
    {
        public UserProfile FromUser { set; get; }
        public DateTime CreatedDate { set; get; }
        public string Message { set; get; }
        public bool HasAttachment { get; set; } = false;
        public ICollection<MessageAttachment> Attachments { get; set; }

    }
    public class MessageAttachment
    {
        public string Title { set; get; }
        public string? Url { set; get; }
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
        public UserProfile FollowUser { set; get; }
        public DateTime FollowDate { set; get; }
    }

    public class PostLike
    {
        public UserProfile LikedByUser { set; get; }
        public DateTime CreatedDate { set; get; }
    }
    public class PostComment
    {
        public string Comment { set; get; }
        public DateTime CreatedDate { set; get; }
        public UserProfile CommentByUser { set; get; }
        public ICollection<CommentLike> CommentLikes { get; set; }

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

        public bool IsRepost { get; set; } = false;

        public UserProfile PostByUser { get; set; }

        public ICollection<PostLike> PostLikes { get; set; }
        public ICollection<PostComment> PostComments { get; set; }

    }

    [Document(Prefixes = new[] { "Notification" })]
   
    public class Notification
    {
       
        [RedisIdField] public string Id{ get; set; }

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
   
    public class UserProfile
    {
      
        [RedisIdField] public string Id{ get; set; }
        [Indexed(Sortable =true)]
        public string Username { get; set; }
        public string Password { get; set; }
        [Searchable]
        public string FullName { get; set; }
        [Indexed]
        public string? Phone { get; set; }
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

        public ICollection<Follow> Follows { get; set; }
        public ICollection<Follow> Followers { get; set; }
        public ICollection<PostLike> PostLikes { get; set; }
        public ICollection<PostComment> PostComments { get; set; }
        public ICollection<Post> Posts { get; set; }

    }
    public enum Genders { Male, Female, NonBinary}
    public enum Roles { Admin, User, Pengurus }


}
