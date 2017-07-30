namespace NewsForUsers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewUserTblMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Collection", "UserId", "dbo.User");
            DropIndex("dbo.Collection", new[] { "UserId" });
            DropTable("dbo.User");
        }
        
        public override void Down()
        {
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
            
            CreateIndex("dbo.Collection", "UserId");
            AddForeignKey("dbo.Collection", "UserId", "dbo.User", "Id");
        }
    }
}
