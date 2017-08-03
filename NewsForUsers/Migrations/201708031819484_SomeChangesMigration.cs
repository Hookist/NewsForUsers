namespace NewsForUsers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeChangesMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Source", "SourceType_Id", "dbo.SourceType");
            DropIndex("dbo.Source", new[] { "SourceType_Id" });
            DropColumn("dbo.Source", "SourceType_Id");
            DropTable("dbo.SourceType");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SourceType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Source", "SourceType_Id", c => c.Int());
            CreateIndex("dbo.Source", "SourceType_Id");
            AddForeignKey("dbo.Source", "SourceType_Id", "dbo.SourceType", "Id");
        }
    }
}
