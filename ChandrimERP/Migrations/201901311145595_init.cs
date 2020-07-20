namespace ChamdrimERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUser_Branch",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Branch_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Branch_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Branches", t => t.Branch_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Branch_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.ApplicationUser_Company",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Company_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Company_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Companies", t => t.Company_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Company_Id);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CompanyName = c.String(),
                        ContactFirstName = c.String(nullable: false),
                        ContactLastName = c.String(nullable: false),
                        Genders = c.Int(nullable: false),
                        Phone = c.String(nullable: false),
                        Email = c.String(),
                        WebPage = c.String(),
                        VatInformation = c.String(),
                        FinancialYearStart = c.Int(nullable: false),
                        BusinessType = c.Int(nullable: false),
                        CompanyLogo = c.String(),
                        Country = c.String(nullable: false),
                        State = c.String(),
                        City = c.String(nullable: false),
                        AddressLineOne = c.String(),
                        AddressLineTwo = c.String(),
                        ImageUrl = c.String(),
                        Notes = c.String(),
                        Status = c.Boolean(nullable: false),
                        Islocked = c.Boolean(nullable: false),
                        LockedDateTime = c.DateTime(),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BankDetailsCompanies",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        BankName = c.String(),
                        BranchName = c.String(),
                        CheckName = c.String(),
                        MicrCode = c.Int(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Branches",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        BranchName = c.String(),
                        Address = c.String(),
                        Status = c.Boolean(nullable: false),
                        Islocked = c.Boolean(nullable: false),
                        LockedDateTime = c.DateTime(),
                        CreatedOn = c.DateTime(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.BankDetailsBranches",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        BankName = c.String(),
                        BranchName = c.String(),
                        CheckName = c.String(),
                        MicrCode = c.Int(nullable: false),
                        BranchId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .Index(t => t.BranchId);
            
            CreateTable(
                "dbo.Branch_Customer",
                c => new
                    {
                        Branch_Id = c.Guid(nullable: false),
                        Customer_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Branch_Id, t.Customer_Id })
                .ForeignKey("dbo.Branches", t => t.Branch_Id, cascadeDelete: true)
                .ForeignKey("dbo.Customers", t => t.Customer_Id, cascadeDelete: true)
                .Index(t => t.Branch_Id)
                .Index(t => t.Customer_Id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Guid(nullable: false, identity: true),
                        CompanyName = c.String(),
                        CustomerCode = c.Int(nullable: false),
                        OpeningBlance = c.Decimal(precision: 18, scale: 2),
                        BlanceLimit = c.Decimal(precision: 18, scale: 2),
                        ContactFirstName = c.String(nullable: false),
                        ContactLastName = c.String(nullable: false),
                        Genders = c.Int(nullable: false),
                        Phone = c.String(nullable: false),
                        Email = c.String(),
                        WebPage = c.String(),
                        Country = c.String(),
                        State = c.String(),
                        City = c.String(),
                        AddressLineOne = c.String(),
                        AddressLineTwo = c.String(),
                        ImageUrl = c.String(),
                        Notes = c.String(),
                        Status = c.Boolean(nullable: false),
                        OrderDate = c.DateTime(),
                        Islocked = c.Boolean(nullable: false),
                        LockedDateTime = c.DateTime(),
                        StatusValue = c.String(),
                        NationalId = c.String(),
                        TinNumber = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerId)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.BankDetailsCustomers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        BankName = c.String(),
                        BranchName = c.String(),
                        CheckName = c.String(),
                        MicrCode = c.Int(nullable: false),
                        CustomerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.InternationalBankingCustomers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MethodType = c.String(),
                        EmailAddress = c.String(),
                        CustomerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.MobileBankingCustomers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MobileBankingType = c.String(),
                        MobileNumber = c.String(),
                        CustomerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Orders",
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
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .ForeignKey("dbo.Ledgers", t => t.LedgerId)
                .ForeignKey("dbo.SalesAgents", t => t.SalesAgentId)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId, cascadeDelete: true)
                .Index(t => t.CustomerID)
                .Index(t => t.LedgerId)
                .Index(t => t.BranchId)
                .Index(t => t.WarehouseId)
                .Index(t => t.SalesAgentId);
            
            CreateTable(
                "dbo.Ledgers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        LedgerCode = c.Int(nullable: false),
                        LedgerCategoryId = c.Guid(),
                        ParentLedgerId = c.Guid(),
                        EffectInventory = c.Boolean(nullable: false),
                        EffectPayrool = c.Boolean(nullable: false),
                        OpeningBalance = c.Decimal(precision: 18, scale: 2),
                        Address = c.String(),
                        Country = c.String(),
                        State = c.String(),
                        City = c.String(),
                        PhoneNo = c.String(),
                        Email = c.String(),
                        Status = c.Boolean(nullable: false),
                        Islocked = c.Boolean(nullable: false),
                        LockedDateTime = c.DateTime(),
                        CreatedOn = c.DateTime(nullable: false),
                        RefType = c.String(),
                        RefNo = c.Guid(),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.LedgerCategories", t => t.LedgerCategoryId)
                .Index(t => t.LedgerCategoryId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.FixedAssetCategories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        LedgerId = c.Guid(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .ForeignKey("dbo.Ledgers", t => t.LedgerId, cascadeDelete: true)
                .Index(t => t.LedgerId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.FixedAssets",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        AssetCode = c.String(),
                        AssetValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AssetLife = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DepreciationRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DepreciationEffectFrom = c.DateTime(nullable: false),
                        AccumulateDepriciation = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WrittenDownValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalvageValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Node = c.String(),
                        SupplierId = c.Guid(),
                        PurchaseDate = c.DateTime(),
                        WarrentyDetails = c.String(),
                        Status = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        BranchId = c.Guid(),
                        CompanyId = c.Guid(nullable: false),
                        WarehouseId = c.Guid(),
                        FixedAssetCategoryId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .ForeignKey("dbo.FixedAssetCategories", t => t.FixedAssetCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId)
                .Index(t => t.BranchId)
                .Index(t => t.CompanyId)
                .Index(t => t.WarehouseId)
                .Index(t => t.FixedAssetCategoryId);
            
            CreateTable(
                "dbo.Warehouses",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        WarehouseName = c.String(),
                        Address = c.String(),
                        Status = c.Boolean(nullable: false),
                        Islocked = c.Boolean(nullable: false),
                        LockedDateTime = c.DateTime(),
                        CreatedOn = c.DateTime(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Branch_Warehouse",
                c => new
                    {
                        Branch_Id = c.Guid(nullable: false),
                        Warehouse_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Branch_Id, t.Warehouse_Id })
                .ForeignKey("dbo.Branches", t => t.Branch_Id, cascadeDelete: true)
                .ForeignKey("dbo.Warehouses", t => t.Warehouse_Id, cascadeDelete: true)
                .Index(t => t.Branch_Id)
                .Index(t => t.Warehouse_Id);
            
            CreateTable(
                "dbo.Inventories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        ItemCode = c.Int(nullable: false),
                        BalanceQuantity = c.Int(nullable: false),
                        AleartLavel = c.Int(),
                        ItemMRP = c.Decimal(precision: 18, scale: 2),
                        ItemUnitCost = c.Decimal(precision: 18, scale: 2),
                        ItemEditionalCost = c.Decimal(precision: 18, scale: 2),
                        ItemTotalCost = c.Decimal(precision: 18, scale: 2),
                        ItemAvrageCost = c.Decimal(precision: 18, scale: 2),
                        UpdateDate = c.DateTime(),
                        CreatedOn = c.DateTime(nullable: false),
                        Status = c.Boolean(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        WarehouseId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId)
                .Index(t => t.ProductId)
                .Index(t => t.CompanyId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Guid(nullable: false, identity: true),
                        ProductCode = c.Int(nullable: false),
                        ProductName = c.String(),
                        ModelName = c.String(),
                        SupplierId = c.Guid(nullable: false),
                        ProductCategoryId = c.Int(nullable: false),
                        ProductSubCategoryId = c.Int(nullable: false),
                        ProductMeasureUnitId = c.Int(nullable: false),
                        ProductBrandId = c.Int(),
                        ProductRackId = c.Int(),
                        ProductType = c.String(),
                        ProductDescription = c.String(),
                        ProductSize = c.String(),
                        ImageUrl = c.String(),
                        ProductColor = c.String(),
                        ProductWeight = c.String(),
                        ProductQuantity = c.Int(),
                        ProductVolume = c.Int(),
                        ProductPrice = c.Decimal(precision: 18, scale: 2),
                        ProductUnitCost = c.Decimal(precision: 18, scale: 2),
                        ProductManufacture = c.String(),
                        ProductManufactureDate = c.DateTime(nullable: false),
                        ProductExpireDate = c.DateTime(nullable: false),
                        Vat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remarks = c.String(),
                        Status = c.Boolean(nullable: false),
                        Islocked = c.Boolean(nullable: false),
                        LockedDateTime = c.DateTime(),
                        StatusValue = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        Barcode = c.String(),
                        BatchOrSerial = c.String(),
                        CompanyId = c.Guid(nullable: false),
                        WarehouseId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .ForeignKey("dbo.ProductBrands", t => t.ProductBrandId)
                .ForeignKey("dbo.ProductCategories", t => t.ProductCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.ProductMeasureUnits", t => t.ProductMeasureUnitId, cascadeDelete: true)
                .ForeignKey("dbo.ProductRacks", t => t.ProductRackId)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId, cascadeDelete: true)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId, cascadeDelete: true)
                .Index(t => t.SupplierId)
                .Index(t => t.ProductCategoryId)
                .Index(t => t.ProductMeasureUnitId)
                .Index(t => t.ProductBrandId)
                .Index(t => t.ProductRackId)
                .Index(t => t.CompanyId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.Branch_Product",
                c => new
                    {
                        Branch_Id = c.Guid(nullable: false),
                        Product_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Branch_Id, t.Product_Id })
                .ForeignKey("dbo.Branches", t => t.Branch_Id, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_Id, cascadeDelete: true)
                .Index(t => t.Branch_Id)
                .Index(t => t.Product_Id);
            
            CreateTable(
                "dbo.OrderDetails",
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
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: false)
                .Index(t => t.OrderID)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ProductBrands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.ProductSubCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductCategoryId = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductCategories", t => t.ProductCategoryId, cascadeDelete: true)
                .Index(t => t.ProductCategoryId);
            
            CreateTable(
                "dbo.ProductMeasureUnits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Note = c.String(),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.ProductRacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.PurchaseOrderDetails",
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
                .ForeignKey("dbo.PurchaseOrders", t => t.OrderID, cascadeDelete: false)
                .Index(t => t.OrderID)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.PurchaseOrders",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        OrderDate = c.DateTime(nullable: false),
                        InvoiceNo = c.Int(),
                        SupplierID = c.Guid(nullable: false),
                        LedgerId = c.Guid(),
                        BranchId = c.Guid(nullable: false),
                        WarehouseId = c.Guid(nullable: false),
                        PONumber = c.String(),
                        Narration = c.String(),
                        TotalQNT = c.Int(nullable: false),
                        VatAmount = c.Decimal(precision: 18, scale: 2),
                        InvoicedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaseAmount = c.Decimal(precision: 18, scale: 2),
                        Addamount = c.Decimal(precision: 18, scale: 2),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Ledgers", t => t.LedgerId)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID, cascadeDelete: true)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId, cascadeDelete: true)
                .Index(t => t.SupplierID)
                .Index(t => t.LedgerId)
                .Index(t => t.BranchId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        SupplierId = c.Guid(nullable: false, identity: true),
                        SupplierCode = c.Int(nullable: false),
                        OpeningBlance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CompanyName = c.String(nullable: false),
                        JobTitle = c.String(nullable: false),
                        ContactFirstName = c.String(nullable: false),
                        ContactLastName = c.String(),
                        Genders = c.Int(nullable: false),
                        BussinessPhone = c.String(nullable: false),
                        MobilePhone = c.String(),
                        AddressLineOne = c.String(),
                        AddressLineTwo = c.String(),
                        NationalId = c.String(),
                        TinNumber = c.String(),
                        FaxNumber = c.String(),
                        Country = c.String(nullable: false),
                        State = c.String(),
                        City = c.String(nullable: false),
                        ZipOrPostalCode = c.String(),
                        ImageUrl = c.String(),
                        Email = c.String(),
                        Website = c.String(),
                        Notes = c.String(),
                        Status = c.Boolean(nullable: false),
                        Islocked = c.Boolean(nullable: false),
                        LockedDateTime = c.DateTime(),
                        CreatedOn = c.DateTime(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierId)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.BankDetailsSuppliers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        BankName = c.String(),
                        BranchName = c.String(),
                        CheckName = c.String(),
                        MicrCode = c.Int(nullable: false),
                        SupplierId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId, cascadeDelete: true)
                .Index(t => t.SupplierId);
            
            CreateTable(
                "dbo.Branch_Supplier",
                c => new
                    {
                        Branch_Id = c.Guid(nullable: false),
                        Supplier_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Branch_Id, t.Supplier_Id })
                .ForeignKey("dbo.Branches", t => t.Branch_Id, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.Supplier_Id, cascadeDelete: true)
                .Index(t => t.Branch_Id)
                .Index(t => t.Supplier_Id);
            
            CreateTable(
                "dbo.InternationalBankingSuppliers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MethodType = c.String(),
                        EmailAddress = c.String(),
                        SupplierId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId, cascadeDelete: true)
                .Index(t => t.SupplierId);
            
            CreateTable(
                "dbo.MobileBankingSuppliers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MobileBankingType = c.String(),
                        MobileNumber = c.String(),
                        SupplierId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId, cascadeDelete: true)
                .Index(t => t.SupplierId);
            
            CreateTable(
                "dbo.TailorOrderDetails",
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
                .ForeignKey("dbo.TailorOrders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.OrderID)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.TailorOrders",
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
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .ForeignKey("dbo.Ledgers", t => t.LedgerId)
                .ForeignKey("dbo.SalesAgents", t => t.SalesAgentId)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId, cascadeDelete: false)
                .Index(t => t.CustomerID)
                .Index(t => t.LedgerId)
                .Index(t => t.BranchId)
                .Index(t => t.WarehouseId)
                .Index(t => t.SalesAgentId);
            
            CreateTable(
                "dbo.SalesAgents",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.SeawingOrderDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Shelha = c.Int(),
                        IbadMohra = c.Int(),
                        TulLong = c.String(),
                        ArradBody = c.String(),
                        KocchorWaist = c.String(),
                        Ibad = c.String(),
                        Hip = c.String(),
                        KhetabShoulder = c.String(),
                        OrdunHand = c.String(),
                        Orgoba = c.String(),
                        Throat = c.String(),
                        GherEnclorure = c.String(),
                        Qty = c.Int(nullable: false),
                        Rate = c.Int(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TailorOrderId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TailorOrders", t => t.TailorOrderId, cascadeDelete: true)
                .Index(t => t.TailorOrderId);
            
            CreateTable(
                "dbo.InventoryExpiaries",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        ItemCode = c.Int(nullable: false),
                        BatchOrSerial = c.String(),
                        BalanceQuantity = c.Int(nullable: false),
                        ProductManufactureDate = c.DateTime(nullable: false),
                        ProductExpireDate = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        Status = c.Boolean(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        WarehouseId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.InventoryIncommings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        ItemCode = c.Int(nullable: false),
                        InvoiceNo = c.Int(),
                        InvoiceType = c.String(),
                        ItemQuantity = c.Int(),
                        ItemUnitCost = c.Decimal(precision: 18, scale: 2),
                        BatchOrSerial = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        Status = c.Boolean(nullable: false),
                        CompanyId = c.Guid(),
                        WarehouseId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId)
                .Index(t => t.CompanyId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.InventoryOutGoings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        ItemCode = c.Int(nullable: false),
                        InvoiceNo = c.Int(nullable: false),
                        InvoiceType = c.String(),
                        ItemQuantity = c.Int(nullable: false),
                        ItemUnitCost = c.Decimal(precision: 18, scale: 2),
                        BatchOrSerial = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        Status = c.Boolean(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        WarehouseId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.InventoryUpcommings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        ItemCode = c.Int(nullable: false),
                        InvoiceNo = c.Int(nullable: false),
                        InvoiceType = c.String(),
                        ItemQuantity = c.Int(nullable: false),
                        ItemUnitCost = c.Decimal(precision: 18, scale: 2),
                        BatchOrSerial = c.String(),
                        ExpectedDate = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        Status = c.Boolean(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        WarehouseId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.InventoryUpgoings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        ItemCode = c.Int(nullable: false),
                        InvoiceNo = c.Int(nullable: false),
                        InvoiceType = c.String(),
                        ItemQuantity = c.Int(nullable: false),
                        ItemUnitCost = c.Decimal(precision: 18, scale: 2),
                        BatchOrSerial = c.String(),
                        ExpectedDate = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        Status = c.Boolean(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        WarehouseId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.InventoryValueChanges",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        ItemCode = c.Int(nullable: false),
                        Current_unit_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Change_unit_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Current_MRP_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Change_MRP_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(nullable: false),
                        Status = c.Boolean(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        WarehouseId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.LedgerCategories",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ChartOfAccountId = c.Guid(nullable: false),
                        ParentLedgerCatId = c.Guid(),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChartOfAccounts", t => t.ChartOfAccountId, cascadeDelete: true)
                .Index(t => t.ChartOfAccountId);
            
            CreateTable(
                "dbo.ChartOfAccounts",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        ParentNode = c.Guid(),
                        CompanyId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Branch_Document",
                c => new
                    {
                        Branch_Id = c.Guid(nullable: false),
                        Document_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Branch_Id, t.Document_Id })
                .ForeignKey("dbo.Branches", t => t.Branch_Id, cascadeDelete: true)
                .ForeignKey("dbo.Documents", t => t.Document_Id, cascadeDelete: true)
                .Index(t => t.Branch_Id)
                .Index(t => t.Document_Id);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        DocumentCode = c.String(nullable: false),
                        DocumentName = c.String(nullable: false),
                        DocumentAccessCount = c.String(),
                        DocumentDetails = c.String(nullable: false),
                        DocumentPath = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Branch_Employee",
                c => new
                    {
                        Branch_Id = c.Guid(nullable: false),
                        Employee_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Branch_Id, t.Employee_Id })
                .ForeignKey("dbo.Branches", t => t.Branch_Id, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.Employee_Id, cascadeDelete: true)
                .Index(t => t.Branch_Id)
                .Index(t => t.Employee_Id);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        EmployeeCode = c.Int(nullable: false),
                        EmailAddress = c.String(nullable: false),
                        JobTitle = c.String(nullable: false),
                        BussinessPhone = c.String(maxLength: 20),
                        ImageUrl = c.String(),
                        Notes = c.String(),
                        Status = c.Boolean(nullable: false),
                        Islocked = c.Boolean(),
                        LockedDateTime = c.DateTime(),
                        CreatedOn = c.DateTime(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        EmpType = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .ForeignKey("dbo.EmployeeTypes", t => t.EmpType, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.EmpType);
            
            CreateTable(
                "dbo.BankDetailsEmployees",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        BankName = c.String(),
                        BranchName = c.String(),
                        CheckName = c.String(),
                        MicrCode = c.Int(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.EmpAttendanceInTimes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InTime = c.DateTime(nullable: false),
                        EmployeesId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeesId, cascadeDelete: true)
                .Index(t => t.EmployeesId);
            
            CreateTable(
                "dbo.EmpAttendanceOutTimes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OutTime = c.DateTime(nullable: false),
                        EmployeesId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeesId, cascadeDelete: true)
                .Index(t => t.EmployeesId);
            
            CreateTable(
                "dbo.EmployeeNomineeInfoes",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        NomineeName = c.String(nullable: false),
                        NomineeDetails = c.String(),
                        Signature = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        Country = c.String(nullable: false),
                        State = c.String(),
                        City = c.String(nullable: false),
                        ImageUrl = c.String(),
                        AddressLineOne = c.String(),
                        AddressLineTwo = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        EmpId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmpId, cascadeDelete: true)
                .Index(t => t.EmpId);
            
            CreateTable(
                "dbo.EmployeePayrollPolicies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PpolicyName = c.String(nullable: false),
                        Ppay = c.String(nullable: false),
                        Pstatus = c.String(nullable: false),
                        Paccount = c.String(nullable: false),
                        EmployeesId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeesId, cascadeDelete: true)
                .Index(t => t.EmployeesId);
            
            CreateTable(
                "dbo.EmployeePersionalInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        Genders = c.Int(nullable: false),
                        BloodGroup = c.Int(nullable: false),
                        HomePhone = c.String(maxLength: 20),
                        MobilePhone = c.String(nullable: false, maxLength: 20),
                        Country = c.String(nullable: false),
                        State = c.String(),
                        City = c.String(nullable: false),
                        AddressLineOne = c.String(),
                        AddressLineTwo = c.String(),
                        ZipOrPostalCode = c.String(),
                        NationalId = c.String(),
                        TinNumber = c.String(),
                        EmployeesId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeesId, cascadeDelete: true)
                .Index(t => t.EmployeesId);
            
            CreateTable(
                "dbo.EmployeePropInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeJoiningDate = c.DateTime(nullable: false),
                        SalaryTypes = c.Int(nullable: false),
                        EmpBasicSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsOvertime = c.Boolean(nullable: false),
                        EmployeesId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeesId, cascadeDelete: true)
                .Index(t => t.EmployeesId);
            
            CreateTable(
                "dbo.EmployeeTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.InternationalBankingEmployees",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MethodType = c.String(),
                        EmailAddress = c.String(),
                        EmployeeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.MobileBankingEmployees",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MobileBankingType = c.String(),
                        MobileNumber = c.String(),
                        EmployeeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.Branch_SalesAgent",
                c => new
                    {
                        Branch_Id = c.Guid(nullable: false),
                        SalesAgent_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Branch_Id, t.SalesAgent_Id })
                .ForeignKey("dbo.Branches", t => t.Branch_Id, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.SalesAgent_Id, cascadeDelete: true)
                .Index(t => t.Branch_Id)
                .Index(t => t.SalesAgent_Id);
            
            CreateTable(
                "dbo.Branch_Tailor",
                c => new
                    {
                        Branch_Id = c.Guid(nullable: false),
                        Tailor_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Branch_Id, t.Tailor_Id })
                .ForeignKey("dbo.Branches", t => t.Branch_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tailors", t => t.Tailor_Id, cascadeDelete: true)
                .Index(t => t.Branch_Id)
                .Index(t => t.Tailor_Id);
            
            CreateTable(
                "dbo.Tailors",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CompanyId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.InternationalBankingBranches",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MethodType = c.String(),
                        EmailAddress = c.String(),
                        BranchId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .Index(t => t.BranchId);
            
            CreateTable(
                "dbo.MobileBankingBranches",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MobileBankingType = c.String(),
                        MobileNumber = c.String(),
                        BranchId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .Index(t => t.BranchId);
            
            CreateTable(
                "dbo.DressModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        Name = c.String(),
                        ImageUrl = c.String(),
                        Note = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.InternationalBankingCompanies",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MethodType = c.String(),
                        EmailAddress = c.String(),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.MobileBankingCompanies",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MobileBankingType = c.String(),
                        MobileNumber = c.String(),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.PServices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PServiceCode = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        updateDate = c.DateTime(),
                        Note = c.String(),
                        ServiceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EstimatedTime = c.String(),
                        Status = c.Boolean(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: false)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Branch_PService",
                c => new
                    {
                        Branch_Id = c.Guid(nullable: false),
                        PService_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Branch_Id, t.PService_Id })
                .ForeignKey("dbo.Branches", t => t.Branch_Id, cascadeDelete: true)
                .ForeignKey("dbo.PServices", t => t.PService_Id, cascadeDelete: true)
                .Index(t => t.Branch_Id)
                .Index(t => t.PService_Id);
            
            CreateTable(
                "dbo.PserviceImages",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PServiceId = c.Guid(nullable: false),
                        ImageId = c.String(),
                        CompanyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.PServices", t => t.PServiceId, cascadeDelete: true)
                .Index(t => t.PServiceId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Vouchers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        VoucherType = c.String(),
                        Name = c.String(),
                        Prefix = c.String(),
                        CompanyId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Genders = c.Int(nullable: false),
                        Type = c.String(),
                        RetryAttempt = c.Int(),
                        Status = c.Boolean(),
                        Islocked = c.Boolean(),
                        LockedDateTime = c.DateTime(),
                        CreatedOn = c.DateTime(nullable: false),
                        Country = c.String(),
                        State = c.String(),
                        City = c.String(),
                        AddressLineOne = c.String(),
                        AddressLineTwo = c.String(),
                        PhotosUrl = c.String(),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        CityName = c.String(),
                        StateId = c.Int(),
                    })
                .PrimaryKey(t => t.CityId)
                .ForeignKey("dbo.States", t => t.StateId)
                .Index(t => t.StateId);
            
            CreateTable(
                "dbo.States",
                c => new
                    {
                        StateId = c.Int(nullable: false, identity: true),
                        StateName = c.String(),
                        CountryId = c.Int(),
                    })
                .PrimaryKey(t => t.StateId)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryId = c.Int(nullable: false, identity: true),
                        SortName = c.String(),
                        CountryName = c.String(),
                        PhoneCode = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CountryId);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        ImageID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        ImagePath = c.String(),
                    })
                .PrimaryKey(t => t.ImageID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        studentID = c.Int(nullable: false, identity: true),
                        studentName = c.String(),
                        studentAddress = c.String(),
                        studentNote = c.String(),
                    })
                .PrimaryKey(t => t.studentID);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        BranchId = c.Guid(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        VoucherNo = c.Int(nullable: false),
                        VoucherType = c.String(),
                        TrasactionalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Narration = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransactionDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        VoucherNo = c.Int(nullable: false),
                        VoucherType = c.String(),
                        DabetLedger = c.String(),
                        CreditLedger = c.String(),
                        DebetAmount = c.Decimal(precision: 18, scale: 2),
                        CreditAmount = c.Decimal(precision: 18, scale: 2),
                        Narration = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                        Transactions_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Transactions", t => t.Transactions_Id)
                .Index(t => t.Transactions_Id);
            
            CreateTable(
                "dbo.userLogDatas",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        AccessDate = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        LoginTime = c.DateTime(nullable: false),
                        LogOutTime = c.DateTime(nullable: false),
                        TotalTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IPAddress = c.Long(),
                        MacAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransactionDetails", "Transactions_Id", "dbo.Transactions");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.States", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Cities", "StateId", "dbo.States");
            DropForeignKey("dbo.ApplicationUser_Branch", "Branch_Id", "dbo.Branches");
            DropForeignKey("dbo.ApplicationUser_Branch", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserDetails", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUser_Company", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.Vouchers", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.PserviceImages", "PServiceId", "dbo.PServices");
            DropForeignKey("dbo.PserviceImages", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.PServices", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Branch_PService", "PService_Id", "dbo.PServices");
            DropForeignKey("dbo.Branch_PService", "Branch_Id", "dbo.Branches");
            DropForeignKey("dbo.MobileBankingCompanies", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.InternationalBankingCompanies", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.DressModels", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.MobileBankingBranches", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.InternationalBankingBranches", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Branches", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Branch_Tailor", "Tailor_Id", "dbo.Tailors");
            DropForeignKey("dbo.Tailors", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Branch_Tailor", "Branch_Id", "dbo.Branches");
            DropForeignKey("dbo.Branch_SalesAgent", "SalesAgent_Id", "dbo.Employees");
            DropForeignKey("dbo.Branch_SalesAgent", "Branch_Id", "dbo.Branches");
            DropForeignKey("dbo.Branch_Employee", "Employee_Id", "dbo.Employees");
            DropForeignKey("dbo.MobileBankingEmployees", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.InternationalBankingEmployees", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "EmpType", "dbo.EmployeeTypes");
            DropForeignKey("dbo.EmployeeTypes", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.EmployeePropInfoes", "EmployeesId", "dbo.Employees");
            DropForeignKey("dbo.EmployeePersionalInfoes", "EmployeesId", "dbo.Employees");
            DropForeignKey("dbo.EmployeePayrollPolicies", "EmployeesId", "dbo.Employees");
            DropForeignKey("dbo.EmployeeNomineeInfoes", "EmpId", "dbo.Employees");
            DropForeignKey("dbo.EmpAttendanceOutTimes", "EmployeesId", "dbo.Employees");
            DropForeignKey("dbo.EmpAttendanceInTimes", "EmployeesId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.BankDetailsEmployees", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Branch_Employee", "Branch_Id", "dbo.Branches");
            DropForeignKey("dbo.Branch_Document", "Document_Id", "dbo.Documents");
            DropForeignKey("dbo.Documents", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Branch_Document", "Branch_Id", "dbo.Branches");
            DropForeignKey("dbo.Branch_Customer", "Customer_Id", "dbo.Customers");
            DropForeignKey("dbo.Orders", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.Orders", "SalesAgentId", "dbo.SalesAgents");
            DropForeignKey("dbo.Orders", "LedgerId", "dbo.Ledgers");
            DropForeignKey("dbo.Ledgers", "LedgerCategoryId", "dbo.LedgerCategories");
            DropForeignKey("dbo.LedgerCategories", "ChartOfAccountId", "dbo.ChartOfAccounts");
            DropForeignKey("dbo.ChartOfAccounts", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.FixedAssetCategories", "LedgerId", "dbo.Ledgers");
            DropForeignKey("dbo.FixedAssets", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.InventoryValueChanges", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.InventoryValueChanges", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.InventoryUpgoings", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.InventoryUpgoings", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.InventoryUpcommings", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.InventoryUpcommings", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.InventoryOutGoings", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.InventoryOutGoings", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.InventoryIncommings", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.InventoryIncommings", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.InventoryExpiaries", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.InventoryExpiaries", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Inventories", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.Inventories", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.TailorOrderDetails", "OrderID", "dbo.TailorOrders");
            DropForeignKey("dbo.TailorOrders", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.SeawingOrderDetails", "TailorOrderId", "dbo.TailorOrders");
            DropForeignKey("dbo.TailorOrders", "SalesAgentId", "dbo.SalesAgents");
            DropForeignKey("dbo.SalesAgents", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.TailorOrders", "LedgerId", "dbo.Ledgers");
            DropForeignKey("dbo.TailorOrders", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.TailorOrders", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.TailorOrderDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.PurchaseOrderDetails", "OrderID", "dbo.PurchaseOrders");
            DropForeignKey("dbo.PurchaseOrders", "WarehouseId", "dbo.Warehouses");
            DropForeignKey("dbo.PurchaseOrders", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.MobileBankingSuppliers", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.InternationalBankingSuppliers", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.Suppliers", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Branch_Supplier", "Supplier_Id", "dbo.Suppliers");
            DropForeignKey("dbo.Branch_Supplier", "Branch_Id", "dbo.Branches");
            DropForeignKey("dbo.BankDetailsSuppliers", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.PurchaseOrders", "LedgerId", "dbo.Ledgers");
            DropForeignKey("dbo.PurchaseOrders", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.PurchaseOrderDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "ProductRackId", "dbo.ProductRacks");
            DropForeignKey("dbo.ProductRacks", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Products", "ProductMeasureUnitId", "dbo.ProductMeasureUnits");
            DropForeignKey("dbo.ProductMeasureUnits", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Products", "ProductCategoryId", "dbo.ProductCategories");
            DropForeignKey("dbo.ProductSubCategories", "ProductCategoryId", "dbo.ProductCategories");
            DropForeignKey("dbo.ProductCategories", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Products", "ProductBrandId", "dbo.ProductBrands");
            DropForeignKey("dbo.ProductBrands", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.OrderDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderDetails", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Products", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Branch_Product", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Branch_Product", "Branch_Id", "dbo.Branches");
            DropForeignKey("dbo.Inventories", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Warehouses", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Branch_Warehouse", "Warehouse_Id", "dbo.Warehouses");
            DropForeignKey("dbo.Branch_Warehouse", "Branch_Id", "dbo.Branches");
            DropForeignKey("dbo.FixedAssets", "FixedAssetCategoryId", "dbo.FixedAssetCategories");
            DropForeignKey("dbo.FixedAssets", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.FixedAssets", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.FixedAssetCategories", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Ledgers", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Orders", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Orders", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.MobileBankingCustomers", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.InternationalBankingCustomers", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Customers", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.BankDetailsCustomers", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Branch_Customer", "Branch_Id", "dbo.Branches");
            DropForeignKey("dbo.BankDetailsBranches", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.BankDetailsCompanies", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.ApplicationUser_Company", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.TransactionDetails", new[] { "Transactions_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.States", new[] { "CountryId" });
            DropIndex("dbo.Cities", new[] { "StateId" });
            DropIndex("dbo.UserDetails", new[] { "Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.Vouchers", new[] { "CompanyId" });
            DropIndex("dbo.PserviceImages", new[] { "CompanyId" });
            DropIndex("dbo.PserviceImages", new[] { "PServiceId" });
            DropIndex("dbo.Branch_PService", new[] { "PService_Id" });
            DropIndex("dbo.Branch_PService", new[] { "Branch_Id" });
            DropIndex("dbo.PServices", new[] { "CompanyId" });
            DropIndex("dbo.MobileBankingCompanies", new[] { "CompanyId" });
            DropIndex("dbo.InternationalBankingCompanies", new[] { "CompanyId" });
            DropIndex("dbo.DressModels", new[] { "CompanyId" });
            DropIndex("dbo.MobileBankingBranches", new[] { "BranchId" });
            DropIndex("dbo.InternationalBankingBranches", new[] { "BranchId" });
            DropIndex("dbo.Tailors", new[] { "CompanyId" });
            DropIndex("dbo.Branch_Tailor", new[] { "Tailor_Id" });
            DropIndex("dbo.Branch_Tailor", new[] { "Branch_Id" });
            DropIndex("dbo.Branch_SalesAgent", new[] { "SalesAgent_Id" });
            DropIndex("dbo.Branch_SalesAgent", new[] { "Branch_Id" });
            DropIndex("dbo.MobileBankingEmployees", new[] { "EmployeeId" });
            DropIndex("dbo.InternationalBankingEmployees", new[] { "EmployeeId" });
            DropIndex("dbo.EmployeeTypes", new[] { "CompanyId" });
            DropIndex("dbo.EmployeePropInfoes", new[] { "EmployeesId" });
            DropIndex("dbo.EmployeePersionalInfoes", new[] { "EmployeesId" });
            DropIndex("dbo.EmployeePayrollPolicies", new[] { "EmployeesId" });
            DropIndex("dbo.EmployeeNomineeInfoes", new[] { "EmpId" });
            DropIndex("dbo.EmpAttendanceOutTimes", new[] { "EmployeesId" });
            DropIndex("dbo.EmpAttendanceInTimes", new[] { "EmployeesId" });
            DropIndex("dbo.BankDetailsEmployees", new[] { "EmployeeId" });
            DropIndex("dbo.Employees", new[] { "EmpType" });
            DropIndex("dbo.Employees", new[] { "CompanyId" });
            DropIndex("dbo.Branch_Employee", new[] { "Employee_Id" });
            DropIndex("dbo.Branch_Employee", new[] { "Branch_Id" });
            DropIndex("dbo.Documents", new[] { "CompanyId" });
            DropIndex("dbo.Branch_Document", new[] { "Document_Id" });
            DropIndex("dbo.Branch_Document", new[] { "Branch_Id" });
            DropIndex("dbo.ChartOfAccounts", new[] { "CompanyId" });
            DropIndex("dbo.LedgerCategories", new[] { "ChartOfAccountId" });
            DropIndex("dbo.InventoryValueChanges", new[] { "WarehouseId" });
            DropIndex("dbo.InventoryValueChanges", new[] { "CompanyId" });
            DropIndex("dbo.InventoryUpgoings", new[] { "WarehouseId" });
            DropIndex("dbo.InventoryUpgoings", new[] { "CompanyId" });
            DropIndex("dbo.InventoryUpcommings", new[] { "WarehouseId" });
            DropIndex("dbo.InventoryUpcommings", new[] { "CompanyId" });
            DropIndex("dbo.InventoryOutGoings", new[] { "WarehouseId" });
            DropIndex("dbo.InventoryOutGoings", new[] { "CompanyId" });
            DropIndex("dbo.InventoryIncommings", new[] { "WarehouseId" });
            DropIndex("dbo.InventoryIncommings", new[] { "CompanyId" });
            DropIndex("dbo.InventoryExpiaries", new[] { "WarehouseId" });
            DropIndex("dbo.InventoryExpiaries", new[] { "CompanyId" });
            DropIndex("dbo.SeawingOrderDetails", new[] { "TailorOrderId" });
            DropIndex("dbo.SalesAgents", new[] { "CompanyId" });
            DropIndex("dbo.TailorOrders", new[] { "SalesAgentId" });
            DropIndex("dbo.TailorOrders", new[] { "WarehouseId" });
            DropIndex("dbo.TailorOrders", new[] { "BranchId" });
            DropIndex("dbo.TailorOrders", new[] { "LedgerId" });
            DropIndex("dbo.TailorOrders", new[] { "CustomerID" });
            DropIndex("dbo.TailorOrderDetails", new[] { "ProductId" });
            DropIndex("dbo.TailorOrderDetails", new[] { "OrderID" });
            DropIndex("dbo.MobileBankingSuppliers", new[] { "SupplierId" });
            DropIndex("dbo.InternationalBankingSuppliers", new[] { "SupplierId" });
            DropIndex("dbo.Branch_Supplier", new[] { "Supplier_Id" });
            DropIndex("dbo.Branch_Supplier", new[] { "Branch_Id" });
            DropIndex("dbo.BankDetailsSuppliers", new[] { "SupplierId" });
            DropIndex("dbo.Suppliers", new[] { "CompanyId" });
            DropIndex("dbo.PurchaseOrders", new[] { "WarehouseId" });
            DropIndex("dbo.PurchaseOrders", new[] { "BranchId" });
            DropIndex("dbo.PurchaseOrders", new[] { "LedgerId" });
            DropIndex("dbo.PurchaseOrders", new[] { "SupplierID" });
            DropIndex("dbo.PurchaseOrderDetails", new[] { "ProductId" });
            DropIndex("dbo.PurchaseOrderDetails", new[] { "OrderID" });
            DropIndex("dbo.ProductRacks", new[] { "CompanyId" });
            DropIndex("dbo.ProductMeasureUnits", new[] { "CompanyId" });
            DropIndex("dbo.ProductSubCategories", new[] { "ProductCategoryId" });
            DropIndex("dbo.ProductCategories", new[] { "CompanyId" });
            DropIndex("dbo.ProductBrands", new[] { "CompanyId" });
            DropIndex("dbo.OrderDetails", new[] { "ProductId" });
            DropIndex("dbo.OrderDetails", new[] { "OrderID" });
            DropIndex("dbo.Branch_Product", new[] { "Product_Id" });
            DropIndex("dbo.Branch_Product", new[] { "Branch_Id" });
            DropIndex("dbo.Products", new[] { "WarehouseId" });
            DropIndex("dbo.Products", new[] { "CompanyId" });
            DropIndex("dbo.Products", new[] { "ProductRackId" });
            DropIndex("dbo.Products", new[] { "ProductBrandId" });
            DropIndex("dbo.Products", new[] { "ProductMeasureUnitId" });
            DropIndex("dbo.Products", new[] { "ProductCategoryId" });
            DropIndex("dbo.Products", new[] { "SupplierId" });
            DropIndex("dbo.Inventories", new[] { "WarehouseId" });
            DropIndex("dbo.Inventories", new[] { "CompanyId" });
            DropIndex("dbo.Inventories", new[] { "ProductId" });
            DropIndex("dbo.Branch_Warehouse", new[] { "Warehouse_Id" });
            DropIndex("dbo.Branch_Warehouse", new[] { "Branch_Id" });
            DropIndex("dbo.Warehouses", new[] { "CompanyId" });
            DropIndex("dbo.FixedAssets", new[] { "FixedAssetCategoryId" });
            DropIndex("dbo.FixedAssets", new[] { "WarehouseId" });
            DropIndex("dbo.FixedAssets", new[] { "CompanyId" });
            DropIndex("dbo.FixedAssets", new[] { "BranchId" });
            DropIndex("dbo.FixedAssetCategories", new[] { "CompanyId" });
            DropIndex("dbo.FixedAssetCategories", new[] { "LedgerId" });
            DropIndex("dbo.Ledgers", new[] { "CompanyId" });
            DropIndex("dbo.Ledgers", new[] { "LedgerCategoryId" });
            DropIndex("dbo.Orders", new[] { "SalesAgentId" });
            DropIndex("dbo.Orders", new[] { "WarehouseId" });
            DropIndex("dbo.Orders", new[] { "BranchId" });
            DropIndex("dbo.Orders", new[] { "LedgerId" });
            DropIndex("dbo.Orders", new[] { "CustomerID" });
            DropIndex("dbo.MobileBankingCustomers", new[] { "CustomerId" });
            DropIndex("dbo.InternationalBankingCustomers", new[] { "CustomerId" });
            DropIndex("dbo.BankDetailsCustomers", new[] { "CustomerId" });
            DropIndex("dbo.Customers", new[] { "CompanyId" });
            DropIndex("dbo.Branch_Customer", new[] { "Customer_Id" });
            DropIndex("dbo.Branch_Customer", new[] { "Branch_Id" });
            DropIndex("dbo.BankDetailsBranches", new[] { "BranchId" });
            DropIndex("dbo.Branches", new[] { "CompanyId" });
            DropIndex("dbo.BankDetailsCompanies", new[] { "CompanyId" });
            DropIndex("dbo.ApplicationUser_Company", new[] { "Company_Id" });
            DropIndex("dbo.ApplicationUser_Company", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.ApplicationUser_Branch", new[] { "Branch_Id" });
            DropIndex("dbo.ApplicationUser_Branch", new[] { "ApplicationUser_Id" });
            DropTable("dbo.userLogDatas");
            DropTable("dbo.TransactionDetails");
            DropTable("dbo.Transactions");
            DropTable("dbo.Students");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Images");
            DropTable("dbo.Countries");
            DropTable("dbo.States");
            DropTable("dbo.Cities");
            DropTable("dbo.UserDetails");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.Vouchers");
            DropTable("dbo.PserviceImages");
            DropTable("dbo.Branch_PService");
            DropTable("dbo.PServices");
            DropTable("dbo.MobileBankingCompanies");
            DropTable("dbo.InternationalBankingCompanies");
            DropTable("dbo.DressModels");
            DropTable("dbo.MobileBankingBranches");
            DropTable("dbo.InternationalBankingBranches");
            DropTable("dbo.Tailors");
            DropTable("dbo.Branch_Tailor");
            DropTable("dbo.Branch_SalesAgent");
            DropTable("dbo.MobileBankingEmployees");
            DropTable("dbo.InternationalBankingEmployees");
            DropTable("dbo.EmployeeTypes");
            DropTable("dbo.EmployeePropInfoes");
            DropTable("dbo.EmployeePersionalInfoes");
            DropTable("dbo.EmployeePayrollPolicies");
            DropTable("dbo.EmployeeNomineeInfoes");
            DropTable("dbo.EmpAttendanceOutTimes");
            DropTable("dbo.EmpAttendanceInTimes");
            DropTable("dbo.BankDetailsEmployees");
            DropTable("dbo.Employees");
            DropTable("dbo.Branch_Employee");
            DropTable("dbo.Documents");
            DropTable("dbo.Branch_Document");
            DropTable("dbo.ChartOfAccounts");
            DropTable("dbo.LedgerCategories");
            DropTable("dbo.InventoryValueChanges");
            DropTable("dbo.InventoryUpgoings");
            DropTable("dbo.InventoryUpcommings");
            DropTable("dbo.InventoryOutGoings");
            DropTable("dbo.InventoryIncommings");
            DropTable("dbo.InventoryExpiaries");
            DropTable("dbo.SeawingOrderDetails");
            DropTable("dbo.SalesAgents");
            DropTable("dbo.TailorOrders");
            DropTable("dbo.TailorOrderDetails");
            DropTable("dbo.MobileBankingSuppliers");
            DropTable("dbo.InternationalBankingSuppliers");
            DropTable("dbo.Branch_Supplier");
            DropTable("dbo.BankDetailsSuppliers");
            DropTable("dbo.Suppliers");
            DropTable("dbo.PurchaseOrders");
            DropTable("dbo.PurchaseOrderDetails");
            DropTable("dbo.ProductRacks");
            DropTable("dbo.ProductMeasureUnits");
            DropTable("dbo.ProductSubCategories");
            DropTable("dbo.ProductCategories");
            DropTable("dbo.ProductBrands");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Branch_Product");
            DropTable("dbo.Products");
            DropTable("dbo.Inventories");
            DropTable("dbo.Branch_Warehouse");
            DropTable("dbo.Warehouses");
            DropTable("dbo.FixedAssets");
            DropTable("dbo.FixedAssetCategories");
            DropTable("dbo.Ledgers");
            DropTable("dbo.Orders");
            DropTable("dbo.MobileBankingCustomers");
            DropTable("dbo.InternationalBankingCustomers");
            DropTable("dbo.BankDetailsCustomers");
            DropTable("dbo.Customers");
            DropTable("dbo.Branch_Customer");
            DropTable("dbo.BankDetailsBranches");
            DropTable("dbo.Branches");
            DropTable("dbo.BankDetailsCompanies");
            DropTable("dbo.Companies");
            DropTable("dbo.ApplicationUser_Company");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.ApplicationUser_Branch");
        }
    }
}
