namespace NewsForUsers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StringsUpdateMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Collection", "Name", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Source", "Link", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Entity", "Title", c => c.String(nullable: false, maxLength: 300));
            AlterColumn("dbo.Entity", "Text", c => c.String(nullable: false, maxLength: 2000));
            AlterColumn("dbo.Entity", "Image", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.Entity", "PublicationDate", c => c.String(nullable: false, maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Entity", "PublicationDate", c => c.String(nullable: false, maxLength: 1));
            AlterColumn("dbo.Entity", "Image", c => c.String(nullable: false, maxLength: 1));
            AlterColumn("dbo.Entity", "Text", c => c.String(nullable: false, maxLength: 1));
            AlterColumn("dbo.Entity", "Title", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Source", "Link", c => c.String(nullable: false, maxLength: 1));
            AlterColumn("dbo.Collection", "Name", c => c.String(nullable: false, maxLength: 1));
        }
    }
}
