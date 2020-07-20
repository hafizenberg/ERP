namespace ChamdrimERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TailorOrders", "LedgerId", "dbo.Ledgers");
            DropIndex("dbo.TailorOrders", new[] { "LedgerId" });
            AlterColumn("dbo.TailorOrders", "LedgerId", c => c.Guid(nullable: false));
            CreateIndex("dbo.TailorOrders", "LedgerId");
            AddForeignKey("dbo.TailorOrders", "LedgerId", "dbo.Ledgers", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TailorOrders", "LedgerId", "dbo.Ledgers");
            DropIndex("dbo.TailorOrders", new[] { "LedgerId" });
            AlterColumn("dbo.TailorOrders", "LedgerId", c => c.Guid());
            CreateIndex("dbo.TailorOrders", "LedgerId");
            AddForeignKey("dbo.TailorOrders", "LedgerId", "dbo.Ledgers", "Id");
        }
    }
}
