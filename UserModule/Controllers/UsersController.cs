using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserModule.Models;

namespace UserModule.Controllers
{
  //  [Authorize(Roles = "Super, Admin")]
    [Authorize]
    public class UsersController : Controller
    {
        #region /// PROPERTIES AND CONSTRUCTOR ///
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UsersController()
        {
        }

        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        } 
        #endregion

        // GET: Users
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;

                ViewBag.displayMenu = "No";

                if (User.IsInRole("Super") || User.IsInRole("Admin"))
                {
                    ViewBag.displayMenu = "Yes";
                }
                else {
                    //    var result = new FilePathResult("~/Views/Users/Profile.html", "text/html");
                    //return result;
                    return View("Profiles");
                }


                return View();
            }
            else
            {
                ViewBag.Name = "Not Logged IN";
            }
            return View();
        }

        public ActionResult AllUsers()
        {

            var users = UserManager.Users.ToList().Select(
                x => new UsersModel
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Password = x.PasswordHash
                });


            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = User.Identity;
            //    ViewBag.Name = user.Name;

            //    ViewBag.displayMenu = "No";

            //    if (User.IsInRole("Super") || User.IsInRole("Admin"))
            //    {
            //        ViewBag.displayMenu = "Yes";
            //    }
            //    return View(users);
            //}
            //else
            //{
            //    ViewBag.Name = "Not Logged IN";
            //}
            return View(users);
        }


        public bool isSuperUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Super"))
                    return true;
                else
                    return false;
            }
            return false;
        }

        public bool isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity; 
                 var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}