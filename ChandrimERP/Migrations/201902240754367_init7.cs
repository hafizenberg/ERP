namespace ChamdrimERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init7 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Images");
            AddColumn("dbo.Documents", "DocumentCode", c => c.String(nullable: false));
            AddColumn("dbo.Documents", "DocumentName", c => c.String(nullable: false));
            AddColumn("dbo.Documents", "DocumentDetails", c => c.String());
            AddColumn("dbo.Documents", "RefId", c => c.String());
            AddColumn("dbo.Documents", "EXPDate", c => c.DateTime());
            AddColumn("dbo.Documents", "UpdateDate", c => c.DateTime());
            AddColumn("dbo.Documents", "CreatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Documents", "CompanyId", c => c.Guid(nullable: false));
            AddColumn("dbo.Images", "ID", c => c.Guid(nullable: false, identity: true));
            AddColumn("dbo.Images", "DocumentPath", c => c.String());
            AddColumn("dbo.Images", "RefId", c => c.String());
            AddColumn("dbo.Images", "DocumentId", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Images", "ID");
            CreateIndex("dbo.Documents", "CompanyId");
            CreateIndex("dbo.Images", "DocumentId");
            AddForeignKey("dbo.Documents", "CompanyId", "dbo.Companies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Images", "DocumentId", "dbo.Documents", "Id", cascadeDelete: false);
            DropColumn("dbo.Documents", "DocumentPath");
            DropColumn("dbo.Images", "ImageID");
            DropColumn("dbo.Images", "ImagePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Images", "ImagePath", c => c.String());
            AddColumn("dbo.Images", "ImageID", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Documents", "DocumentPath", c => c.String());
            DropForeignKey("dbo.Images", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.Documents", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Images", new[] { "DocumentId" });
            DropIndex("dbo.Documents", new[] { "CompanyId" });
            DropPrimaryKey("dbo.Images");
            DropColumn("dbo.Images", "DocumentId");
            DropColumn("dbo.Images", "RefId");
            DropColumn("dbo.Images", "DocumentPath");
            DropColumn("dbo.Images", "ID");
            DropColumn("dbo.Documents", "CompanyId");
            DropColumn("dbo.Documents", "CreatedOn");
            DropColumn("dbo.Documents", "UpdateDate");
            DropColumn("dbo.Documents", "EXPDate");
            DropColumn("dbo.Documents", "RefId");
            DropColumn("dbo.Documents", "DocumentDetails");
            DropColumn("dbo.Documents", "DocumentName");
            DropColumn("dbo.Documents", "DocumentCode");
            AddPrimaryKey("dbo.Images", "ImageID");
        }
    }
}
