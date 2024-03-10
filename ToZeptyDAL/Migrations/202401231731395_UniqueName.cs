using System.Data.Entity.Migrations;

public partial class UniqueName : DbMigration
{
    public override void Up()
    {
        DropIndex("dbo.Categories", new[] { "CategoryName" });
        CreateIndex("dbo.Categories", "CategoryName", unique: true);
    }

    public override void Down()
    {
        DropIndex("dbo.Categories", new[] { "CategoryName" });
    }
}
