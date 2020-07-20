namespace ChamdrimERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PurchaseOrderDetails", "ProductId", "dbo.Products");
            DropIndex("dbo.PurchaseOrderDetails", new[] { "ProductId" });
            AddColumn("dbo.Inventories", "ProId", c => c.Guid(nullable: false));
            AlterColumn("dbo.PurchaseOrderDetails", "ProductId", c => c.Guid(nullable: false));
            AlterColumn("dbo.PurchaseOrderDetails", "ProductCode", c => c.Int(nullable: false));
            CreateIndex("dbo.Inventories", "ProId");
            CreateIndex("dbo.PurchaseOrderDetails", "ProductId");
            AddForeignKey("dbo.Inventories", "ProId", "dbo.Products", "ProductId", cascadeDelete: false);
            AddForeignKey("dbo.PurchaseOrderDetails", "ProductId", "dbo.Products", "ProductId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseOrderDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Inventories", "ProId", "dbo.Products");
            DropIndex("dbo.PurchaseOrderDetails", new[] { "ProductId" });
            DropIndex("dbo.Inventories", new[] { "ProId" });
            AlterColumn("dbo.PurchaseOrderDetails", "ProductCode", c => c.Int());
            AlterColumn("dbo.PurchaseOrderDetails", "ProductId", c => c.Guid());
            DropColumn("dbo.Inventories", "ProId");
            CreateIndex("dbo.PurchaseOrderDetails", "ProductId");
            AddForeignKey("dbo.PurchaseOrderDetails", "ProductId", "dbo.Products", "ProductId");
        }
    }
}
