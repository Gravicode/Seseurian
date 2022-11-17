using Microsoft.EntityFrameworkCore;
using Seseurian.Data;
using Seseurian.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seseurian.Data
{
    public class MessageHeaderService : ICrud<MessageHeader>
    {
        SeseurianDB db;

        public MessageHeaderService()
        {
            if (db == null) db = new SeseurianDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.MessageHeaders.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.MessageHeaders.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<MessageHeader> FindByKeyword(string Keyword)
        {
            var data = from x in db.MessageHeaders
                       where x.Title.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<MessageHeader> GetAllData()
        {
            return db.MessageHeaders.OrderBy(x => x.Id).ToList();
        }

        public MessageHeader GetDataById(object Id)
        {
            return db.MessageHeaders.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(MessageHeader data)
        {
            try
            {
                db.MessageHeaders.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(MessageHeader data)
        {
            try
            {
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                /*
                if (sel != null)
                {
                    sel.Nama = data.Nama;
                    sel.Keterangan = data.Keterangan;
                    sel.Tanggal = data.Tanggal;
                    sel.DocumentUrl = data.DocumentUrl;
                    sel.StreamUrl = data.StreamUrl;
                    return true;

                }*/
                return true;
            }
            catch
            {

            }
            return false;
        }

        public long GetLastId()
        {
            return db.MessageHeaders.Max(x => x.Id);
        }
    }

}
/*











 */
