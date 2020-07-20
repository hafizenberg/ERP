namespace ChamdrimERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InventoryMovements",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        ItemCode = c.Int(nullable: false),
                        AleartLavel = c.Int(),
                        Status = c.String(),
                        InvoiceType = c.String(),
                        InvoiceNo = c.Int(nullable: false),
                        InQuantity = c.Int(),
                        OutQuantity = c.Int(),
                        BatchOrSerial = c.String(),
                        Note = c.String(),
                        UserName = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        ProId = c.Guid(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        WarehouseId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .ForeignKey("dbo.Products", t => t.ProId, cascadeDelete: false)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId)
                .Index(t => t.ProId)
                .Index(t => t.CompanyId)
                .Index(t => t.WarehouseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventoryMovements", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.InventoryMovements", "ProId", "dbo.Products");
            DropForeignKey("dbo.InventoryMovements", "CompanyId", "dbo.Companies");
            DropIndex("dbo.InventoryMovements", new[] { "WarehouseId" });
            DropIndex("dbo.InventoryMovements", new[] { "CompanyId" });
            DropIndex("dbo.InventoryMovements", new[] { "ProId" });
            DropTable("dbo.InventoryMovements");
        }
    }
}
