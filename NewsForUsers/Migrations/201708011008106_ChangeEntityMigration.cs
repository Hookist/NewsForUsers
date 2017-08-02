namespace NewsForUsers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeEntityMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Entity", "Link", c => c.String(maxLength: 300));
            AlterColumn("dbo.Entity", "Title", c => c.String(maxLength: 300));
            AlterColumn("dbo.Entity", "Text", c => c.String(maxLength: 2000));
            AlterColumn("dbo.Entity", "Image", c => c.String(maxLength: 500));
            AlterColumn("dbo.Entity", "PublicationDate", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Entity", "PublicationDate", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.Entity", "Image", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.Entity", "Text", c => c.String(nullable: false, maxLength: 2000));
            AlterColumn("dbo.Entity", "Title", c => c.String(nullable: false, maxLength: 300));
            DropColumn("dbo.Entity", "Link");
        }
    }
}
