namespace ChamdrimERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init8 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Documents", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Documents", new[] { "CompanyId" });
            AlterColumn("dbo.Documents", "CompanyId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Documents", "CompanyId");
            AddForeignKey("dbo.Documents", "CompanyId", "dbo.Companies", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Documents", new[] { "CompanyId" });
            AlterColumn("dbo.Documents", "CompanyId", c => c.Guid());
            CreateIndex("dbo.Documents", "CompanyId");
            AddForeignKey("dbo.Documents", "CompanyId", "dbo.Companies", "Id");
        }
    }
}
