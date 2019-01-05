using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ServiceDeskApplication.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using ServiceDeskApplication.Enums;

namespace ServiceDeskApplication.Controllers
{
    public class TroubleTasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

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

        // GET: TroubleTasks
        public async Task<ActionResult> Index()
        {
            if (SignInManager.AuthenticationManager.User.IsInRole("employee"))
            {
                string currentUserId = SignInManager.AuthenticationManager.User.Identity.GetUserId<string>();
                return View(await db.TroubleTasks.Where(task => task.User.Id == currentUserId).ToListAsync());
            }
            return View(await db.TroubleTasks.ToListAsync());
        }

        // GET: TroubleTasks/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TroubleTask troubleTask = await db.TroubleTasks.FindAsync(id);
            if (troubleTask == null)
            {
                return HttpNotFound();
            }
            return View(troubleTask);
        }

        // GET: TroubleTasks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TroubleTasks/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Text")] TroubleTask troubleTask)
        {
            if (ModelState.IsValid)
            {
                troubleTask.Id = Guid.NewGuid();
                troubleTask.GeneratedDate = DateTime.Now;
                troubleTask.Status = Enum.GetName(typeof(TroubleTaskStatus), 0);
                troubleTask.User = await UserManager.FindByIdAsync(SignInManager.AuthenticationManager.User.Identity.GetUserId<string>());

                db.TroubleTasks.Add(troubleTask);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(troubleTask);
        }

        // GET: TroubleTasks/Edit/5
        [Authorize(Roles = "tech")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TroubleTask troubleTask = await db.TroubleTasks.FindAsync(id);
            if (troubleTask == null)
            {
                return HttpNotFound();
            }
            return View(troubleTask);
        }

        // POST: TroubleTasks/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "tech")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,GeneratedDate,Text,Comment,Status")] TroubleTask troubleTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(troubleTask).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(troubleTask);
        }

        [Authorize(Roles = "tech")]
        [HttpGet]
        public async Task<ActionResult> Assign(Guid? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TroubleTask troubleTask = await db.TroubleTasks.FindAsync(id);
            if(troubleTask == null)
            {
                return HttpNotFound();
            }

            ViewBag.TechUsers = await _userManager.Users.Where(user => _userManager.IsInRole(user.Id, "tech")).ToListAsync();
            return View(troubleTask);
        }

        [Authorize(Roles = "tech")]
        [HttpPost, ActionName("Assign")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Assign(Guid? id, string assignedUserId)
        {
            TroubleTask troubleTask = await db.TroubleTasks.FindAsync(id);
            ApplicationUser assignedUser = await _userManager.FindByIdAsync(assignedUserId);
            troubleTask.Assigned = assignedUser;

            await db.SaveChangesAsync();
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
