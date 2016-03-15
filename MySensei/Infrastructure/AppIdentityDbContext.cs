using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using MySensei.Models;
using Microsoft.AspNet.Identity;
using System.Collections;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Text;

namespace MySensei.Infrastructure
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext() : base("IdentityDb") { }

        static AppIdentityDbContext()
        {
            Database.SetInitializer<AppIdentityDbContext>(new IdentityDbInit());
        }
        public IEnumerable AppUsers { get; internal set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseStatus> CourseStatuses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); // Identity use pluralized table names
            // one-to-many relation between Course (1) and User (N)
            modelBuilder.Entity<Course>()
                .HasRequired<AppUser>(s => s.AppUser)
                .WithMany(s => s.CreatedCourses)
                .HasForeignKey(s => s.AppUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasRequired<CourseStatus>(s => s.CourseStatus)
                .WithMany(s => s.Courses)
                .HasForeignKey(s => s.CourseStatusId).WillCascadeOnDelete(false);
            
            modelBuilder.Entity<AppUser>()
                .HasMany<Course>(s => s.Courses)
                .WithMany(c => c.AppUsers)
                .Map(cs =>
                {
                    cs.MapLeftKey("AppUserId");
                    cs.MapRightKey("CourseId");
                    cs.ToTable("AppUserCourseRelations");
                });
            
            // the all important base class call! Add this line to make your problems go away.
            base.OnModelCreating(modelBuilder);
        }

        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }

    public class IdentityDbInit : NullDatabaseInitializer<AppIdentityDbContext>
    {
    }
}