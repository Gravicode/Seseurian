using Microsoft.EntityFrameworkCore;
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
        SeseurianDB db;

        public TrendingService()
        {
            if (db == null) db = new SeseurianDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Trendings.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Trendings.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Trending> FindByKeyword(string Keyword)
        {
            var data = from x in db.Trendings
                       where x.Hashtag.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Trending> GetAllData()
        {
            return db.Trendings.OrderBy(x => x.Id).ToList();
        }

        public Trending GetDataById(object Id)
        {
            return db.Trendings.Where(x => x.Id == (long)Id).FirstOrDefault();
        }

        public bool InsertFromPost(UserProfile user, Post data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.Hashtags))
                {
                    foreach(var hashtag in data.Hashtags.Split(';'))
                    {
                        var newTrend = new Trending() { CreatedDate = data.CreatedDate, Hashtag = hashtag, Latitude = user.Latitude, Longitude = user.Longitude, Location = user.Alamat  };
                        db.Trendings.Add(newTrend);
                    }
                }
                
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }


        public bool InsertData(Trending data)
        {
            try
            {
                db.Trendings.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(Trending data)
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
            return db.Trendings.Max(x => x.Id);
        }
    }

}
/*











 */
