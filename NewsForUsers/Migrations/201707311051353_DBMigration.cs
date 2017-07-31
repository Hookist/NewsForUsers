namespace NewsForUsers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Collection",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SourceToCollection",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CollectionId = c.Int(nullable: false),
                        SourceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Source", t => t.SourceId)
                .ForeignKey("dbo.Collection", t => t.CollectionId)
                .Index(t => t.CollectionId)
                .Index(t => t.SourceId);
            
            CreateTable(
                "dbo.Source",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Link = c.String(nullable: false, maxLength: 200),
                        SourceTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SourceType", t => t.SourceTypeId)
                .Index(t => t.SourceTypeId);
            
            CreateTable(
                "dbo.Entity",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 300),
                        Text = c.String(nullable: false, maxLength: 2000),
                        Image = c.String(nullable: false, maxLength: 500),
                        PublicationDate = c.String(nullable: false, maxLength: 500),
                        SourceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Source", t => t.SourceId)
                .Index(t => t.SourceId);
            
            CreateTable(
                "dbo.SourceType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        CustomRole_Id = c.Int(),
                        ApplicationUser_Id = c.Int(),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.CustomRoles", t => t.CustomRole_Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id)
                .Index(t => t.CustomRole_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.ApplicationUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastVisitDate = c.DateTime(nullable: false),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        ApplicationUser_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                        ApplicationUser_Id = c.Int(),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.AspNetUserLogins", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.CustomUserClaims", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.AspNetUserRoles", "CustomRole_Id", "dbo.CustomRoles");
            DropForeignKey("dbo.SourceToCollection", "CollectionId", "dbo.Collection");
            DropForeignKey("dbo.Source", "SourceTypeId", "dbo.SourceType");
            DropForeignKey("dbo.SourceToCollection", "SourceId", "dbo.Source");
            DropForeignKey("dbo.Entity", "SourceId", "dbo.Source");
            DropIndex("dbo.AspNetUserLogins", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.CustomUserClaims", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "CustomRole_Id" });
            DropIndex("dbo.Entity", new[] { "SourceId" });
            DropIndex("dbo.Source", new[] { "SourceTypeId" });
            DropIndex("dbo.SourceToCollection", new[] { "SourceId" });
            DropIndex("dbo.SourceToCollection", new[] { "CollectionId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.CustomUserClaims");
            DropTable("dbo.ApplicationUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.CustomRoles");
            DropTable("dbo.SourceType");
            DropTable("dbo.Entity");
            DropTable("dbo.Source");
            DropTable("dbo.SourceToCollection");
            DropTable("dbo.Collection");
        }
    }
}
