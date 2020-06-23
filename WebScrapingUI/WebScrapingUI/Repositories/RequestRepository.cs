using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebScrapingUI.EF;
using WebScrapingUI.Entities;
using WebScrapingUI.Interfaces;

namespace WebScrapingUI.Repositories
{
    public class RequestRepository : IRepository<Requests>, IDisposable
    {
        private ModelContext context;

        public RequestRepository()
        {
            this.context = new ModelContext();
        }

        public IList<Requests> GetAll()
        {
            string userId = HttpContext.Current.Session["userId"] as string;
            var requests = this.context.Requests.ToList();
            var orders = this.context.RequestsOrders.Where(u => u.UserId == userId).ToList();
            var UserRequests = from req in requests
                               join order in orders on req.RequestId equals order.RequestId
                               select req;
            return UserRequests.ToList();
        }

        public Requests Get(int id)
        {
            return this.context.Requests.Find(id);
        }

        public Requests Create(Requests request)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                Requests req = null;
                try
                {
                    string userId = HttpContext.Current.Session["userId"] as string;
                    req = this.context.Requests.Add(request);
                    this.context.SaveChanges();

                    RequestsOrders reqOrder = new RequestsOrders { RequestId = req.RequestId, UserId = userId };
                    RequestsOrders addeduserReq = this.context.RequestsOrders.Add(reqOrder);
                    this.context.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    transaction.Rollback();
                    return null;
                }
                return req;
            }
        }

        public void Delete(int id)
        {
            Requests request = this.context.Requests.Find(id);
            if (request != null)
            {
                this.context.Requests.Remove(request);
                this.context.SaveChanges();
            }
        }

        public void Dispose()
        {
            this.context?.Dispose();
            this.context = null;
        }

        //public void Update(Requests request)
        //{
        //    this.context.Entry(request).State = EntityState.Modified;
        //    this.context.SaveChanges();
        //}

        //public IEnumerable<Requests> Find(Func<Requests, Boolean> predicate)
        //{
        //    return context.Requests.Where(predicate).ToList();
        //}
    }
}