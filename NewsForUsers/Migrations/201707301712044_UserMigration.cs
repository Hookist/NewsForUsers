namespace NewsForUsers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMigration : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.IdentityUsers", newName: "ApplicationUsers");
            RenameColumn(table: "dbo.IdentityUserClaims", name: "IdentityUser_Id", newName: "ApplicationUser_Id");
            RenameColumn(table: "dbo.AspNetUserLogins", name: "IdentityUser_Id", newName: "ApplicationUser_Id");
            RenameColumn(table: "dbo.AspNetUserRoles", name: "IdentityUser_Id", newName: "ApplicationUser_Id");
            RenameIndex(table: "dbo.AspNetUserRoles", name: "IX_IdentityUser_Id", newName: "IX_ApplicationUser_Id");
            RenameIndex(table: "dbo.IdentityUserClaims", name: "IX_IdentityUser_Id", newName: "IX_ApplicationUser_Id");
            RenameIndex(table: "dbo.AspNetUserLogins", name: "IX_IdentityUser_Id", newName: "IX_ApplicationUser_Id");
            AddColumn("dbo.ApplicationUsers", "LastVisitDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUsers", "LastVisitDate");
            RenameIndex(table: "dbo.AspNetUserLogins", name: "IX_ApplicationUser_Id", newName: "IX_IdentityUser_Id");
            RenameIndex(table: "dbo.IdentityUserClaims", name: "IX_ApplicationUser_Id", newName: "IX_IdentityUser_Id");
            RenameIndex(table: "dbo.AspNetUserRoles", name: "IX_ApplicationUser_Id", newName: "IX_IdentityUser_Id");
            RenameColumn(table: "dbo.AspNetUserRoles", name: "ApplicationUser_Id", newName: "IdentityUser_Id");
            RenameColumn(table: "dbo.AspNetUserLogins", name: "ApplicationUser_Id", newName: "IdentityUser_Id");
            RenameColumn(table: "dbo.IdentityUserClaims", name: "ApplicationUser_Id", newName: "IdentityUser_Id");
            RenameTable(name: "dbo.ApplicationUsers", newName: "IdentityUsers");
        }
    }
}
