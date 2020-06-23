using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebScrapingUI.Entities;

namespace WebScrapingUI.EF
{
    public class ModelContext : DbContext
    {
        public DbSet<Requests> Requests { get; set; }

        public DbSet<RequestsOrders> RequestsOrders { get; set; }


        public ModelContext() : base(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString)
        {

        }
        //static ModelContext()
        //{ 
        //Database.SetInitializer<ModelContext>(new StoreDbInitializer)
        //}
    }

    //public class StoreDbInitializer : DropCreateDatabaseIfModelChanges<ModelContext>
    //{
    //    protected override void Seed(ModelContext context)
    //    {

    //    }
    //}
}