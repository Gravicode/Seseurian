using GemBox.Document;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Reflection;

namespace Seseurian.Models
{
    #region helpers model
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

    [Table("message_header")]
    public class MessageHeader
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public string Uid { set; get; }
        [ForeignKey(nameof(FromUser)), Column(Order = 0)]
        public long FromUserId { set; get; }
        public UserProfile FromUser { set; get; }
        [ForeignKey(nameof(User)), Column(Order = 1)]
        public long UserId { set; get; }
        public UserProfile User { set; get; }
        public string? Title { set; get; }
        public string? Desc { set; get; }
        public DateTime CreatedDate { set; get; }
        //public MessageTypes MessageType { set; get; }
        //public string MemberIds { set; get; }
        //public int MemberCount { set; get; } = 1;
        public string? LastMessage { set; get; }
        public DateTime LastUpdate { set; get; }
        public bool IsArchived { set; get; } = false;
        public bool IsRead { set; get; } = false;
        //public string? WallpaperUrl { set; get; }
        public bool IsMuted { set; get; } = false;
        public bool IsBlocked { set; get; } = false;
    }

    [Table("message_detail")]
    public class MessageDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public string Uid { set; get; }
        [ForeignKey(nameof(MessageHeader)), Column(Order = 0)]
        public long MessageHeaderId { set; get; }
        public MessageHeader MessageHeader { get; set; }
        public DateTime CreatedDate { set; get; }
        [ForeignKey(nameof(FromUser)), Column(Order = 1)]
        public long FromUserId { set; get; }
        public UserProfile FromUser { set; get; }
        public string Message { set; get; }

        public bool HasAttachment { get; set; } = false;

        [InverseProperty(nameof(MessageAttachment.MessageDetail))]
        public ICollection<MessageAttachment> MessageAttachments { get; set; }
    }
    [Table("message_attachment")]
    public class MessageAttachment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [ForeignKey(nameof(MessageDetail)), Column(Order = 0)]
        public long MessageDetailId { set; get; }
        public MessageDetail MessageDetail { set; get; }
        public string MessageHeaderUid { set; get; }
        public string MessageDetailUid { set; get; }
        public string Title { set; get; }
        public string? Url { set; get; }
        public string? Desc { set; get; }
        public string? Longitude { set; get; }
        public string? Latitude { set; get; }
       
        public AttachmentTypes AttachmentType { set; get; }
        public DateTime CreatedDate { set; get; }
    }
    public enum AttachmentTypes { Doc, Video, Audio, Link, Location };


    [Table("trending")]
    public class Trending
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string Hashtag { set; get; }
        public DateTime CreatedDate { set; get; }
        public string? Location { set; get; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
    }

    [Table("follow")]
    public class Follow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        //user
        [ForeignKey(nameof(User)), Column(Order = 0)]
        public long UserId { get; set; }
        public UserProfile User { set; get; }
        public string UserName { set; get; }
        //follow
        public string FollowUserName { set; get; }

        [ForeignKey(nameof(FollowUser)), Column(Order = 1)]
        public long FollowUserId { get; set; }
        public UserProfile FollowUser { set; get; }
        public DateTime FollowDate { set; get; }
    }


    [Table("postlike")]
    public class PostLike
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public Post Post { set; get; }
        [ForeignKey("Post")]
        public long PostId { set; get; }
        [ForeignKey("UserProfile")]
        public long LikedByUserId { set; get; }
        public string LikedByUserName { set; get; }
        public UserProfile LikedByUser { set; get; }
        public DateTime CreatedDate { set; get; }
    }

    [Table("postcomment")]
    public class PostComment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string Comment { set; get; }
        public Post Post { set; get; }
        [ForeignKey("Post")]
        public long PostId { set; get; }
        [ForeignKey("UserProfile")]
        public long UserId { set; get; }
        public string Username { set; get; }
        public DateTime CreatedDate { set; get; }
        public UserProfile User { set; get; }
        public ICollection<CommentLike> CommentLikes { get; set; }

    }
    [Table("commentlike")]
    public class CommentLike
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public PostComment Comment { set; get; }
        [ForeignKey("Comment")]
        public long CommentId { set; get; }
        [ForeignKey("UserProfile")]
        public long LikedByUserId { set; get; }
        public string LikedByUserName { set; get; }
        public UserProfile LikedByUser { set; get; }
        public DateTime CreatedDate { set; get; }
    }


    [Table("post")]
    public class Post
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [ForeignKey("UserProfile")]
        public long UserId { set; get; }
        public string UserName { set; get; }
        public DateTime CreatedDate { set; get; }
        public string Message { set; get; }
        public string? Mentions { set; get; }
        public string? LinkUrls { set; get; }
        public string? VideoUrls { set; get; }
        public string? DocUrls { set; get; }
        public string? ImageUrls { set; get; }
        public string? Hashtags { set; get; }

        public UserProfile User { get; set; }

        public ICollection<PostLike> PostLikes { get; set; }
        public ICollection<PostComment> PostComments { get; set; }

        public ICollection<CommentLike> CommentLikes { get; set; }
    }


    [Table("notification")]
    public class Notification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public DateTime CreatedDate { set; get; }
        [ForeignKey(nameof(User)), Column(Order = 0)]
        public long UserId { set; get; }
        public UserProfile User { set; get; }
        public string Action { set; get; }
        public string LinkUrl { set; get; }
        public string LinkDesc { set; get; }
        public string Message { set; get; }
        public bool IsRead { set; get; } = false;
    }
    public enum LogCategory
    {
        Info, Error, Warning
    }
    [Table("logs")]
    public class Log
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LogDate { get; set; }
        public string Message { get; set; }
        public LogCategory Category { get; set; }
    }

    [Table("userprofile")]
    public class UserProfile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Alamat { get; set; }
        public string? KTP { get; set; }
        public string? PicUrl { get; set; }
        public bool Aktif { get; set; } = true;
        public string? Daerah { get; set; }
        public string? Desa { get; set; }
        public string? Kelompok { get; set; }
        public Char Gender { get; set; } = 'N';
        public Roles Role { set; get; } = Roles.User;
        public DateTime CreatedDate { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        public string? FirstName { set; get; }
        public string? LastName { set; get; }
      
        public string? AboutMe { set; get; }
        public string? Tags { set; get; }



        [InverseProperty(nameof(Follow.FollowUser))]
        public ICollection<Follow> Follows { get; set; }

        [InverseProperty(nameof(Follow.User))]
        public ICollection<Follow> FollowedBy { get; set; }
        public ICollection<PostLike> PostLikes { get; set; }
        public ICollection<PostComment> PostComments { get; set; }
        public ICollection<Post> Posts { get; set; }

        [InverseProperty(nameof(MessageHeader.User))]
        public ICollection<MessageHeader> UserMessages { get; set; }

    }

    public enum Roles { Admin, User, Pengurus }


}
