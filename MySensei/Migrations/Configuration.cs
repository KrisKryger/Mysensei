using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MySensei.Infrastructure;
using MySensei.Models;

namespace MySensei.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MySensei.Infrastructure.AppIdentityDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AppIdentityDbContext context)
        {
            // -----------------------
            // ADMIN SEEDING
            // -----------------------
            AppUserManager userMgr = new AppUserManager(new UserStore<AppUser>(context));
            AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));
            string roleName = "Administrators";
            string userName = "Admin";
            string password = "Password";
            string email = "admin@example.com";
            string phone = "88888888";
            string street = "Sønderhøj 30";
            string zipcode = "8260";
            string city = "Viby J";
            DateTime birthDate = DateTime.Now;
            DateTime joinDate = DateTime.Now;

            if (!roleMgr.RoleExists(roleName))
            {
                roleMgr.Create(new AppRole(roleName));
            }
            AppUser user = userMgr.FindByName(userName);
            if (user == null)
            {
                userMgr.Create(new AppUser {
                    UserName = userName,
                    Email = email,
                    Street = street,
                    Zipcode = zipcode,
                    PhoneNumber = phone,
                    City = city,
                    JoinDate = joinDate,
                    BirthDate = birthDate
                }, password);
                user = userMgr.FindByName(userName);
            }
            if (!userMgr.IsInRole(user.Id, roleName))
            {
                userMgr.AddToRole(user.Id, roleName);
            }

            context.SaveChanges();

            // -----------------------
            // CoursesStatuses
            // -----------------------

            var courseStatuses = new List<CourseStatus>{
                new CourseStatus {Name = "Offentlig"},
                new CourseStatus {Name = "Lukket"},
                new CourseStatus {Name = "Privat"}
            };

            courseStatuses.ForEach(s => context.CourseStatuses.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
            // -----------------------
            // Courses
            // -----------------------

            var courses = new List<Course>{
                new Course {Title = "Lær guitar", AppUser = user, AppUserId = user.Id, Description = "Jeg kan lære dig guitar. Jeg er god til guitar. Jeg kan undervise fra kl 15-16, mandag til fredag. Og jeg skal få den har beskrivelse til at være 125 karaterer nu.", Experience = 2, CourseStatusId = 1, CreateDate = DateTime.Now},
                new Course {Title = "Lær Pileflet", AppUser = user, AppUserId = user.Id, Description = "Jeg kan lære dig Pileflet. Jeg er god til Pileflet. Jeg kan undervise fra kl 15-16, mandag til fredag. Og jeg skal få den har beskrivelse til at være 125 karaterer nu.", Experience = 4, CourseStatusId = 1, CreateDate = DateTime.Now},
                new Course {Title = "Lær at vaske op", AppUser = user, AppUserId = user.Id, Description = "Jeg kan lære dig at vaske op. Jeg er god til at vaske op. Jeg kan undervise fra kl 15-16, mandag til fredag. Og jeg skal få den har beskrivelse til at være 125 karaterer nu.", Experience = 10, CourseStatusId = 1, CreateDate = DateTime.Now},
                new Course {Title = "Lær at lave mad", AppUser = user, AppUserId = user.Id, Description = "Jeg kan lære dig at lave mad. Jeg er god til at lave mad. Jeg kan undervise fra kl 15-16, mandag til fredag. Og jeg skal få den har beskrivelse til at være 125 karaterer nu.", Experience = 30, CourseStatusId = 1, CreateDate = DateTime.Now},
                new Course {Title = "Lær at snyde", AppUser = user, AppUserId = user.Id, Description = "Jeg kan lære dig at snyde. Jeg er god til at snyde. Jeg kan undervise fra kl 15-16, mandag til fredag. Og jeg skal få den har beskrivelse til at være 125 karaterer nu.", Experience = 50, CourseStatusId = 1, CreateDate = DateTime.Now}
            };

            courses.ForEach(s => context.Courses.AddOrUpdate(p => p.Title, s));
            context.SaveChanges();

            // -----------------------
            // Stored procedure
            // -----------------------
            context.Database.ExecuteSqlCommand("IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetRandomCourses') DROP PROCEDURE[dbo].GetRandomCourses;");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].GetRandomCourses @coursesToReturn int AS SET NOCOUNT ON; SELECT TOP(@coursesToReturn) * FROM Courses WHERE CourseStatusId = 1 OR CourseStatusId = 2 ORDER BY NEWID()");
            context.SaveChanges();

        }
    }
}