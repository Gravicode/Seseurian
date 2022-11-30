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
using Seseurian.Helpers;
using GemBox.Document;

namespace Seseurian.Data
{
    public class LogService : ICrud<Log>
    {
        //SeseurianDB db;
        RedisConnectionProvider provider;
        //IRedisCollection<Log> db;
        IDocumentSession db;
        public LogService(RedisConnectionProvider provider, IDocumentStore store)
        {
            db = store.OpenSession();
           

            //this.provider = provider;
            //db = this.provider.RedisCollection<Log>();
        }
        public bool DeleteData(Log item)
        {
            db.Delete(item);
            db.SaveChanges();
            return true;
        }

        public List<Log> FindByKeyword(string Keyword)
        {
            var data = db.Query<Log>().Where(x => x.Message.Contains(Keyword));
            return data.ToList();
        }

        public List<Log> GetAllData()
        {
            return db.Query<Log>().ToList();
        }

        public Log GetDataById(string Id)
        {
            return db.Query<Log>().Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(Log data)
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

        public bool UpdateData(Log data)
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
