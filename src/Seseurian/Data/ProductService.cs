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


using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using static System.Formats.Asn1.AsnWriter;
using Seseurian.Helpers;
using GemBox.Document;

namespace Seseurian.Data
{
    public class ProductService : ICrud<Product>
    {
        //SeseurianDB db;
        RedisConnectionProvider provider;
        //IRedisCollection<Product> db;
        IDocumentSession db;
        public ProductService( IDocumentStore store)
        {
            db = store.OpenSession();


            //this.provider = provider;
            //db = this.provider.RedisCollection<Product>();
        }
        public bool DeleteData(Product item)
        {
            db.Delete(item);
            db.SaveChanges();
            return true;
        }

        public List<Product> FindByKeyword(string Keyword)
        {
            var data = db.Query<Product>().Where(x => x.Name.Contains(Keyword));
            return data.ToList();
        }

        public List<Product> GetAllData()
        {
            return db.Query<Product>().ToList();
        } 
        public List<Product> GetLatest(int Limit=30)
        {
            db.Advanced.Clear();
            return db.Query<Product>().OrderByDescending(x=>x.CreatedDate).Take(Limit).ToList();
        }
        
        public List<Product> GetByCategory(string Category, int Limit=30)
        {
            db.Advanced.Clear();
            return db.Query<Product>().Where(x=>x.Category == Category).OrderByDescending(x=>x.CreatedDate).Take(Limit).ToList();
        } 
        
        public List<Product> GetHits(int Limit=30)
        {
            return db.Query<Product>().OrderByDescending(x=>x.CreatedDate).ThenByDescending(x=>x.Views.Count).ThenByDescending(x=>x.Rating).Take(Limit).ToList();
        }

        public Product GetDataById(string Id)
        {
            return db.Query<Product>().Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(Product data)
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

        public bool UpdateData(Product data)
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
    }

}
