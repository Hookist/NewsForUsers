namespace NewsForUsers.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePublicationDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Entity", "PublicationDate", c => c.DateTimeOffset(precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Entity", "PublicationDate", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
    }
}
