namespace NewsForUsers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeSourceMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Source", "SourceTypeId", "dbo.SourceType");
            AddForeignKey("dbo.Source", "SourceTypeId", "dbo.SourceType", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Source", "SourceTypeId", "dbo.SourceType");
            AddForeignKey("dbo.Source", "SourceTypeId", "dbo.SourceType", "Id");
        }
    }
}
