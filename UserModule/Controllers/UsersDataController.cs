using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserModule.Models;


namespace UserModule.Controllers
{
    [Authorize(Roles = "Super, Admin")]
    public class UsersDataController : Controller
    {
        private UserModuleBDEntities db = new UserModuleBDEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


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

        public UsersDataController()
        {
        }

        public UsersDataController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        // GET: UsersData
        public ActionResult Index()
        {
            var users = db.Users.ToList().Select(
                x => new UsersModel
                {
                    Id = x.Id,
                    CreationDate = x.CreationDate,
                    Email = x.Email,
                    FirstName = x.FirsName,
                    LastName = x.LastName,
                    ModifiedDate = x.ModifiedDate,
                    UserName = x.UserName
                });
            return View(users);
        }


        // GET: UsersData/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: UsersData/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsersData/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,Password,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,FirsName,LastName,Locked,Disabled,CreationDate,ModifiedDate")] UsersModel users)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    var user = new User { UserName = users.Email, Email = users.Email };
                    var result = UserManager.Create(user, users.Password);


                    using (var context = new UserModuleBDEntities())
                    {
                        var h = context.Users.Find(user.Id);
                        h.CreationDate = DateTime.Now;
                        h.Id = user.Id;
                        h.AccessFailedCount = user.AccessFailedCount;
                        h.Email = user.Email;
                        h.PasswordHash = user.PasswordHash;
                        h.FirsName = users.FirstName;
                        h.LastName = users.LastName;
                        h.Disabled = false;
                        h.SecurityStamp = user.SecurityStamp;
                        h.UserName = user.UserName;
                        h.Locked = false;
                        h.LockoutEnabled = user.LockoutEnabled;
                        h.LockoutEndDateUtc = user.LockoutEndDateUtc;
                        context.SaveChanges();
                    }

                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    throw ex;
                }
                return RedirectToAction("Index");
            }

            return View(users);
        }

        // GET: UsersData/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: UsersData/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,FirsName,LastName,Locked,Disabled,CreationDate,ModifiedDate")] Users users)
        {
            if (ModelState.IsValid)
            {
                users.ModifiedDate = DateTime.Now;
                db.Entry(users).State = EntityState.Modified;
                db.Entry(users).Property(x => x.CreationDate).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(users);
        }

        // GET: UsersData/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: UsersData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Users users = db.Users.Find(id);
            db.Users.Remove(users);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
