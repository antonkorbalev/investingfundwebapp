namespace InvestingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RatesTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.Rates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        DateTimeStamp = c.DateTime(nullable: false),
                        Value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("public.Rates");
        }
    }
}
