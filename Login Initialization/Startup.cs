using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using ScheduleUsers.Models;

[assembly: OwinStartupAttribute(typeof(ScheduleUsers.Startup))]
namespace ScheduleUsers
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
        }

        //Creating default users and admin users to be logged in
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            //Creating a default User and Admin role
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                {
                    Name = "Admin"
                };
                roleManager.Create(role);
            }

            try //or??
            {
                var user = new ApplicationUser
                {
                    FirstName = "Adam",
                    LastName = "Jones",
                    HireDate = System.DateTime.Today,
                    Department = "Development",
                    Position = "Software Developer Manager",
                    Address = "321 Portland Ct",
                    HourlyPayRate = 40,
                    Fulltime = true,
                    Email = "adam@gmail.com",
                    PhoneNumber = "808-231-2455",
                    UserName = "adam@gmail.com",

                };

                string userPassword = "Password1!";

                var chkUser = UserManager.Create(user, userPassword);

                //Adding default user to admin role
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }
            }
            catch (System.Exception)
            {
                
            }    
                   
            //Creating User Role
            if (!roleManager.RoleExists("User"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                {
                    Name = "User"
                };
                roleManager.Create(role);
            }
            try //or??
            {
                var user = new ApplicationUser
                {
                    FirstName = "Alex",
                    LastName = "Smith",
                    HireDate = System.DateTime.Today,
                    Department = "Develoment",
                    Position = "Software Development",
                    Address = "3123 Portland Ct",
                    HourlyPayRate = 20,
                    Fulltime = true,
                    Email = "alex@gmail.com",
                    PhoneNumber = "123-231-2255",
                    UserName = "alex@gmail.com"
                };



                string userPassword = "Password1!";
                var chkUser = UserManager.Create(user, userPassword);

                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "User");
                }
            }
            catch (System.Exception)
            {

            }

        }
    }
}

