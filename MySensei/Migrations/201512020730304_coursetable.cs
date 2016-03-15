namespace MySensei.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class coursetable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Courses", "AppUserId", "dbo.AspNetUsers");
            CreateTable(
                "dbo.AppUserCourseRelations",
                c => new
                    {
                        AppUserId = c.String(nullable: false, maxLength: 128),
                        CourseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AppUserId, t.CourseId })
                .ForeignKey("dbo.AspNetUsers", t => t.AppUserId, cascadeDelete: true)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.AppUserId)
                .Index(t => t.CourseId);
            
            AlterColumn("dbo.Courses", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.Courses", "Description", c => c.String(maxLength: 2048));
            AlterColumn("dbo.CourseStatus", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Tags", "Name", c => c.String(nullable: false));
            AddForeignKey("dbo.Courses", "AppUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "AppUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AppUserCourseRelations", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.AppUserCourseRelations", "AppUserId", "dbo.AspNetUsers");
            DropIndex("dbo.AppUserCourseRelations", new[] { "CourseId" });
            DropIndex("dbo.AppUserCourseRelations", new[] { "AppUserId" });
            AlterColumn("dbo.Tags", "Name", c => c.String());
            AlterColumn("dbo.CourseStatus", "Name", c => c.String());
            AlterColumn("dbo.Courses", "Description", c => c.String());
            AlterColumn("dbo.Courses", "Title", c => c.String());
            DropTable("dbo.AppUserCourseRelations");
            AddForeignKey("dbo.Courses", "AppUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
