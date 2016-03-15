using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using System.Security.Principal;
using System.Threading.Tasks;
using MySensei.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MySensei.Models;
using System.Data.Entity;
using System.Linq;
using System.Data.SqlClient;

namespace MySensei.Controllers
{
    public class HomeController : Controller
    {
        AppIdentityDbContext db = new AppIdentityDbContext();
        // GET: Home
        public ActionResult Index()
        {
            //Declare variable null becase course
            IEnumerable<Course> courses = null;
            try
            {
                //Use the stored procedure to get the 6 random courses
                courses = db.Database.SqlQuery<Course>
                    (
                    "EXEC dbo.GetRandomCourses @coursesToReturn", new SqlParameter("coursesToReturn", 6)
                    )
                    .ToList();
            }
            catch (SqlException e)//Catch and make sure course is null
            {
                switch (e.Number)
                {
                    case 2812:
                        courses = null;
                        break;
                    case 21343:
                        courses = null;
                        break;
                    default:
                        throw;
                }
            }
 
            //If the stored procedure isn't in the DB, then this will make it fall back to using the top 6 courses instead.
            if(courses == null)
            {
                courses = db.Courses.Take(6);
            }
            else
            {
                foreach (var course in courses)
                {   
                    //Get the userdata for the randomly selected courses manually
                    course.AppUser = UserManager.FindById(course.AppUserId);
                }
            }
            return View(courses);
        }

        //Stuff i didn't write
        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }
    }
}