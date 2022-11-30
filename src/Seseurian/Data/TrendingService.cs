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
    public class TrendingService : ICrud<Trending>
    {
        //SeseurianDB db;
        RedisConnectionProvider provider;
        //IRedisCollection<Trending> db;
        IDocumentSession db;
        public TrendingService(RedisConnectionProvider provider, IDocumentStore store)
        {
            this.provider = provider;
            db = store.OpenSession();
            //db = this.provider.RedisCollection<Trending>();
        }
        public bool DeleteData(Trending item)
        {
            db.Delete(item);
            db.SaveChanges();
            return true;
        }

        public List<Trending> FindByKeyword(string Keyword)
        {
            var data = db.Query<Trending>().Where(x => x.Hashtag.Contains(Keyword));
            return data.ToList();
        }
        public bool InsertFromPost(UserProfile user, Post data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.Hashtags))
                {
                    foreach (var hashtag in data.Hashtags.Split(';'))
                    {
                        var newTrend = new Trending() { CreatedDate = data.CreatedDate, Hashtag = hashtag, Latitude = user.Latitude, Longitude = user.Longitude, Location = user.Alamat };
                        db.Store(newTrend);
                    }
                    db.SaveChanges();
                }

             
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;

        }
        public List<Trending> GetAllData()
        {
            return db.Query<Trending>().ToList();
        }  
        
        public List<Trending> GetTrending(int count=10)
        {
            return db.Query<Trending>().OrderByDescending(x=>x.CreatedDate).Take(count).ToList();
        }

        public Trending GetDataById(string Id)
        {
            return db.Query<Trending>().Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(Trending data)
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

        public bool UpdateData(Trending data)
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
