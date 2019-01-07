using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ServiceDeskApplication.Models
{
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public virtual string FullName { get; set; }

        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ServiceDeskDb", throwIfV1Schema: false)
        {
            Database.SetInitializer(new DbInitializer());
        }

        public DbSet<TroubleTask> TroubleTasks { get; set; }
        public DbSet<AttachedFile> AttachedFiles { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class DbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole("employee"),
                new IdentityRole("tech")
            };

            ApplicationUser techUser = new ApplicationUser
            {
                UserName = "domo.ddr@gmail.com",
                Email = "domo.ddr@gmail.com",
                PasswordHash = new PasswordHasher().HashPassword("admin123"),
                FullName = "Админ Админыч",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            context.Roles.Add(roles[0]);
            context.Roles.Add(roles[1]);
            context.Users.Add(techUser);
            context.SaveChanges();

            ApplicationUserManager manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            manager.AddToRole(techUser.Id, "tech");

            base.Seed(context);
        }
    }
}