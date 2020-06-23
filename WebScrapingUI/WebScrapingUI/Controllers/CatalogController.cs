using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApi.Models;
using WebScrapingUI.EF;
using WebScrapingUI.Entities;
using WebScrapingUI.Repositories;
using Microsoft.AspNet.Identity;
using BaseScraperLibrary;

namespace WebScrapingUI.Controllers
{
    public class CatalogController : Controller
    {
        private RequestRepository repo = new RequestRepository();

        // GET: Catalog
        public ActionResult ShowCatalogs()
        {
            // GetAll Requests for current user from database

            HttpContext.Session["userId"] = User.Identity.GetUserId();
            var requests = repo.GetAll();

            return View(requests.Reverse());
        }

        public ActionResult Create()
        {
            HttpContext.Session["userId"] = User.Identity.GetUserId();
            // Add search in database
            RequestModel model = HttpContext.Session["model"] as RequestModel;
            Requests request = new Requests();

            request.ItemName = model.ItemName;
            request.MinPrice = model.MinPrice;
            request.MaxPrice = model.MaxPrice;
            request.ItemCount = model.ItemCount;
            if (model.Currency != null)
            {
                request.Currency = model.Currency.ToString();
            }
            string sites = string.Empty;
            for (int i = 0; i < model.CheckedSites.Count - 1; i++)
            {
                sites += model.CheckedSites[i] + ',';
            }
            sites += model.CheckedSites[model.CheckedSites.Count - 1];
            request.Sites = sites;

            Requests addedReq = repo.Create(request);

            TempData["Message"] = addedReq == null ? "Unable to save your search, please try later" : "Your search added successfully!";
            TempData["Color"] = addedReq == null ? "red" : "#5cb85c";

            return RedirectToAction("Result", "Result");
        }

        public ActionResult Get(int id)
        {
            // Get requestmodel from database

            Requests request = repo.Get(id);
            RequestModel model = new RequestModel();

            model.ItemName = request.ItemName;
            model.MinPrice = request.MinPrice;
            model.MaxPrice = request.MaxPrice;
            model.ItemCount = request.ItemCount;
            if (!string.IsNullOrEmpty(request.Currency))
                model.Currency = (AvailableCurrencies)Enum.Parse(typeof(AvailableCurrencies), request.Currency);
            else
                model.Currency = null;
            foreach (string s in request.Sites.Split(','))
            {
                model.CheckedSites.Add(s);
            }

            TempData["model"] = model;
            return RedirectToAction("Result", "Result");
        }

        public ActionResult Delete(int id)
        {
            // Delete request from Requests table ( auto deletes from RequestsOrders table ought to foreign key )

            repo.Delete(id);

            return RedirectToAction("ShowCatalogs");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repo.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}