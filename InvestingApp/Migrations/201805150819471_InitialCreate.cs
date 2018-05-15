namespace InvestingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.BalancesRows",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTimeStamp = c.DateTime(nullable: false),
                        Balance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.FlowRows",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTimeStamp = c.DateTime(nullable: false),
                        Payment = c.Double(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "public.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Login = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        SharedRatio = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("public.FlowRows", "User_Id", "public.Users");
            DropIndex("public.FlowRows", new[] { "User_Id" });
            DropTable("public.Users");
            DropTable("public.FlowRows");
            DropTable("public.BalancesRows");
        }
    }
}
