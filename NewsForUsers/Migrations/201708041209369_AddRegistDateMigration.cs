namespace NewsForUsers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRegistDateMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "RegistrationDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.ApplicationUsers", "LastVisitDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ApplicationUsers", "LastVisitDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.ApplicationUsers", "RegistrationDate");
        }
    }
}
