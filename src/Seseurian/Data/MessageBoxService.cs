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
namespace Seseurian.Data
{
    public class MessageBoxService : ICrud<MessageBox>
    {
        //SeseurianDB db;
        RedisConnectionProvider provider;
        //IRedisCollection<MessageBox> db;

        IDocumentSession db; 
        public MessageBoxService( IDocumentStore store)
        {
            db = store.OpenSession();

            this.provider = provider;
            //db = this.provider.RedisCollection<MessageBox>();
        }

        public void RefreshEntity(MessageBox item)
        {
            try
            {
                db.Advanced.Refresh(item);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }


        }
        public bool DeleteData(MessageBox item)
        {
            db.Delete(item);
            db.SaveChanges();
            return true;
        }

        public int GetUnreadMessageByUser(string username)
        {
            
            var data = db.Query<MessageBox>().Where(x => x.Username == username && !x.IsRead).Count();
            return data;
        }
        
        public MessageBox GetInboxByUid(string uid, string username)
        {
            var data = db.Query<MessageBox>().Where(x => x.Username == username && x.Uid == uid).FirstOrDefault();
            return data;
        } 
        
        public MessageBox GetInboxByFromAndTo(string FromUsername,string ToUsername)
        {
            var data = db.Query<MessageBox>().Where(x => x.Username == FromUsername && x.ToUsername == ToUsername).FirstOrDefault();
            return data;
        }

        public List<MessageBox> FindByKeyword(string Keyword)
        {
            var data = db.Query<MessageBox>().Where(x => x.Title.Contains(Keyword));
            return data.ToList();
        }
        public List<MessageBox> GetLatestMessage(string username)
        {
            var data = db.Query<MessageBox>().Where(x => x.Username == username).ToList();
            return data.Where(x => !x.IsRead).OrderByDescending(x => x.CreatedDate).ToList();
        }
        public List<Inbox> LoadInbox(string username)
        {
            try
            {
                var messages = GetLatestMessage(username);
                var exist_user = messages.Select(x => x.ToUsername).ToList();
                var currentUser = db.Query<UserProfile>().Where(x => x.Username == username).FirstOrDefault();
                var friend_usernames = currentUser?.Follows.Where(x => !exist_user.Contains(x.FollowUsername)).Select(x => x.FollowUser);
                var friends = db.Load<UserProfile>(friend_usernames);
                var list_inbox = new List<Inbox>();
                UserProfile usr;
                foreach (var item in messages)
                {
                    list_inbox.Add(new Inbox() { Message = item, User = db.Load<UserProfile>(item.ToUserId) });
                }
                foreach (var friend in friends)
                {
                    list_inbox.Add(new Inbox() { Message = new() { }, User = friend.Value });
                }
                return list_inbox;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return default;
           
        }

        public List<MessageBox> GetAllData()
        {
            return db.Query<MessageBox>().ToList();
        }

        public MessageBox GetDataById(string Id)
        {
            return db.Query<MessageBox>().Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(MessageBox data)
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

        public bool UpdateData(MessageBox data)
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
    }

}
