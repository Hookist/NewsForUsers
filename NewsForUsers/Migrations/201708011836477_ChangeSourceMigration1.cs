namespace NewsForUsers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeSourceMigration1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Source", "SourceTypeId", "dbo.SourceType");
            DropIndex("dbo.Source", new[] { "SourceTypeId" });
            RenameColumn(table: "dbo.Source", name: "SourceTypeId", newName: "SourceType_Id");
            AlterColumn("dbo.Source", "SourceType_Id", c => c.Int());
            CreateIndex("dbo.Source", "SourceType_Id");
            AddForeignKey("dbo.Source", "SourceType_Id", "dbo.SourceType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Source", "SourceType_Id", "dbo.SourceType");
            DropIndex("dbo.Source", new[] { "SourceType_Id" });
            AlterColumn("dbo.Source", "SourceType_Id", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Source", name: "SourceType_Id", newName: "SourceTypeId");
            CreateIndex("dbo.Source", "SourceTypeId");
            AddForeignKey("dbo.Source", "SourceTypeId", "dbo.SourceType", "Id", cascadeDelete: true);
        }
    }
}
