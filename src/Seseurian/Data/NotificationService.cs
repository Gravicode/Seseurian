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
    public class NotificationService : ICrud<Notification>
    {
        //SeseurianDB db;
        RedisConnectionProvider provider;
        IRedisCollection<Notification> db;
        public NotificationService()
        {
            provider = new RedisConnectionProvider(AppConstants.RedisCon);
            db = provider.RedisCollection<Notification>();
        }
        public bool DeleteData(Notification item)
        {
            db.Delete(item);
            return true;
        }

        public List<Notification> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.Message.Contains(Keyword));
            return data.ToList();
        }

        public List<Notification> GetAllData()
        {
            return db.ToList();
        }

        public Notification GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(Notification data)
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

        public bool UpdateData(Notification data)
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
