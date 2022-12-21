using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Seseurian.Models;
using GemBox.Spreadsheet;
using System.Drawing;
using Seseurian.Tools;
using Redis.OM.Searching;
using Redis.OM;
using ChartJs.Blazor.Common.Axes;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using static System.Formats.Asn1.AsnWriter;
using Raven.Client.Documents.Linq;

namespace Seseurian.Data
{
    public class UserProfileService : ICrud<UserProfile>
    {
        IDocumentSession db;
        //SeseurianDB db;
        RedisConnectionProvider provider;
        //IRedisCollection<UserProfile> db;
        public UserProfileService(RedisConnectionProvider provider, IDocumentStore store)
        {
            db = store.OpenSession();
            this.provider = provider;
            //db = this.provider.RedisCollection<UserProfile>();
        }
        public bool Login(string username, string password)
        {
            bool isAuthenticate = false;
            var usr = db.Query<UserProfile>().Where(x => x.Username == username).FirstOrDefault();
            if (usr != null)
            {
                var enc = new Encryption();
                var pass = enc.Decrypt(usr.Password);
                isAuthenticate = pass == password;
            }
            return isAuthenticate;
        }
        public bool DeleteData(UserProfile item)
        {
            db.Delete(item);
            db.SaveChanges();
            return true;
        }

        public List<UserProfile> FindByKeyword(string Keyword)
        {
            var data = db.Query<UserProfile>().Where(x => x.Username.Contains(Keyword));
            return data.ToList();
        }

        public string GetProfilePic(string UserId)
        {
            var data = db.Load<UserProfile>(UserId);
            return data == null ? "assets/images/avatars/avatar-2.jpg" : data.PicUrl;
        }

        public List<UserProfile> GetAllData()
        {
            return db.Query<UserProfile>().ToList();
        }
        public List<UserProfile> GetFollowers(string username)
        {
            var user = db.Query<UserProfile>().Where(x => x.Username == username).FirstOrDefault();
            if (user != null)
            {
                var userids = user.Followers.Select(x => x.FollowUser).ToList();
                var users = db.Load<UserProfile>(userids).Select(x=>x.Value).ToList();
                return users;
            }else
            return default;
        }
        public List<UserProfile> GetFollows(string username)
        {
            var user = db.Query<UserProfile>().Where(x => x.Username == username).FirstOrDefault();
            var userids = user.Follows.Select(x => x.FollowUser).ToList();
            var users = db.Load<UserProfile>(userids).Select(x=>x.Value).ToList();
            return user == null ? default : users;
        }

        public UserProfile GetDataById(string Id)
        {
            return db.Query<UserProfile>().Where(x => x.Id == Id).FirstOrDefault();
        }

        public Dictionary<string,UserProfile> GetProfiles (string[] userids)
        {
            var persons = db.Load<UserProfile>(userids);
            return persons;
        }
        public UserProfile? GetProfile (string userids)
        {
            if (string.IsNullOrEmpty(userids)) return new() { FullName = "anonymous" };
            var persons = db.Load<UserProfile>(userids);
            return persons;
        }
        public bool InsertData(UserProfile data)
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

        public bool UpdateData(UserProfile data)
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
        public UserProfile GetItemByUsername(string UName)
        {
            if (string.IsNullOrEmpty(UName)) return null;
            //var selItem = db.Where(x => x.Username.ToLower() == UName.ToLower()).FirstOrDefault();
            var selItem = db.Query<UserProfile>().Where(x => x.Username == UName).FirstOrDefault();
            return selItem;
        }
        public UserProfile GetItemByEmail(string Email)
        {
            try
            {
                if (string.IsNullOrEmpty(Email)) return null;
                var selItem = db.Query<UserProfile>().Where(x => x.Email == Email).FirstOrDefault();
                return selItem;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return default;
        }
        public Roles GetUserRole(string Email)
        {
            var selItem = db.Query<UserProfile>().Where(x => x.Username == Email).FirstOrDefault();
            return selItem.Role;
        } 
        
        public void RefreshEntity(UserProfile user)
        {
            db.Advanced.Refresh(user);
            //return user;
        }
        public UserProfile GetUserByEmail(string Email)
        {
            
            var selItem = db.Query<UserProfile>().Where(x => x.Username == Email).FirstOrDefault();
            return selItem;
        }
        public UserProfile GetItemByPhone(string Phone)
        {
            var selItem = db.Query<UserProfile>().Where(x => x.Phone.ToLower() == Phone.ToLower()).FirstOrDefault();
            return selItem;
        }
       
        public bool IsUserExists(string Email)
        {
            if (string.IsNullOrEmpty(Email)) return true;
            //if (db.UserProfiles.Count() <= 0 ) return false;
            //if (db.Count() <= 0) return false;
            
            var exists = db.Query<UserProfile>().Any(x => x.Username == Email);
            return exists;
        }

        public bool isValidLogin(string username, string password)
        {
            var anyUser = db.Query<UserProfile>().Where(x => x.Username == username).FirstOrDefault();
            if (anyUser != null) {
                var enc = new Encryption();
                var pass = enc.Decrypt(anyUser.Password);
                if (password == pass) return true;
            }
            return false;

        }

        public List<PopularPeople> GetPopularPeople(string Username, int Number = 5)
        {
            var currentUser = db.Query<UserProfile>().Where(x => x.Username == Username).FirstOrDefault();
            var IFollow = currentUser.Follows;
            var DontFollowIds = IFollow.Select(x => x.FollowUsername).ToList();
            var notFollowByMeList = from x in db.Query<UserProfile>()
                                    where x.Username != Username && !x.Username.In(DontFollowIds)//.Contains(x.Username)
                                    select x;
            if (notFollowByMeList == null || notFollowByMeList.Count() <= 0) return default;

            var listPopular = notFollowByMeList.OrderByDescending(x => x.Followers.Count).Take(Number).Select(x=>new PopularPeople(x.Username,x.Followers.Count,x));
            return listPopular.ToList();
        }
        public List<PeopleByJob> GetPeopleByJob(string Username, int Number = 5)
        {
            
            var retVal = new List<PeopleByJob>();
            var count = db.Query<UserProfile>().Count() - 1;
            if (count <= 0) return default;
            List<UserProfile> data;
            if (!string.IsNullOrEmpty(Username))
            {
                var currentUser = db.Query<UserProfile>().Where(x => x.Username == Username).FirstOrDefault();
                var IFollow = currentUser.Follows;
              
                var DontFollowIds = IFollow.Select(x => x.FollowUsername).ToList();
                data = (from x in db.Query<UserProfile>()
                        where !x.Username.In(DontFollowIds) && x.Username != Username
                        select x).ToList();

            }
            else
            {
                data = (from x in db.Query<UserProfile>()
                        where x.Username != Username
                        select x).ToList();
            }
            if (data != null && data.Count > 0)
            {

                var jobs = data.Select(x => x.Pekerjaan).Distinct();
                foreach (var job in jobs)
                {
                    var newJob = new PeopleByJob() { Job = job, Users = data.Where(x => x.Pekerjaan == job).Select(x => x).ToList() };
                    retVal.Add(newJob);
                }
                return retVal;
            }
            return default;
        }
        public List<UserProfile> GetRandomPeople(string Username, int Number = 5)
        {
            var count = db.Query<UserProfile>().Count() - 1;
            if (count <= 0) return default;
            var take = count > Number ? Number : count;
            Random rnd = new Random(Environment.TickCount);
            if (!string.IsNullOrEmpty(Username))
            {
                var currentUser = db.Query<UserProfile>().Where(x => x.Username == Username).FirstOrDefault();
                var IFollow = currentUser.Follows;
                var DontFollowIds = IFollow.Select(x => x.FollowUsername).ToList();
                var data = from x in db.Query<UserProfile>()
                           where !x.Username.In(DontFollowIds) && x.Username != Username
                           select x;
                return data.Take(take).ToList();
            }
            else
            {
                var data = from x in db.Query<UserProfile>()
                           where x.Username != Username
                           select x;
                return data.Take(take).ToList();
            }
        }

        public bool UnFollowUser(UserProfile CurrentUser, UserProfile FollowUser)
        {
            try
            {
               
                var removeItem = CurrentUser.Follows.Where(x=>x.FollowUsername == FollowUser.Username).FirstOrDefault();
                if(removeItem!=null)
                    CurrentUser.Follows.Remove(removeItem);
                
                var removeItem2 = FollowUser.Followers.Where(x => x.FollowUsername == CurrentUser.Username).FirstOrDefault();
                if(removeItem2!=null)
                    FollowUser.Followers.Remove(removeItem2);

                db.Store(CurrentUser);
                db.Store(FollowUser);

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public bool FollowUser(UserProfile CurrentUser, UserProfile FollowUser)
        {
            try
            {
                var newItem = new Follow() { FollowDate = DateHelper.GetLocalTimeNow(), FollowUsername=FollowUser.Username, FollowUser = FollowUser.Id };
                CurrentUser.Follows.Add(newItem);

                var newItem2 = new Follow() { FollowDate = DateHelper.GetLocalTimeNow(), FollowUsername = CurrentUser.Username, FollowUser = CurrentUser.Id };
                FollowUser.Followers.Add(newItem2);

                db.Store(CurrentUser);
                db.Store(FollowUser);

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        #region Excel
        public static List<UserProfile> ReadExcel(string PathFile)
        {
            try
            {
                Encryption enc = new Encryption();
                var DefaultPass = enc.Encrypt("354jaya");
                var DataUserProfile = new List<UserProfile>();
                //SpreadsheetInfo.SetLicense(AppConstants.GemLic);

                if (!File.Exists(PathFile)) return null;
                var workbook = ExcelFile.Load(PathFile);


                // Iterate through all worksheets in an Excel workbook.
                var worksheet = workbook.Worksheets[0];
                var counter = 0;
                // Iterate through all rows in an Excel worksheet.
                foreach (var row in worksheet.Rows)
                {
                    if (counter > 0)
                    {
                        var newUserProfile = new UserProfile();
                        //newUserProfile.KK = row.Cells[0].Value?.ToString();
                        //newUserProfile.NoUrut = int.Parse(row.Cells[1].Value?.ToString());

                        newUserProfile.FullName = row.Cells[1].Value?.ToString().Trim();
                        newUserProfile.Alamat = row.Cells[2].Value?.ToString().Trim();
                        newUserProfile.Phone = row.Cells[3].Value?.ToString().Trim();
                        newUserProfile.Email = row.Cells[4].Value?.ToString().Trim();
                        newUserProfile.Aktif = row.Cells[5].Value?.ToString().Trim() == "Ya" ? true:false;
                        newUserProfile.KTP = row.Cells[6].Value?.ToString().Trim();
                        newUserProfile.Daerah = row.Cells[7].Value?.ToString().Trim();
                        newUserProfile.Desa = row.Cells[8].Value?.ToString().Trim();
                        newUserProfile.Kelompok = row.Cells[9].Value?.ToString().Trim();
                        newUserProfile.Username = string.IsNullOrEmpty(row.Cells[10].Value?.ToString().Trim()) ? MailHelper.GenerateEmailFromName(newUserProfile.FullName.Trim(), "Seseurian.online") :  row.Cells[10].Value?.ToString().Trim();
                        if (string.IsNullOrEmpty(newUserProfile.Email))
                        {
                            newUserProfile.Email = newUserProfile.Username;
                        }
                        newUserProfile.Role = row.Cells[11].Value?.ToString().Trim() == "Ya" ? Roles.Pengurus  : Roles.User;
                        newUserProfile.Password = DefaultPass;
                        DataUserProfile.Add(newUserProfile);
                    }
                    counter++;


                }

                return DataUserProfile;
            }
            catch
            {
                return null;
            }

        }

        public OutputCls ImportData(List<UserProfile> NewData)
        {
            var output = new OutputCls();
            try
            {
                Encryption enc = new Encryption();
                var currentData = db.Query<UserProfile>().ToList();
                foreach (var item in NewData)
                {
                    //UserProfile? existing = null;
                    //foreach(var x in currentData)
                    //{
                    //    if(x.Username == item.Username)
                    //    {
                    //        existing = x; break;
                    //    }
                    //}
                    var existing = currentData.Where(x => x.Username.Equals(item.Username,StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (existing == null)
                    {
                        item.Password = enc.Encrypt(AppConstants.DefaultPass);
                        db.Store(item);
                    }
                    else
                    {
                        existing.FullName = item.FullName;
                        existing.Alamat = item.Alamat;
                        existing.Phone = item.Phone;
                        existing.Email = item.Email;
                        existing.Aktif = item.Aktif;
                        existing.KTP = item.KTP;
                        existing.Daerah = item.Daerah;
                        existing.Desa = item.Desa;
                        existing.Kelompok = item.Kelompok;
                        //existing.Username =   item.Username  ;
                        existing.Role = item.Role;
                    }
                }
                db.SaveChanges();
                output.IsSucceed = true;
               
            }
            catch (Exception ex)
            {
                output.IsSucceed = false;
                output.Message = ex.ToString();
                Console.WriteLine(ex);
             
            }
            return output;
        }

        public byte[] ExportToExcel()
        {
            // If using Professional version, put your serial key below.
            //SpreadsheetInfo.SetLicense(AppConstants.GemLic);

            var workbook = new ExcelFile();
            var worksheet = workbook.Worksheets.Add("Anggota");
            var datas = GetAllData();
            int row = 1;

            var styleHeader = new CellStyle();
            styleHeader.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            styleHeader.VerticalAlignment = VerticalAlignmentStyle.Center;
            styleHeader.Font.Weight = ExcelFont.BoldWeight;
            styleHeader.Font.Color = Color.Black;
            styleHeader.WrapText = true;
            styleHeader.Borders.SetBorders(MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Top | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);

         

            worksheet.Cells[0, 0].Value = "No";
            worksheet.Cells[0, 1].Value = "Nama";
            worksheet.Cells[0, 2].Value = "Alamat";
            worksheet.Cells[0, 3].Value = "Telepon";
            worksheet.Cells[0, 4].Value = "Email";
            worksheet.Cells[0, 5].Value = "Aktif";
            worksheet.Cells[0, 6].Value = "KTP";
            worksheet.Cells[0, 7].Value = "Daerah";
            worksheet.Cells[0, 8].Value = "Desa";
            worksheet.Cells[0, 9].Value = "Kelompok";
            worksheet.Cells[0, 10].Value = "Username";
            worksheet.Cells[0, 11].Value = "Pengurus";
          

            worksheet.Cells[0, 0].Style = styleHeader;
            worksheet.Cells[0, 1].Style = styleHeader;
            worksheet.Cells[0, 2].Style = styleHeader;
            worksheet.Cells[0, 3].Style = styleHeader;
            worksheet.Cells[0, 4].Style = styleHeader;
            worksheet.Cells[0, 5].Style = styleHeader;
            worksheet.Cells[0, 6].Style = styleHeader;
            worksheet.Cells[0, 7].Style = styleHeader;
            worksheet.Cells[0, 8].Style = styleHeader;
            worksheet.Cells[0, 9].Style = styleHeader;
            worksheet.Cells[0, 10].Style = styleHeader;
            worksheet.Cells[0, 11].Style = styleHeader;
         

            var style = new CellStyle();
            style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            style.VerticalAlignment = VerticalAlignmentStyle.Center;
            style.Font.Weight = ExcelFont.NormalWeight;
            style.Font.Color = Color.Black;
            style.WrapText = true;
            style.Borders.SetBorders(MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Top | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);

            foreach (var item in datas)
            {

             

                worksheet.Cells[row, 0].Value = row;
                worksheet.Cells[row, 1].Value = item.FullName;
                worksheet.Cells[row, 2].Value = item.Alamat;
                worksheet.Cells[row, 3].Value = item.Phone;
                worksheet.Cells[row, 4].Value = item.Email;
                worksheet.Cells[row, 5].Value = item.Aktif? "Ya":"Tidak";
                worksheet.Cells[row, 6].Value = item.KTP;
                worksheet.Cells[row, 7].Value = item.Daerah;
                worksheet.Cells[row, 8].Value = item.Desa;
                worksheet.Cells[row, 9].Value = item.Kelompok;
                worksheet.Cells[row, 10].Value = item.Username?.ToString();
                worksheet.Cells[row, 11].Value = item.Role == Roles.Pengurus?"Ya":"Tidak";
           

                worksheet.Cells[row, 0].Style = style;
                worksheet.Cells[row, 1].Style = style;
                worksheet.Cells[row, 2].Style = style;
                worksheet.Cells[row, 3].Style = style;
                worksheet.Cells[row, 4].Style = style;
                worksheet.Cells[row, 5].Style = style;
                worksheet.Cells[row, 6].Style = style;
                worksheet.Cells[row, 7].Style = style;
                worksheet.Cells[row, 8].Style = style;
                worksheet.Cells[row, 9].Style = style;
                worksheet.Cells[row, 10].Style = style;
                worksheet.Cells[row, 11].Style = style;
           
              
                row++;
            }
            var tmpfile = Path.GetTempFileName() + ".xlsx";

            workbook.Save(tmpfile);
            return File.ReadAllBytes(tmpfile);
        }
        #endregion
    }
}

