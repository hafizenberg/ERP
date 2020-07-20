namespace ChamdrimERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionDetails", "BranchID", c => c.Guid(nullable: false));
            AddColumn("dbo.TransactionDetails", "LedgerID", c => c.Guid(nullable: false));
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
            DropColumn("dbo.TransactionDetails", "LedgerID");
            DropColumn("dbo.TransactionDetails", "BranchID");
        }
    }
}
