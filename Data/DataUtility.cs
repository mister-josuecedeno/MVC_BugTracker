using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVC_BugTracker.Models;
using MVC_BugTracker.Models.Enums;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Data
{
    //The static keyword is used so that we don't have to create an instance of the class
    //in order to use methods within the class

    public static class DataUtility
    {
        //Get company Ids
        private static int company1Id;
        private static int company2Id;
        private static int company3Id;
        private static int company4Id;
        private static int company5Id;
        private static int company6Id;
        private static int company7Id;


        public static string GetConnectionString(IConfiguration configuration)
        {
            //The default connection string will come from appSettings like usual
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            //It will be automatically overwritten if we are running on Heroku
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        public static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            //Provides a simple way to create and manage the contents of connection strings used by the NpgsqlConnection class.
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task ManageDataAsync(IHost host)
        {
            using var svcScope = host.Services.CreateScope();
            var svcProvider = svcScope.ServiceProvider;
            //Service: An instance of RoleManager
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            //Service: An instance of RoleManager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //Service: An instance of the UserManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BTUser>>();
            //TsTEP 1: This is the programmatic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();

            //Custom  Bug Tracker Seed Methods
            await SeedRolesAsync(userManagerSvc, roleManagerSvc);
            await SeedDefaultCompaniesAsync(dbContextSvc);
            await SeedDefaultUsersAsync(userManagerSvc, roleManagerSvc);
            await SeedDemoUsersAsync(userManagerSvc, roleManagerSvc);
            await SeedDefaultTicketTypeAsync(dbContextSvc);
            await SeedDefaultTicketStatusAsync(dbContextSvc);
            await SeedDefaultTicketPriorityAsync(dbContextSvc);
            await SeedDefaultProjectPriorityAsync(dbContextSvc);
            await SeedDefautProjectsAsync(dbContextSvc);
            await SeedDefautTicketsAsync(dbContextSvc);
        }

        public static async Task SeedRolesAsync(UserManager<BTUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Developer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Submitter.ToString()));
            //await roleManager.CreateAsync(new IdentityRole(Roles.NewUser.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.DemoUser.ToString()));
        }

        public static async Task SeedDefaultCompaniesAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Company> defaultcompanies = new List<Company>() {
                    new Company() { Name = "Demo Company", Description="This is a sample company for demonstration purposes" },
                    new Company() { Name = "Company2", Description="This is default Company 2" },
                    new Company() { Name = "MVC Bug Tracker", Description="Manage your team projects with the power of .NET Core." },
                    new Company() { Name = "Company4", Description="This is default Company 4" },
                    new Company() { Name = "Company5", Description="This is default Company 5" },
                    new Company() { Name = "Company6", Description="This is default Company 6" },
                    new Company() { Name = "Company7", Description="This is default Company 7" }
                };

                var dbCompanies = context.Company.Select(c => c.Name).ToList();
                await context.Company.AddRangeAsync(defaultcompanies.Where(c => !dbCompanies.Contains(c.Name)));
                context.SaveChanges();

                //Get company Ids
                company1Id = context.Company.FirstOrDefault(p => p.Name == "Demo Company").Id;
                company2Id = context.Company.FirstOrDefault(p => p.Name == "Company2").Id;
                company3Id = context.Company.FirstOrDefault(p => p.Name == "MVC Bug Tracker").Id;
                company4Id = context.Company.FirstOrDefault(p => p.Name == "Company4").Id;
                company5Id = context.Company.FirstOrDefault(p => p.Name == "Company5").Id;
                company6Id = context.Company.FirstOrDefault(p => p.Name == "Company6").Id;
                company7Id = context.Company.FirstOrDefault(p => p.Name == "Company7").Id;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Companies.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultProjectPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<ProjectPriority> projectPriorities = new List<ProjectPriority>() {
                                                    new ProjectPriority() { Name = "Low" },
                                                    new ProjectPriority() { Name = "Medium" },
                                                    new ProjectPriority() { Name = "High" },
                                                    new ProjectPriority() { Name = "Urgent" },
                };

                var dbProjectPriorities = context.ProjectPriority.Select(c => c.Name).ToList();
                await context.ProjectPriority.AddRangeAsync(projectPriorities.Where(c => !dbProjectPriorities.Contains(c.Name)));
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Project Priorities.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefautProjectsAsync(ApplicationDbContext context)
        {

            //Get project priority Ids
            int priorityLow = context.ProjectPriority.FirstOrDefault(p => p.Name == "Low").Id;
            int priorityMedium = context.ProjectPriority.FirstOrDefault(p => p.Name == "Medium").Id;
            int priorityHigh = context.ProjectPriority.FirstOrDefault(p => p.Name == "High").Id;
            int priorityUrgent = context.ProjectPriority.FirstOrDefault(p => p.Name == "Urgent").Id;

            try
            {
                IList<Project> projects = new List<Project>() {
                     // DEMO Projects
                     new Project()
                     {
                         CompanyId = company1Id,
                         Name = "[DEMO] Build a Personal Porfolio",
                         Description="Single page html, css & javascript page.  Serves as a landing page for candidates and contains a bio and links to all applications and challenges." ,
                         StartDate = new DateTime(2021,6,1),
                         EndDate = new DateTime(2021,6,1).AddMonths(3),
                         ProjectPriorityId = priorityLow
                     },
                     new Project()
                     {
                         CompanyId = company1Id,
                         Name = "[DEMO] Build a supplemental Blog Web Application",
                         Description="Candidate's custom built web application using .Net Core with MVC, a postgres database and hosted in a heroku container.  The app is designed for the candidate to create, update and maintain a live blog site.",
                         StartDate = new DateTime(2021,6,1),
                         EndDate = new DateTime(2021,6,1).AddMonths(3),
                         ProjectPriorityId = priorityMedium
                     },
                     new Project()
                     {
                         CompanyId = company1Id,
                         Name = "[DEMO] Build an Issue Tracking Web Application",
                         Description="A custom designed .Net Core application with postgres database.  The application is a multi tennent application designed to track issue tickets' progress.  Implemented with identity and user roles, Tickets are maintained in projects which are maintained by users in the role of projectmanager.  Each project has a team and team members.",
                         StartDate = new DateTime(2021,6,1),
                         EndDate = new DateTime(2021,6,1).AddMonths(3),
                         ProjectPriorityId = priorityHigh
                     },
                    new Project()
                     {
                         CompanyId = company1Id,
                         Name = "[DEMO] Build a Movie Information Web Application",
                         Description="A custom designed .Net Core application with postgres database.  An API based application allows users to input and import movie posters and details including cast and crew information.",
                         StartDate = new DateTime(2021,6,1),
                         EndDate = new DateTime(2021,6,1).AddMonths(3),
                         ProjectPriorityId = priorityHigh
                     },
                     new Project()
                     {
                         CompanyId = company1Id,
                         Name = "[DEMO] Build an Address Book Web Application",
                         Description="A custom designed .Net Core application with postgres database.  This is an application to serve as a rolodex of contacts for a given user..",
                         StartDate = new DateTime(2021,6,1),
                         EndDate = new DateTime(2021,6,1).AddMonths(3),
                         ProjectPriorityId = priorityHigh
                     },

                     // More Projects

                     new Project()
                     {
                         CompanyId = company3Id,
                         Name = "Build a Personal Porfolio",
                         Description="Single page html, css & javascript page.  Serves as a landing page for candidates and contains a bio and links to all applications and challenges." ,
                         StartDate = new DateTime(2021,6,1),
                         EndDate = new DateTime(2021,6,1).AddMonths(3),
                         ProjectPriorityId = priorityLow
                     },
                     new Project()
                     {
                         CompanyId = company3Id,
                         Name = "Build a supplemental Blog Web Application",
                         Description="Candidate's custom built web application using .Net Core with MVC, a postgres database and hosted in a heroku container.  The app is designed for the candidate to create, update and maintain a live blog site.",
                         StartDate = new DateTime(2021,6,1),
                         EndDate = new DateTime(2021,6,1).AddMonths(3),
                         ProjectPriorityId = priorityMedium
                     },
                     new Project()
                     {
                         CompanyId = company3Id,
                         Name = "Build an Issue Tracking Web Application",
                         Description="A custom designed .Net Core application with postgres database.  The application is a multi tennent application designed to track issue tickets' progress.  Implemented with identity and user roles, Tickets are maintained in projects which are maintained by users in the role of projectmanager.  Each project has a team and team members.",
                         StartDate = new DateTime(2021,6,1),
                         EndDate = new DateTime(2021,6,1).AddMonths(3),
                         ProjectPriorityId = priorityHigh
                     },
                    new Project()
                     {
                         CompanyId = company3Id,
                         Name = "Build a Movie Information Web Application",
                         Description="A custom designed .Net Core application with postgres database.  An API based application allows users to input and import movie posters and details including cast and crew information.",
                         StartDate = new DateTime(2021,6,1),
                         EndDate = new DateTime(2021,6,1).AddMonths(3),
                         ProjectPriorityId = priorityHigh
                     },
                     new Project()
                     {
                         CompanyId = company3Id,
                         Name = "Build an Address Book Web Application",
                         Description="A custom designed .Net Core application with postgres database.  This is an application to serve as a rolodex of contacts for a given user..",
                         StartDate = new DateTime(2021,6,1),
                         EndDate = new DateTime(2021,6,1).AddMonths(3),
                         ProjectPriorityId = priorityHigh
                     }
                };

                var dbProjects = context.Project.Select(c => c.Name).ToList();
                await context.Project.AddRangeAsync(projects.Where(c => !dbProjects.Contains(c.Name)));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Projects.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultUsersAsync(UserManager<BTUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default Admin User
            var defaultUser = new BTUser
            {
                UserName = "josuecedeno@gmail.com",
                Email = "josuecedeno@gmail.com",
                FirstName = "Josue",
                LastName = "Cedeno",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default ProjectManager1 User
            defaultUser = new BTUser
            {
                UserName = "hmccoy@mailinator.com",
                Email = "hmccoy@mailinator.com",
                FirstName = "Henry",
                LastName = "McCoy",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default ProjectManager1 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default ProjectManager2 User
            defaultUser = new BTUser
            {
                UserName = "pquill@mailinator.com",
                Email = "pquill@mailinator.com",
                FirstName = "Peter",
                LastName = "Quill",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default ProjectManager2 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer1 User
            defaultUser = new BTUser
            {
                UserName = "srogers@mailinator.com",
                Email = "srogers@mailinator.com",
                FirstName = "Steve",
                LastName = "Rogers",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Developer1 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer2 User
            defaultUser = new BTUser
            {
                UserName = "jhowlett@mailinator.com",
                Email = "jhowlett@mailinator.com",
                FirstName = "James",
                LastName = "Howlett",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Developer2 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer3 User
            defaultUser = new BTUser
            {
                UserName = "nromanova@mailinator.com",
                Email = "nromanova@mailinator.com",
                FirstName = "Natasha",
                LastName = "Romanova",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Developer3 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer4 User
            defaultUser = new BTUser
            {
                UserName = "cdanvers@mailinator.com",
                Email = "cdanvers@mailinator.com",
                FirstName = "Carol",
                LastName = "Danvers",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Developer4 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer5 User
            defaultUser = new BTUser
            {
                UserName = "tstark@mailinator.com",
                Email = "tstark@mailinator.com",
                FirstName = "Tony",
                LastName = "Stark",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Developer5 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Submitter1 User
            defaultUser = new BTUser
            {
                UserName = "ssummers@mailinator.com",
                Email = "ssummers@mailinator.com",
                FirstName = "Scott",
                LastName = "Summers",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Submitter1 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Default Submitter2 User
            defaultUser = new BTUser
            {
                UserName = "sstorm@mailinator.com",
                Email = "sstorm@mailinator.com",
                FirstName = "Sue",
                LastName = "Storm",
                EmailConfirmed = true,
                CompanyId = company3Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Submitter2 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }

        }

        public static async Task SeedDemoUsersAsync(UserManager<BTUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Demo Admin User
            var defaultUser = new BTUser
            {
                UserName = "demoadmin@mailinator.com",
                Email = "demoadmin@mailinator.com",
                FirstName = "Demo",
                LastName = "Admin",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Demo ProjectManager User
            defaultUser = new BTUser
            {
                UserName = "demopm@mailinator.com",
                Email = "demopm@mailinator.com",
                FirstName = "Demo",
                LastName = "ProjectManager",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo ProjectManager1 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Demo Developer User
            defaultUser = new BTUser
            {
                UserName = "demodev@mailinator.com",
                Email = "demodev@mailinator.com",
                FirstName = "Demo",
                LastName = "Developer",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo Developer1 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Demo Submitter User
            defaultUser = new BTUser
            {
                UserName = "demosub@mailinator.com",
                Email = "demosub@mailinator.com",
                FirstName = "Demo",
                LastName = "Submitter",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo Submitter User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


            //Seed Demo New User
            defaultUser = new BTUser
            {
                UserName = "demonew@mailinator.com",
                Email = "demonew@mailinator.com",
                FirstName = "Demo",
                LastName = "NewUser",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo Submitter User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultTicketTypeAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketType> ticketTypes = new List<TicketType>() {
                     new TicketType() { Name = "New Development" },
                     new TicketType() { Name = "Runtime" },
                     new TicketType() { Name = "UI" },
                     new TicketType() { Name = "Maintenance" },
                };

                var dbTicketTypes = context.TicketType.Select(c => c.Name).ToList();
                await context.TicketType.AddRangeAsync(ticketTypes.Where(c => !dbTicketTypes.Contains(c.Name)));
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Ticket Types.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultTicketStatusAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketStatus> ticketStatuses = new List<TicketStatus>() {
                    new TicketStatus() { Name = "New" },                 // Newly Created ticket having never been assigned
                    new TicketStatus() { Name = "Unassigned" },          // Ticket has been assigned at some point but is currently unassigned
                    new TicketStatus() { Name = "Development" },         // Ticket is assigned and currently being worked 
                    new TicketStatus() { Name = "Testing" },             // Ticket is assigned and is currently being tested
                    new TicketStatus() { Name = "Resolved" },           // Ticket remains assigned to the developer but work in now complete
                    new TicketStatus() { Name = "Archived" }            // Ticket remains assigned to the developer but becomes inactive
                };

                var dbTicketStatuses = context.TicketStatus.Select(c => c.Name).ToList();
                await context.TicketStatus.AddRangeAsync(ticketStatuses.Where(c => !dbTicketStatuses.Contains(c.Name)));
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Ticket Statuses.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultTicketPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketPriority> ticketPriorities = new List<TicketPriority>() {
                                                    new TicketPriority() { Name = "Low" },
                                                    new TicketPriority() { Name = "Medium" },
                                                    new TicketPriority() { Name = "High" },
                                                    new TicketPriority() { Name = "Urgent" },
                };

                var dbTicketPriorities = context.TicketPriority.Select(c => c.Name).ToList();
                await context.TicketPriority.AddRangeAsync(ticketPriorities.Where(c => !dbTicketPriorities.Contains(c.Name)));
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Ticket Priorities.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefautTicketsAsync(ApplicationDbContext context)
        {
            //Get project Ids
            int portfolioId = context.Project.FirstOrDefault(p => p.Name == "Build a Personal Porfolio").Id;
            int blogId = context.Project.FirstOrDefault(p => p.Name == "Build a supplemental Blog Web Application").Id;
            int bugtrackerId = context.Project.FirstOrDefault(p => p.Name == "Build an Issue Tracking Web Application").Id;

            //Get DEMO project Ids
            int demoPortfolioId = context.Project.FirstOrDefault(p => p.Name == "[DEMO] Build a Personal Porfolio").Id;
            int demoBlogId = context.Project.FirstOrDefault(p => p.Name == "[DEMO] Build a supplemental Blog Web Application").Id;
            int demoBugtrackerId = context.Project.FirstOrDefault(p => p.Name == "[DEMO] Build an Issue Tracking Web Application").Id;

            //Get ticket type Ids
            int typeNewDev = context.TicketType.FirstOrDefault(p => p.Name == "New Development").Id;
            int typeRuntime = context.TicketType.FirstOrDefault(p => p.Name == "Runtime").Id;
            int typeUI = context.TicketType.FirstOrDefault(p => p.Name == "UI").Id;
            int typeMaintenance = context.TicketType.FirstOrDefault(p => p.Name == "Maintenance").Id;

            //Get ticket priority Ids
            int priorityLow = context.TicketPriority.FirstOrDefault(p => p.Name == "Low").Id;
            int priorityMedium = context.TicketPriority.FirstOrDefault(p => p.Name == "Medium").Id;
            int priorityHigh = context.TicketPriority.FirstOrDefault(p => p.Name == "High").Id;
            int priorityUrgent = context.TicketPriority.FirstOrDefault(p => p.Name == "Urgent").Id;

            //Get ticket status Ids
            int statusNew = context.TicketStatus.FirstOrDefault(p => p.Name == "New").Id;
            int statusUnassigned = context.TicketStatus.FirstOrDefault(p => p.Name == "Unassigned").Id;
            int statusDev = context.TicketStatus.FirstOrDefault(p => p.Name == "Development").Id;
            int statusTest = context.TicketStatus.FirstOrDefault(p => p.Name == "Testing").Id;
            int statusResolved = context.TicketStatus.FirstOrDefault(p => p.Name == "Resolved").Id;

            try
            {
                IList<Ticket> tickets = new List<Ticket>() {
                        //PORTFOLIO
                        new Ticket() {Title = "001 - Portfolio - Assign Team Members", Description = "Assign Team Members", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                        new Ticket() {Title = "002 - Portfolio - Define KPIs", Description = "Define KPIs", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityMedium, TicketStatusId = statusUnassigned, TicketTypeId = typeMaintenance},
                        new Ticket() {Title = "003 - Portfolio - Meet with Stakeholders", Description = "Meet with stakeholders", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev, TicketTypeId = typeUI},
                        new Ticket() {Title = "004 - Portfolio - Wireframe Project", Description = "Wireframe project", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityUrgent, TicketStatusId = statusTest, TicketTypeId = typeRuntime},
                        new Ticket() {Title = "005 - Portfolio - Code MVP", Description = "Code MVP", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                        //new Ticket() {Title = "Portfolio Ticket 6", Description = "Ticket details for portfolio ticket 6", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityMedium, TicketStatusId = statusUnassigned, TicketTypeId = typeMaintenance},
                        //new Ticket() {Title = "Portfolio Ticket 7", Description = "Ticket details for portfolio ticket 7", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev, TicketTypeId = typeUI},
                        //new Ticket() {Title = "Portfolio Ticket 8", Description = "Ticket details for portfolio ticket 8", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityUrgent, TicketStatusId = statusTest, TicketTypeId = typeRuntime},
                                
                        //BLOG
                        new Ticket() {Title = "001 - Blog - Assign Team Members", Description = "Assign Team Members", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusUnassigned, TicketTypeId = typeRuntime},
                        new Ticket() {Title = "002 - Blog - Define KPIs", Description = "Define KPIs", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusDev, TicketTypeId = typeUI},
                        new Ticket() {Title = "003 - Blog - Meet with Stakeholders", Description = "Meet with Stakeholders", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeMaintenance},
                        new Ticket() {Title = "004 - Blog - Wireframe Project", Description = "Wireframe Project", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        new Ticket() {Title = "005 - Blog - Code MVP", Description = "Code MVP", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusDev,  TicketTypeId = typeRuntime},
                        //new Ticket() {Title = "Blog Ticket 6", Description = "Ticket details for blog ticket 6", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusNew,  TicketTypeId = typeUI},
                        //new Ticket() {Title = "Blog Ticket 7", Description = "Ticket details for blog ticket 7", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeMaintenance},
                        //new Ticket() {Title = "Blog Ticket 8", Description = "Ticket details for blog ticket 8", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusDev,  TicketTypeId = typeNewDev},
                                
                                
                        //BUGTRACKER                                                                                                                         
                        new Ticket() {Title = "001 - Blog - Assign Team Members", Description = "Assign Team Members", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        new Ticket() {Title = "002 - Blog - Define KPIs", Description = "Define KPIs", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        new Ticket() {Title = "003 - Blog - Meet with Stakeholders", Description = "Meet with Stakeholders", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        new Ticket() {Title = "004 - Blog - Wireframe Project", Description = "Wireframe Project", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        new Ticket() {Title = "005 - Blog - Code MVP", Description = "Code MVP", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        //new Ticket() {Title = "Bug Tracker Ticket 6", Description = "Ticket details for blog ticket 6", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        //new Ticket() {Title = "Bug Tracker Ticket 7", Description = "Ticket details for blog ticket 7", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        //new Ticket() {Title = "Bug Tracker Ticket 8", Description = "Ticket details for blog ticket 8", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        
                        // DEMO Tickets

                        //PORTFOLIO
                        new Ticket() {Title = "001 - Portfolio - Assign Team Members", Description = "Assign Team Members", Created = DateTimeOffset.Now, ProjectId = demoPortfolioId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                        new Ticket() {Title = "002 - Portfolio - Define KPIs", Description = "Define KPIs", Created = DateTimeOffset.Now, ProjectId = demoPortfolioId, TicketPriorityId = priorityMedium, TicketStatusId = statusUnassigned, TicketTypeId = typeMaintenance},
                        new Ticket() {Title = "003 - Portfolio - Meet with Stakeholders", Description = "Meet with stakeholders", Created = DateTimeOffset.Now, ProjectId = demoPortfolioId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev, TicketTypeId = typeUI},
                        new Ticket() {Title = "004 - Portfolio - Wireframe Project", Description = "Wireframe project", Created = DateTimeOffset.Now, ProjectId = demoPortfolioId, TicketPriorityId = priorityUrgent, TicketStatusId = statusTest, TicketTypeId = typeRuntime},
                        new Ticket() {Title = "005 - Portfolio - Code MVP", Description = "Code MVP", Created = DateTimeOffset.Now, ProjectId = demoPortfolioId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                        //new Ticket() {Title = "Portfolio Ticket 6", Description = "Ticket details for portfolio ticket 6", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityMedium, TicketStatusId = statusUnassigned, TicketTypeId = typeMaintenance},
                        //new Ticket() {Title = "Portfolio Ticket 7", Description = "Ticket details for portfolio ticket 7", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev, TicketTypeId = typeUI},
                        //new Ticket() {Title = "Portfolio Ticket 8", Description = "Ticket details for portfolio ticket 8", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityUrgent, TicketStatusId = statusTest, TicketTypeId = typeRuntime},
                                
                        //BLOG
                        new Ticket() {Title = "001 - Blog - Assign Team Members", Description = "Assign Team Members", Created = DateTimeOffset.Now, ProjectId = demoBlogId, TicketPriorityId = priorityLow, TicketStatusId = statusUnassigned, TicketTypeId = typeRuntime},
                        new Ticket() {Title = "002 - Blog - Define KPIs", Description = "Define KPIs", Created = DateTimeOffset.Now, ProjectId = demoBlogId, TicketPriorityId = priorityMedium, TicketStatusId = statusDev, TicketTypeId = typeUI},
                        new Ticket() {Title = "003 - Blog - Meet with Stakeholders", Description = "Meet with Stakeholders", Created = DateTimeOffset.Now, ProjectId = demoBlogId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeMaintenance},
                        new Ticket() {Title = "004 - Blog - Wireframe Project", Description = "Wireframe Project", Created = DateTimeOffset.Now, ProjectId = demoBlogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        new Ticket() {Title = "005 - Blog - Code MVP", Description = "Code MVP", Created = DateTimeOffset.Now, ProjectId = demoBlogId, TicketPriorityId = priorityLow, TicketStatusId = statusDev,  TicketTypeId = typeRuntime},
                        //new Ticket() {Title = "Blog Ticket 6", Description = "Ticket details for blog ticket 6", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusNew,  TicketTypeId = typeUI},
                        //new Ticket() {Title = "Blog Ticket 7", Description = "Ticket details for blog ticket 7", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeMaintenance},
                        //new Ticket() {Title = "Blog Ticket 8", Description = "Ticket details for blog ticket 8", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusDev,  TicketTypeId = typeNewDev},
                                
                                
                        //BUGTRACKER                                                                                                                         
                        new Ticket() {Title = "001 - Blog - Assign Team Members", Description = "Assign Team Members", Created = DateTimeOffset.Now, ProjectId = demoBugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        new Ticket() {Title = "002 - Blog - Define KPIs", Description = "Define KPIs", Created = DateTimeOffset.Now, ProjectId = demoBugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        new Ticket() {Title = "003 - Blog - Meet with Stakeholders", Description = "Meet with Stakeholders", Created = DateTimeOffset.Now, ProjectId = demoBugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        new Ticket() {Title = "004 - Blog - Wireframe Project", Description = "Wireframe Project", Created = DateTimeOffset.Now, ProjectId = demoBugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        new Ticket() {Title = "005 - Blog - Code MVP", Description = "Code MVP", Created = DateTimeOffset.Now, ProjectId = demoBugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        //new Ticket() {Title = "Bug Tracker Ticket 6", Description = "Ticket details for blog ticket 6", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        //new Ticket() {Title = "Bug Tracker Ticket 7", Description = "Ticket details for blog ticket 7", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        //new Ticket() {Title = "Bug Tracker Ticket 8", Description = "Ticket details for blog ticket 8", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                        

                };


                var dbTickets = context.Ticket.Select(c => c.Title).ToList();
                await context.Ticket.AddRangeAsync(tickets.Where(c => !dbTickets.Contains(c.Title)));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Ticket.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

    }

}
