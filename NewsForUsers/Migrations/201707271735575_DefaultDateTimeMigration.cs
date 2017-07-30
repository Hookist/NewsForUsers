namespace NewsForUsers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultDateTimeMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.User", "LastVisit", c => c.DateTime(nullable: false, defaultValue: DateTime.Now));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.User", "LastVisit", c => c.DateTime(nullable: false));
        }
    }
}
