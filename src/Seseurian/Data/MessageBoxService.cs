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
    public class MessageBoxService : ICrud<MessageBox>
    {
        //SeseurianDB db;
        RedisConnectionProvider provider;
        IRedisCollection<MessageBox> db;
        public MessageBoxService(RedisConnectionProvider provider)
        {
            this.provider = provider;
            db = this.provider.RedisCollection<MessageBox>();
        }
        public bool DeleteData(MessageBox item)
        {
            db.Delete(item);
            return true;
        }

        public List<MessageBox> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.Title.Contains(Keyword));
            return data.ToList();
        }

        public List<MessageBox> GetAllData()
        {
            return db.ToList();
        }

        public MessageBox GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(MessageBox data)
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

        public bool UpdateData(MessageBox data)
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
