using Microsoft.AspNet.Identity;
using MySensei.Infrastructure;
using MySensei.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MySensei.Controllers
{
    public class CatalogController : Controller
    {
        AppIdentityDbContext db = new AppIdentityDbContext();

        public ActionResult Index(string SearchQuery)
        {
            //Declare variable now, because scope
            IEnumerable<Course> courses;
            if (SearchQuery != null && SearchQuery != "")
            {
                //Get the search query to fill back into the search field
                ViewBag.SQ = SearchQuery;
                //Find courses based on the search query
                courses = db.Courses.Where(
                    c =>
                    c.Title.Contains(SearchQuery) ||//Search title
                    c.Description.Contains(SearchQuery) ||//Search description
                    c.AppUser.UserName.Contains(SearchQuery)//Search unsername of owner
                );
            }
            else
            {
                //If no search query is set, return all courses
                courses = db.Courses; ;
            }
            return View(courses);
        }
    }
}