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
    public class LogService : ICrud<Log>
    {
        //SeseurianDB db;
        RedisConnectionProvider provider;
        IRedisCollection<Log> db;
        public LogService(RedisConnectionProvider provider)
        {
            this.provider = provider;
            db = this.provider.RedisCollection<Log>();
        }
        public bool DeleteData(Log item)
        {
            db.Delete(item);
            return true;
        }

        public List<Log> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.Message.Contains(Keyword));
            return data.ToList();
        }

        public List<Log> GetAllData()
        {
            return db.ToList();
        }

        public Log GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(Log data)
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

        public bool UpdateData(Log data)
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
