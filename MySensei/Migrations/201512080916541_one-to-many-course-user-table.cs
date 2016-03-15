namespace MySensei.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onetomanycourseusertable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Courses", "AppUserId", c => c.String(nullable: false, maxLength: 128));
        }

        public override void Down()
        {
        }
    }
}
