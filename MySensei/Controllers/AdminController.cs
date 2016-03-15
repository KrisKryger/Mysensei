using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using MySensei.Infrastructure;
using MySensei.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data.Entity;

namespace MySensei.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdminController : Controller
    {
        //Init db.context
        AppIdentityDbContext db = new AppIdentityDbContext();

        public ActionResult Index()
        {
            //Get all user roles
            var Roles = RoleManager.Roles;
            ViewBag.RolesCount = Roles.Count();
            ViewBag.Roles = Roles;

            //Get all Courses
            IEnumerable <Course> Courses = db.Courses;
            ViewBag.CoursesCount = Courses.Count();
            ViewBag.Courses = Courses;

            //Send all Users to view
            return View(UserManager.Users);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateModel model)
        {
            //Check that the given data is correct
            if (ModelState.IsValid)
            {
                //Create new object of user
                AppUser user = new AppUser {
                    PhoneNumber = model.PhoneNumber,
                    Street = model.Street,
                    Zipcode = model.Zipcode,
                    City = model.City,
                    BirthDate = model.BirthDate,
                    UserName = model.UserName,
                    Email = model.Email,
                    JoinDate = DateTime.Now
                };
                //Insert new user into DB via EF
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                //If success, then redirect to timed message page for feedback
                 if (result.Succeeded)
                {
                    ViewBag.targetcontroller = "Admin";
                    ViewBag.targetaction = "";
                    ViewBag.msg = "Brugeren er oprettet";
                    return View("_RedirectTo");
                }
                else//Else rturn error
                {
                    AddErrorsFromResult(result);
                }
            }
            //If data is not correct, send data back to view for another try
            return View(model);
        }

        public async Task<ActionResult> Edit(string id)
        {
            //Get user data
            AppUser user = await UserManager.FindByIdAsync(id);

            //If the user exist let the editing begin
            if (user != null)
            {
                return View(user);
            }
            else//Return error if user doesnt exist
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, string phoneNumber, string street, string zipcode, string city, string username, string password)
        {
            //Find the user by id
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)//if the user exists
            {
                //Apply all new data to user objekt
                user.PhoneNumber = phoneNumber;
                user.Street = street;
                user.Zipcode = zipcode;
                user.City = city;
                user.UserName = username;

                IdentityResult validPass = null;
                if (password != string.Empty)
                {
                    //Verify that password follows all rules
                    validPass = await UserManager.PasswordValidator.ValidateAsync(password);
                    if (validPass.Succeeded)
                    {
                        //Add password hash
                        user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
                    }
                    else
                    {
                        //Return error if password is good enough
                        AddErrorsFromResult(validPass);
                    }
                }

                //Check if everything succeded
                if ((validPass == null) || (password != string.Empty && validPass.Succeeded))
                {
                    //Save new user data
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        //if yes: Send user to timed message page
                        ViewBag.targetcontroller = "Admin";
                        ViewBag.targetaction = "";
                        ViewBag.msg = "De nye infomationer er gemt";
                        return View("_RedirectTo");
                    }
                    else
                    {
                        //if no: return error
                        AddErrorsFromResult(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User Not Found");
                }
            }
            return View(user);
        }

        public ActionResult Delete(string id)
        {
            //Find user by id
            AppUser user = UserManager.FindById(id);
            if (user != null) {
                //if user exists, show confirmation page
                return View(user);
            }
            else
            {
                //If not return error
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, int? whyDoYouDoThisMicrosft)
        {
            //Find user by id
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)//start the delete process if the user exists
            {
                //Find courses the user owns
                var Courses = db.Courses.Where(c => c.AppUserId == id);
                //Delete those course
                foreach (Course course in Courses) {
                    db.Entry(course).State = EntityState.Deleted;
                }
                db.SaveChanges();
                //Delete the user
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)//If the user get deleted
                {
                    //Send user to timed redirect page with message
                    ViewBag.targetcontroller = "Admin";
                    ViewBag.targetaction = "";
                    ViewBag.msg = "Brugeren er slettet";
                    return View("_RedirectTo");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "User Not Found" });
            }
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

        //Stuff i didn't write
        private AppRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();
            }
        }
    }
}