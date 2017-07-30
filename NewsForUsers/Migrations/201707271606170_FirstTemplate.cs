namespace NewsForUsers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstTemplate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Collection",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 1),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
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
                        Link = c.String(nullable: false, maxLength: 1),
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
                        Title = c.String(nullable: false, maxLength: 100),
                        Text = c.String(nullable: false, maxLength: 1),
                        Image = c.String(nullable: false, maxLength: 1),
                        PublicationDate = c.String(nullable: false, maxLength: 1),
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
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 200),
                        LastVisit = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Collection", "UserId", "dbo.User");
            DropForeignKey("dbo.SourceToCollection", "CollectionId", "dbo.Collection");
            DropForeignKey("dbo.Source", "SourceTypeId", "dbo.SourceType");
            DropForeignKey("dbo.SourceToCollection", "SourceId", "dbo.Source");
            DropForeignKey("dbo.Entity", "SourceId", "dbo.Source");
            DropIndex("dbo.Entity", new[] { "SourceId" });
            DropIndex("dbo.Source", new[] { "SourceTypeId" });
            DropIndex("dbo.SourceToCollection", new[] { "SourceId" });
            DropIndex("dbo.SourceToCollection", new[] { "CollectionId" });
            DropIndex("dbo.Collection", new[] { "UserId" });
            DropTable("dbo.User");
            DropTable("dbo.SourceType");
            DropTable("dbo.Entity");
            DropTable("dbo.Source");
            DropTable("dbo.SourceToCollection");
            DropTable("dbo.Collection");
        }
    }
}
