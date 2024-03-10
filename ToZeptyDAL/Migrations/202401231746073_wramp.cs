namespace FoodDeliveryDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wramp : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Categories", "CategoryName", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Categories", "CategoryName", c => c.String(nullable: false));
        }
    }
}
