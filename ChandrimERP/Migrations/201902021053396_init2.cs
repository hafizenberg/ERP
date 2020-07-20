namespace ChamdrimERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SalesOrderDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        OrderID = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        ProductName = c.String(),
                        ProductCode = c.Int(nullable: false),
                        ProductDescription = c.String(),
                        Quantity = c.Int(nullable: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MeasureUnit = c.String(),
                        BonusQuantity = c.Int(),
                        BatchOrSerial = c.String(),
                        NetTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discount = c.Decimal(precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAT = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: false)
                .ForeignKey("dbo.SalesOrders", t => t.OrderID, cascadeDelete: false)
                .Index(t => t.OrderID)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.SalesOrders",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        OrderDate = c.DateTime(nullable: false),
                        InvoiceNo = c.Int(nullable: false),
                        CustomerID = c.Guid(nullable: false),
                        LedgerId = c.Guid(),
                        DueDate = c.DateTime(nullable: false),
                        BranchId = c.Guid(nullable: false),
                        WarehouseId = c.Guid(nullable: false),
                        SalesAgentId = c.Guid(),
                        Narration = c.String(),
                        TotalQNT = c.Int(nullable: false),
                        TotalAmount = c.Decimal(precision: 18, scale: 2),
                        VatAmount = c.Decimal(precision: 18, scale: 2),
                        InvoicedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaseAmount = c.Decimal(precision: 18, scale: 2),
                        Addamount = c.Decimal(precision: 18, scale: 2),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: false)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: false)
                .ForeignKey("dbo.Ledgers", t => t.LedgerId)
                .ForeignKey("dbo.SalesAgents", t => t.SalesAgentId)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId, cascadeDelete: false)
                .Index(t => t.CustomerID)
                .Index(t => t.LedgerId)
                .Index(t => t.BranchId)
                .Index(t => t.WarehouseId)
                .Index(t => t.SalesAgentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalesOrderDetails", "OrderID", "dbo.SalesOrders");
            DropForeignKey("dbo.SalesOrders", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.SalesOrders", "SalesAgentId", "dbo.SalesAgents");
            DropForeignKey("dbo.SalesOrders", "LedgerId", "dbo.Ledgers");
            DropForeignKey("dbo.SalesOrders", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.SalesOrders", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.SalesOrderDetails", "ProductId", "dbo.Products");
            DropIndex("dbo.SalesOrders", new[] { "SalesAgentId" });
            DropIndex("dbo.SalesOrders", new[] { "WarehouseId" });
            DropIndex("dbo.SalesOrders", new[] { "BranchId" });
            DropIndex("dbo.SalesOrders", new[] { "LedgerId" });
            DropIndex("dbo.SalesOrders", new[] { "CustomerID" });
            DropIndex("dbo.SalesOrderDetails", new[] { "ProductId" });
            DropIndex("dbo.SalesOrderDetails", new[] { "OrderID" });
            DropTable("dbo.SalesOrders");
            DropTable("dbo.SalesOrderDetails");
        }
    }
}
