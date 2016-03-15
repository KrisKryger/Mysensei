using Microsoft.AspNet.Identity.Owin;
using MySensei.Infrastructure;
using MySensei.Models;
using MySensei.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;

namespace MySensei.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        AppIdentityDbContext db = new AppIdentityDbContext();

        [AllowAnonymous]
        public ActionResult Show(int? id)
        {
            //Declare variable now beacause scope
            bool isProtected = true;
            if (id != null)//If id is set
            {
                //Finde corresponding course by id
                var course = db.Courses.First(c => c.CourseId == id);
                ViewBag.Course = course;

                if (course.CourseStatusId == 1 || course.CourseStatusId == 2)
                {
                    //Check that the course is Public or just closed before showing it
                    isProtected = false;
                }
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    if (course.AppUserId == HttpContext.User.Identity.GetUserId())
                    {
                        //Override protection if a user is logged in and owns the course
                        isProtected = false;
                    }
                }
                
            } 
            //If the course is cant be allowed to be seen then return error
            if (id == null || isProtected)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
  
            return View();
        }

        public ActionResult Create()
        {
            //I dont think i wrote this
            var course = new Course();
            //BUT this gets the list over possible statuses that a course can have
            ViewBag.CourseStatusId = new SelectList(db.CourseStatuses.ToList(), "CourseStatusId", "Name", course.CourseStatusId);

            return View(course);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course model)
        {
            //Check if the input data fits the models validation rules
            if (ModelState.IsValid)
            {
                //Find the currently logged in users id and put it as the owner of the new course
                model.AppUserId = HttpContext.User.Identity.GetUserId();
                db.Courses.Add(model);//Add the new course to the DBContext
                db.SaveChanges();//Save the new ourse

                //Redirect the user to timed mesage page
                ViewBag.targetcontroller = "Account";
                ViewBag.targetaction = "";
                ViewBag.msg = "Kurset er oprettet";
                return View("_RedirectTo");
            }
            //Get the status possiblities if theres a field that wasn't given a valid value
            ViewBag.CourseStatusId = new SelectList(db.CourseStatuses.ToList(), "CourseStatusId", "Name", model.CourseStatusId);
            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            //Check if an id is even given or return error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Find the course based on the given id
            Course course = db.Courses
                .Where(c => c.CourseId == id)
                .Single();

            //Get the list of status possbilities
            ViewBag.CourseStatusId = new SelectList(db.CourseStatuses.ToList(), "CourseStatusId", "Name", course.CourseStatusId);

            //Check that there is a course and that the user is owner or admin
            if (course != null && (course.AppUserId == HttpContext.User.Identity.GetUserId() || HttpContext.User.IsInRole("Administrators")))
            {
                //Let the user start editing
                return View(course);
                
            }
            else
            {
                //Return error
                return HttpNotFound();
            }    
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id)
        {
            //Get the course by id with the owner users data and send it off to the view
            var courseToUpdate = db.Courses.Include(c => c.AppUser).Where(c => c.CourseId == id).Single();
            ViewBag.CourseStatusId = new SelectList(db.CourseStatuses.ToList(), "CourseStatusId", "Name", courseToUpdate.CourseStatusId);
            //Check that there is a course and that the user is owner or admin
            if (courseToUpdate != null && (courseToUpdate.AppUserId == HttpContext.User.Identity.GetUserId() || HttpContext.User.IsInRole("Administrators")))
            {   
                //Try to save the new data
                if (TryUpdateModel(courseToUpdate, "", new string[] { "Title", "Description", "Experience", "CourseStatusId", "AppUserId" }))
                {
                    try
                    {   //try to save the new course data
                        db.SaveChanges();
                    }
                    catch
                    {
                        //Return error if error is thrown
                        ViewBag.CourseStatusId = new SelectList(db.CourseStatuses.ToList(), "CourseStatusId", "Name", courseToUpdate.CourseStatusId);
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see administrator.");
                        return View(courseToUpdate);
                    }
                }
            }
            return Redirect("/Account");
        }

        public ActionResult Delete(int id)
        {
            //Find the course by id
            Course course = db.Courses.First(c => c.CourseId == id);
            if (course != null && (course.AppUserId == HttpContext.User.Identity.GetUserId() || HttpContext.User.IsInRole("Administrators")))
            {
                //Let the editing begin if course exists
                return View(course);
            }
            else
            {
                //Else return error
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult Delete(int id, int? whyDoYouDoThisMicrosft)
        {
            //Find the course by id
            Course course = db.Courses.First(c => c.CourseId == id);
            //Check that the course exists and that the users is either admin or the owner
            if (course != null && (course.AppUserId == HttpContext.User.Identity.GetUserId() || HttpContext.User.IsInRole("Administrators")))
            {
                //Delete the course
                db.Entry(course).State = EntityState.Deleted;
                db.SaveChanges();

                //Redirect user to timed message page
                ViewBag.targetcontroller = "Account";
                ViewBag.targetaction = "";
                ViewBag.msg = "Kurset er slettet";
                return View("_RedirectTo");
            }
            else
            {
                //Return error if course isn't found with that id
                return View("Error", new string[] { "Kurses findes ikke" });
            }
        }

        [HttpPost]
        public ActionResult Join(int? id)
        {
            //Get the id of the currently authenticated user and get their data
            var userId = HttpContext.User.Identity.GetUserId();
            AppUser appUser = UserManager.FindById(userId);

            //Find the course the user wants to join by id
            Course course = db.Courses.Where(c => c.CourseId == id).First();

            //Get the list of courses the current user is aldreay in
            IEnumerable<Course> joinedCourses = db.Courses.Where(x => x.AppUsers.Any(c => c.Id == userId));
            //Check if the user is in the selected course
            var isInCourse = joinedCourses.Any(x => x.CourseId == course.CourseId);

            //Check if user is owner of course or already in course and that the course is open
            if (!isInCourse && course.AppUserId != userId && course.CourseStatusId == 1)
            {
                //Create the new relation
                //ØØH SQL INJECTION?
                db.Database.ExecuteSqlCommand("INSERT INTO AppUserCourseRelations (AppUserId, CourseId) VALUES ('" + appUser.Id + "', '" + course.CourseId + "')");
                db.SaveChanges();
            }
            //Send the user back to the page they were on
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult Leave(int? id)
        {
            //Get the id of the currently authenticated user and get their data
            var userId = HttpContext.User.Identity.GetUserId();
            AppUser appUser = UserManager.FindById(userId);

            //Find the course the user wants to join by id
            Course course = db.Courses.Where(c => c.CourseId == id).First();

            //Get the list of courses the current user is aldreay in
            IEnumerable<Course> joinedCourses = db.Courses.Where(x => x.AppUsers.Any(c => c.Id == userId));
            //Check if the user is in the selected course
            var isInCourse = joinedCourses.Any(x => x.CourseId == course.CourseId);

            //Check if user is in the course
            if (isInCourse)
            {
                //Delete the relation
                db.Database.ExecuteSqlCommand("DELETE FROM AppUserCourseRelations WHERE AppUserId='" + appUser.Id + "' AND CourseId='" + course.CourseId + "'");
                db.SaveChanges();
            }
            //Send the user back to the page they were on
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            
        }

        //Stuff i didn't write
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
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