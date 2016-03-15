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
    public class UtilityController : Controller
    {
        AppIdentityDbContext db = new AppIdentityDbContext();

        [ChildActionOnly]
        public ActionResult GenerateCoursePreview(Course course, bool JoinLeaveButton = false, bool OwnCourse = false)
        {
            //Check if the this action is being requested from a page where the user will own the courses or not and send the values to the view
            if (OwnCourse)
            {
                ViewBag.OwnCourse = true;
                ViewBag.IsPrivate = false;
            }
            else
            {
                ViewBag.OwnCourse = false;
                ViewBag.IsPrivate = course.CourseStatusId == 3 ? true : false;//Ternary top to deny showing the course if it's hidden
            }
            //Tell the view if it has to put JoinLeave button in
            ViewBag.JoinLeaveButton = JoinLeaveButton;
            //Check if user is logged in
            if (Request.IsAuthenticated)
            {
                ViewBag.LoggedIn = true;
                //Check if user created the course
                var userId = HttpContext.User.Identity.GetUserId();
                if (course.AppUserId == userId)
                {
                    ViewBag.MadeByUser = true;
                }
                else
                {
                    ViewBag.MadeByUser = false;
                    //Find a list of the courses the currently loggin user is in
                    IEnumerable<Course> joinedCourses = db.Courses.Where(x => x.AppUsers.Any(c => c.Id == userId));
                    //Check if user is in this particular course
                    var isInCourse = joinedCourses.Any(x => x.CourseId == course.CourseId);
                    if (isInCourse)
                    {
                        ViewBag.IsInCourse = true;
                    }
                    else
                    {
                        ViewBag.IsInCourse = false;
                    }
                }
            }
            else
            {
                ViewBag.LoggedIn = false;
            }

            return PartialView(course);
        }

        [ChildActionOnly]
        public ActionResult GenerateJoinLeaveCourseButton(Course course, bool OwnCourse = false)
        {
            //Check if the this action is being requested from a page where the user will own the courses or not and send the values to the view
            if (OwnCourse)
            {
                ViewBag.OwnCourse = true;
                ViewBag.IsClosed = false;
            }
            else
            {
                ViewBag.OwnCourse = false;
                ViewBag.IsClosed = course.CourseStatusId == 2 ? true : false;//Ternary top to prevent join leave button if the course is closed
            }
            
            //Check if user is logged in
            if (Request.IsAuthenticated)
            {
                ViewBag.LoggedIn = true;
                //Check if user created course
                var userId = HttpContext.User.Identity.GetUserId();
                if (course.AppUserId == userId)
                {
                    ViewBag.MadeByUser = true;
                }
                else
                {
                    ViewBag.MadeByUser = false;
                    //Find a list of the courses the currently loggin user is in
                    IEnumerable<Course> joinedCourses = db.Courses.Where(x => x.AppUsers.Any(c => c.Id == userId));
                    //Check if user is in course
                    var isInCourse = joinedCourses.Any(x => x.CourseId == course.CourseId);
                    
                    if (isInCourse)
                    {
                        ViewBag.IsInCourse = true;
                    }
                    else
                    {
                        ViewBag.IsInCourse = false;
                    }
                }
            }
            else
            {
                ViewBag.LoggedIn = false;
            }

            return PartialView(course);
        }

        [ChildActionOnly]
        public ActionResult CheckUserStatus(AppUser appUser, bool Self = true)
        {
            //Get a list of the courses the user is in
            IEnumerable<Course> joinedCourses  = db.Courses.Where(x => x.AppUsers.Any(c => c.Id == appUser.Id));
            int joinedCoursesCount = joinedCourses.Count();//Count how many courses the user is in

            //Get a list of the courses the user owns
            IEnumerable<Course> createdCourses = db.Courses.Where(c => c.AppUserId == appUser.Id);

            ViewBag.IsStudent = joinedCoursesCount > 0 ? true: false;//Decide if user is a student
            ViewBag.IsSensei  = createdCourses.Count() > 0 ? true: false;//Decide if user is a sensei

            //Tell the view how to refeer to the user of the page
            if (Self)
            {
                ViewBag.Target = "Du";
            }
            else
            {
                ViewBag.Target = appUser.UserName;
            }
            
            return PartialView();
        }
    }
}