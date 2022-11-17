using Microsoft.EntityFrameworkCore;
using Seseurian.Data;
using Seseurian.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seseurian.Data
{
    public class FollowService : ICrud<Follow>
    {
        SeseurianDB db;

        public FollowService()
        {
            if (db == null) db = new SeseurianDB();

        }
      
        public List<PopularPeople> GetPopularPeople(string Username, int Number = 5)
        {
            var IFollow = GetFollowBy(Username);
            var DontFollowIds = IFollow.Select(x => x.FollowUserId).ToList();
            var notFollowByMeList =from x in db.Follows.Include(c => c.User).Include(c => c.FollowUser)
                                           where x.UserName != Username && x.FollowUserName != Username && !DontFollowIds.Contains(x.FollowUserId)
                                           select x;
            if (notFollowByMeList == null || notFollowByMeList.Count() <= 0 ) return default;
            
            var listPopular = notFollowByMeList.GroupBy(info => info.FollowUserName)
                        .Select(group => new PopularPeople(group.Key,
                            group.Count(),
                            group.First().FollowUser)
                        ).AsEnumerable()
                        .OrderBy(x => x.Username).Take(Number).ToList();
            return listPopular;
        }
   
        public List<UserProfile> GetRandomPeople(string Username, int Number = 10)
        {
            var count = db.UserProfiles.Count() - 1;
            if (count <= 0) return default;
            var take = count > Number ? Number : count;
            Random rnd = new Random(Environment.TickCount);
            if (!string.IsNullOrEmpty(Username))
            {
                var IFollow = GetFollowBy(Username);
                var DontFollowIds = IFollow.Select(x => x.FollowUserId).ToList();
                var data = from x in db.UserProfiles
                           where !DontFollowIds.Contains(x.Id) && x.Username != Username
                           select x;
                return data.Take(take).ToList();
            }
            else
            { 
                var data = from x in db.UserProfiles
                           where x.Username != Username
                           select x;
                return data.Take(take).ToList();
            }
        }

        public bool UnFollowUser(long userid, long followuserid)
        {
            try
            {
                var removeFollow = db.Follows.Where(x => x.UserId == userid && x.FollowUserId == followuserid).FirstOrDefault();
                if (removeFollow != null)
                {
                    db.Follows.Remove(removeFollow);
                }
            
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public bool FollowUser(string username,long userid, string followusername,long followuserid)
        {
            try
            {
                var newFollow = new Follow() { FollowDate = DateHelper.GetLocalTimeNow(), FollowUserName = followusername, FollowUserId = followuserid, UserId = userid, UserName = username };
                db.Follows.Add(newFollow);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public bool DeleteData(object Id)
        {
            var selData = (db.Follows.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Follows.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Follow> FindByKeyword(string Keyword)
        {
            var data = from x in db.Follows
                       where x.UserName.Contains(Keyword) || x.FollowUserName.Contains(Keyword)
                       select x;
            return data.ToList();
        }  
        
        public List<Follow> GetMyFollower(string Username)
        {
            var data = from x in db.Follows.Include(c=>c.User).Include(c=>c.FollowUser)
                       where x.FollowUserName == Username
                       select x;
            return data.ToList();
        } 
        
        public List<Follow> GetFollowBy(string Username)
        {
            var data = from x in db.Follows.Include(c=>c.User).Include(c=>c.FollowUser)
                       where x.UserName == Username
                       select x;
            return data.ToList();
        }

        public List<Follow> GetAllData()
        {
            return db.Follows.OrderBy(x => x.Id).ToList();
        }

        public Follow GetDataById(object Id)
        {
            return db.Follows.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Follow data)
        {
            try
            {
                db.Follows.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(Follow data)
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
            return db.Follows.Max(x => x.Id);
        }
    }

}
/*











 */
