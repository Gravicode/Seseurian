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
    public class NotificationService : ICrud<Notification>
    {
        //SeseurianDB db;
        RedisConnectionProvider provider;
        //IRedisCollection<Notification> db;
        IDocumentSession db;

        public NotificationService(RedisConnectionProvider provider, IDocumentStore store)
        {
            db = store.OpenSession();
            this.provider = provider;
            //db = this.provider.RedisCollection<Notification>();
        }
        public bool DeleteData(Notification item)
        {
            db.Delete(item);
            db.SaveChanges();
            return true;
        }

        public List<Notification> FindByKeyword(string Keyword)
        {
            var data = db.Query<Notification>().Where(x => x.Message.Contains(Keyword));
            return data.ToList();
        }

        public List<Notification> GetAllData()
        {
            return db.Query<Notification>().ToList();
        } 
        
        public List<Notification> GetLatestNotifications(string username)
        {
            var data = db.Query<Notification>().Where(x => x.UserName == username).Where(x => x.IsRead == false).ToList();
            return data.OrderByDescending(x=>x.CreatedDate).ToList();
        }

        public Notification GetDataById(string Id)
        {
            return db.Query<Notification>().Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(Notification data)
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

        public bool UpdateData(Notification data)
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
