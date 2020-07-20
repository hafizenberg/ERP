namespace ChamdrimERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PurchaseOrders", "LedgerId", "dbo.Ledgers");
            DropIndex("dbo.PurchaseOrders", new[] { "LedgerId" });
            AlterColumn("dbo.PurchaseOrders", "LedgerId", c => c.Guid(nullable: false));
            CreateIndex("dbo.PurchaseOrders", "LedgerId");
            AddForeignKey("dbo.PurchaseOrders", "LedgerId", "dbo.Ledgers", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseOrders", "LedgerId", "dbo.Ledgers");
            DropIndex("dbo.PurchaseOrders", new[] { "LedgerId" });
            AlterColumn("dbo.PurchaseOrders", "LedgerId", c => c.Guid());
            CreateIndex("dbo.PurchaseOrders", "LedgerId");
            AddForeignKey("dbo.PurchaseOrders", "LedgerId", "dbo.Ledgers", "Id");
        }
    }
}
