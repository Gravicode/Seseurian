using Microsoft.EntityFrameworkCore;
using Seseurian.Data;
using Seseurian.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seseurian.Data
{
    public class PostCommentService : ICrud<PostComment>
    {
        SeseurianDB db;

        public PostCommentService()
        {
            if (db == null) db = new SeseurianDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.PostComments.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.PostComments.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<PostComment> FindByKeyword(string Keyword)
        {
            var data = from x in db.PostComments
                       where x.Comment.Contains(Keyword)
                       select x;
            return data.ToList();
        }
        
        public List<PostComment> GetCommentByPostId(long PostId)
        {
            var data = from x in db.PostComments.Include(x=>x.User)
                       where x.PostId == PostId
                       select x;
            return data.ToList();
        }

        public List<PostComment> GetAllData()
        {
            return db.PostComments.OrderBy(x => x.Id).ToList();
        }

        public PostComment GetDataById(object Id)
        {
            return db.PostComments.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(PostComment data)
        {
            try
            {
                db.PostComments.Add(data);
                db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;

        }



        public bool UpdateData(PostComment data)
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
            return db.PostComments.Max(x => x.Id);
        }
    }

}
/*











 */
