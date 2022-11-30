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
        public MessageBoxService(RedisConnectionProvider provider, IDocumentStore store)
        {
            db = store.OpenSession();

            this.provider = provider;
            //db = this.provider.RedisCollection<MessageBox>();
        }
        public bool DeleteData(MessageBox item)
        {
            db.Delete(item);
            db.SaveChanges();
            return true;
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
