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
    public class TrendingService : ICrud<Trending>
    {
        //SeseurianDB db;
        RedisConnectionProvider provider;
        IRedisCollection<Trending> db;
        public TrendingService(RedisConnectionProvider provider)
        {
            this.provider = provider;
            db = this.provider.RedisCollection<Trending>();
        }
        public bool DeleteData(Trending item)
        {
            db.Delete(item);
            return true;
        }

        public List<Trending> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.Hashtag.Contains(Keyword));
            return data.ToList();
        }

        public List<Trending> GetAllData()
        {
            return db.ToList();
        }  
        
        public List<Trending> GetTrending(int count=10)
        {
            return db.OrderByDescending(x=>x.CreatedDate).Take(count).ToList();
        }

        public Trending GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(Trending data)
        {
            try
            {
                db.Insert(data);
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
                db.Update(data);
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
