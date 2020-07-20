namespace ChamdrimERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init9 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TransactionDetails", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.TransactionDetails", "LedgerID", "dbo.Ledgers");
            DropIndex("dbo.TransactionDetails", new[] { "BranchID" });
            DropIndex("dbo.TransactionDetails", new[] { "LedgerID" });
            AlterColumn("dbo.TransactionDetails", "VoucherNo", c => c.Int(nullable: false));
            AlterColumn("dbo.TransactionDetails", "TransactionDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TransactionDetails", "BranchID", c => c.Guid(nullable: false));
            AlterColumn("dbo.TransactionDetails", "LedgerID", c => c.Guid(nullable: false));
            CreateIndex("dbo.TransactionDetails", "BranchID");
            CreateIndex("dbo.TransactionDetails", "LedgerID");
            AddForeignKey("dbo.TransactionDetails", "BranchID", "dbo.Branches", "Id", cascadeDelete: false);
            AddForeignKey("dbo.TransactionDetails", "LedgerID", "dbo.Ledgers", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransactionDetails", "LedgerID", "dbo.Ledgers");
            DropForeignKey("dbo.TransactionDetails", "BranchID", "dbo.Branches");
            DropIndex("dbo.TransactionDetails", new[] { "LedgerID" });
            DropIndex("dbo.TransactionDetails", new[] { "BranchID" });
            AlterColumn("dbo.TransactionDetails", "LedgerID", c => c.Guid());
            AlterColumn("dbo.TransactionDetails", "BranchID", c => c.Guid());
            AlterColumn("dbo.TransactionDetails", "TransactionDate", c => c.DateTime());
            AlterColumn("dbo.TransactionDetails", "VoucherNo", c => c.Int());
            CreateIndex("dbo.TransactionDetails", "LedgerID");
            CreateIndex("dbo.TransactionDetails", "BranchID");
            AddForeignKey("dbo.TransactionDetails", "LedgerID", "dbo.Ledgers", "Id");
            AddForeignKey("dbo.TransactionDetails", "BranchID", "dbo.Branches", "Id");
        }
    }
}
