namespace ServiceDeskApplication.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using ServiceDeskApplication.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ServiceDeskApplication.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ServiceDeskApplication.Models.ApplicationDbContext context)
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
            };

            context.Roles.AddOrUpdate(roles[0]);
            context.Roles.AddOrUpdate(roles[1]);
            context.Users.AddOrUpdate(techUser);
            context.SaveChanges();

            ApplicationUserManager manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            if (!manager.IsInRole(techUser.Id, "tech"))
                manager.AddToRole(techUser.Id, "tech");

            base.Seed(context);
        }
    }
}
