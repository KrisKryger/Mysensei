using System.Threading.Tasks;
using System.Web.Mvc;
using MySensei.Models;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MySensei.Infrastructure;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net;
using System.Data.SqlClient;

namespace MySensei.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //Init db.context
        AppIdentityDbContext db = new AppIdentityDbContext();

        public ActionResult Index()
        {
            //Get currently logged in user
            var userId = HttpContext.User.Identity.GetUserId();
            AppUser appUser = UserManager.FindById(userId);
            ViewBag.User = appUser;

            //Find Courses this user is participating in
            IEnumerable<Course> joinedCourses = db.Courses.Where(x => x.AppUsers.Any(c => c.Id == userId));
            ViewBag.JoinedCoursesCount = joinedCourses.Count();//Count how many courses is returned because for some reason this can't be done in view.....
            ViewBag.JoinedCourses = joinedCourses;

            //Find Courses was created by this user
            var Courses = db.Courses.Where(c => c.AppUserId == userId);

            return View(Courses);
        }

        [AllowAnonymous]
        public ActionResult Show(string id)
        {
            //Check if an id is even passed
            if (id == null)
            {
                //Return error if no id
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Redeclaration for code clarity
            var userId = id;
            //Check that the currently logged in user is accessing thier own public page
            if (userId != HttpContext.User.Identity.GetUserId())
            {
                //Find the user in question
                AppUser appUser = UserManager.FindById(userId);
                ViewBag.User = UserManager.FindById(userId);

                //Get Courses this user is participating in
                IEnumerable<Course> joinedCourses = db.Courses.Where(x => x.AppUsers.Any(c => c.Id == userId));
                ViewBag.joinedCoursesCount = joinedCourses.Count();//Count how many courses is returned because for some reason this can't be done in view.....
                ViewBag.JoinedCourses = joinedCourses;

                //Find Courses was created by this user
                var Courses = db.Courses.Where(c => c.AppUserId == userId);

                return View(Courses);
            }
            else
            {
                //Redirect if loggid in users own public profile 
                return Redirect("/Account");
            }
        }
        

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //Check if user is already logged in and redirect to profile page if true
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/Account");
            }

            //Url to send user back to based on what the unauthenticated user wanted to accsess
            ViewBag.returnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //Find user based on email 
                AppUser user = await UserManager.FindByNameOrEmailAsync(details.Email, details.Password);

                //If user cant be found - return error
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid name or password.");
                }
                else
                {
                    //Wizardry i dont understand and didn't write, but somehow logs user out
                    ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user,
                    DefaultAuthenticationTypes.ApplicationCookie);
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, ident);
                    if (returnUrl == "")
                    {
                        return Redirect("/Account");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }

            //Url to send user back to based on what the unauthenticated user wanted to accsess
            ViewBag.returnUrl = returnUrl;

            return View(details);
        }

        [Authorize]
        public ActionResult Logout()
        {
            //Log user out
            AuthManager.SignOut();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult SignUp()
        {
            //Check if user is already logged in and redirect to profile page if true
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/Account");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUp(CreateModel newUser, string termsAccepted)
        {
            //Check if user is already logged in and redirect to profile page if true
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/Account");
            }
            if(termsAccepted != "true")
            {
                ModelState.AddModelError("termsAccepted", "Du skal acceptere betingelserne for at oprette en bruger");
                return View(newUser);
            }
            //Check if user typed in all and correct data
            if (ModelState.IsValid)
            {
                //Create new object of user
                AppUser user = new AppUser {
                    UserName = newUser.UserName,
                    Email = newUser.Email,
                    Street = newUser.Street,
                    Zipcode = newUser.Zipcode,
                    City = newUser.City,
                    JoinDate = DateTime.Now,
                    BirthDate = newUser.BirthDate,
                    PhoneNumber = newUser.PhoneNumber
                };
                //Insert new user into DB via EF
                IdentityResult result = await UserManager.CreateAsync(user, newUser.Password);

                //If success, then redirect to timed message page for feedback
                if (result.Succeeded)
                {
                    //Wizardry i dont understand and didn't write, but somehow logs user out
                    ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user,
                    DefaultAuthenticationTypes.ApplicationCookie);
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, ident);
                    ViewBag.targetcontroller = "Account";
                    ViewBag.targetaction = "";
                    ViewBag.msg = "Din bruger er oprettet";
                    return View("_RedirectTo");
                }
                else//Else rturn error
                {
                    AddErrorsFromResult(result);
                }
            }
            //If data is not correct, send data back to view for another try
            return View(newUser);
            
        }

        public async Task<ActionResult> Edit()
        {
            //Get id of current user and get all data
            var userId = HttpContext.User.Identity.GetUserId();
            AppUser user = await UserManager.FindByIdAsync(userId);

            //If the logged in user somehow doesn't exist, return error, otherwise, let the user edit information
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, string phoneNumber, string description, string street, string zipcode, string city, string userName, string password, string repeatPassword, DateTime? birthDate)
        {
            //Make sure the user that is being edited is the one thats logged in
            if (id == HttpContext.User.Identity.GetUserId())
            {
                //Find the users data
                AppUser user = await UserManager.FindByIdAsync(id);
                if (user != null)
                {
                    //Apply all new data to user objekt
                    user.PhoneNumber = phoneNumber;
                    user.Description = description;
                    user.BirthDate = birthDate;
                    user.Street = street;
                    user.Zipcode = zipcode;
                    user.City = city;
                    user.UserName = userName;
                    
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
                    //Check that password and repeat password fields match
                    bool passwordMatch;
                    if (password != repeatPassword) {
                        ModelState.AddModelError("", "Password og Gentag Password skal være ens.");
                        passwordMatch = false;
                    }
                    else
                    {
                        passwordMatch = true;
                    }

                    //Check if everything succeded
                    if ((validPass == null) || (password != string.Empty && validPass.Succeeded && passwordMatch))
                    {
                        //Save new user data
                        IdentityResult result = await UserManager.UpdateAsync(user);
                        //Check that the new data is saved
                        if (result.Succeeded)
                        {
                            //if yes: Send user to timed message page
                            ViewBag.targetcontroller = "Account";
                            ViewBag.targetaction = "";
                            ViewBag.msg = "Dine nye oplysninger er gemt";
                            return View("_RedirectTo");
                        }
                        else
                        {
                            //if no: return error
                            AddErrorsFromResult(result);
                        }
                    }

                }
                return View(user);
            }
            else
            {
                return Redirect("/");
            }
            
        }

        //stuff i didn't write
        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //stuff i didn't write
        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        //stuff i didn't write
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}