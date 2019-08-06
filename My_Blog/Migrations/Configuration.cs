namespace My_Blog.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using My_Blog.Models;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<My_Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(My_Blog.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            #region roleManager

            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));


            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            #endregion

            #region Assign User Roles

            //create users that will occupy roles of either admin or moderator
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            if(!context.Users.Any(u => u.Email == "JasonTwichell@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "JasonTwichell@mailinator.com",
                    Email = "JasonTwichell@mailinator.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "twich"

                },"abc&123!");
            }

            if (!context.Users.Any(u => u.Email == "aboyles05@outlook.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "aboyles05@outlook.com",
                    Email = "aboyles05@outlook.com",
                    FirstName = "Aaron",
                    LastName = "Boyles",
                    DisplayName = "aBoyles"

                }, "abc&123!");
            }


            //how to assign admin and moderator

            var userId = userManager.FindByEmail("aboyles05@outlook.com").Id;
            userManager.AddToRole(userId, "Admin");

            userId = userManager.FindByEmail("JasonTwichell@mailinator.com").Id;
            userManager.AddToRole(userId, "Moderator");

            #endregion

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
