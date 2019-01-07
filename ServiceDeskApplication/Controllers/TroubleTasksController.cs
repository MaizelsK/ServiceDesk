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
using Microsoft.AspNet.Identity.EntityFramework;
using ServiceDeskApplication.Services;

namespace ServiceDeskApplication.Controllers
{
    [Authorize]
    public class TroubleTasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private TroubleTaskService troubleTaskService = new TroubleTaskService();

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

        // ------ INDEX ------

        public async Task<ActionResult> Index()
        {
            if (SignInManager.AuthenticationManager.User.IsInRole("employee"))
            {
                string currentUserId = SignInManager.AuthenticationManager.User.Identity.GetUserId<string>();
                return View(await troubleTaskService.GetIndexViewModelListAsync(db, currentUserId));
            }

            return View(await troubleTaskService.GetIndexViewModelListAsync(db));
        }

        // ------ DETAILS ------

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

            TroubleTaskIndexViewModel viewModel = troubleTaskService.GetIndexViewModel(troubleTask);
            return View(viewModel);
        }

        // ------ CREATE ------

        public ActionResult Create()
        {
            return View(new TroubleTaskCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Text, Attach")] TroubleTaskCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                TroubleTask newTask = new TroubleTask
                {
                    Id = Guid.NewGuid(),
                    GeneratedDate = DateTime.Now,
                    Status = Enum.GetName(typeof(TroubleTaskStatus), 0),
                    Text = viewModel.Text,
                };
                if (viewModel.Attach != null)
                {
                    newTask.AttachedFile = FileHelper.GetAttachedFile(viewModel.Attach);
                    newTask.IsFileAttached = true;
                    newTask.AttachedFileName = viewModel.Attach.FileName;
                    newTask.AttachedFile.TroubleTaskId = newTask.Id.ToString();
                }
                else
                    newTask.IsFileAttached = false;

                string signedInUserId = SignInManager.AuthenticationManager.User.Identity.GetUserId<string>();
                newTask.User = await db.Users
                    .FirstOrDefaultAsync(user => user.Id == signedInUserId);

                db.TroubleTasks.Add(newTask);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        // ------ EDIT ------

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

            TroubleTaskEditViewModel editModel = troubleTaskService.GetEditViewModel(troubleTask);
            return View(editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "tech")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CreatorFullName,GeneratedDate," +
                                    "Text,Comment,Status,AssignedFullName")] TroubleTaskEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                TroubleTask taskToModify = db.TroubleTasks.FirstOrDefault(task => task.Id == viewModel.Id);
                if (taskToModify != null)
                {
                    // Меняем значения задаче
                    troubleTaskService.ModifyTroubleTask(viewModel, ref taskToModify);

                    db.Entry(taskToModify).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(viewModel);
        }

        // ------ ASSIGN ------

        [Authorize(Roles = "tech")]
        [HttpGet]
        public async Task<ActionResult> Assign(Guid? id)
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

            // Поиск пользователей из технического отдела
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            string techRoleId = roleManager.FindByName("tech").Id;
            IEnumerable<ApplicationUser> techUsers = await UserManager.Users
                                                     .Where(user => user.Roles
                                                     .Any(role => role.RoleId == techRoleId))
                                                     .ToListAsync();

            // Передача ViewBag Id Задачи и список пользователей тех. отдела
            ViewBag.TechUsers = techUsers;
            ViewBag.TaskId = troubleTask.Id;

            TroubleTaskAssignViewModel viewModel = troubleTaskService.GetAssignViewModel(troubleTask);
            return View(viewModel);
        }

        [Authorize(Roles = "tech")]
        public async Task<ActionResult> AssignConfirm(Guid? id, string assignedUserId)
        {
            TroubleTask troubleTask = await db.TroubleTasks.FindAsync(id);
            ApplicationUser assignedUser = await db.Users.FirstOrDefaultAsync(user => user.Id == assignedUserId);
            troubleTask.Assigned = assignedUser;

            db.Entry(troubleTask).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> DownloadFile(Guid? id)
        {
            TroubleTask troubleTask = await db.TroubleTasks.Include(x => x.AttachedFile)
                                              .FirstOrDefaultAsync(x => x.Id == id);

            if (troubleTask != null)
                return File(troubleTask.AttachedFile.Data, troubleTask.AttachedFile.ContentType,
                            troubleTask.AttachedFileName);
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
