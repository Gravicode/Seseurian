using Microsoft.EntityFrameworkCore;
using Seseurian.Data;
using Seseurian.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seseurian.Data
{
    public class MessageDetailService : ICrud<MessageDetail>
    {
        SeseurianDB db;

        public MessageDetailService()
        {
            if (db == null) db = new SeseurianDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.MessageDetails.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.MessageDetails.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<MessageDetail> FindByKeyword(string Keyword)
        {
            var data = from x in db.MessageDetails
                       where x.Message.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<MessageDetail> GetAllData()
        {
            return db.MessageDetails.OrderBy(x => x.Id).ToList();
        }

        public MessageDetail GetDataById(object Id)
        {
            return db.MessageDetails.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(MessageDetail data)
        {
            try
            {
                db.MessageDetails.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(MessageDetail data)
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
            return db.MessageDetails.Max(x => x.Id);
        }
    }

}
/*











 */
