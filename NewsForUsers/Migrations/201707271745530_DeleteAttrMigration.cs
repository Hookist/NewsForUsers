namespace NewsForUsers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteAttrMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.User", "LastVisit", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.User", "LastVisit", c => c.DateTime(nullable: false));
        }
    }
}
