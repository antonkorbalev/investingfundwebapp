namespace InvestingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSharedRatiocolumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("public.Users", "SharedRatio");
        }
        
        public override void Down()
        {
            AddColumn("public.Users", "SharedRatio", c => c.Double(nullable: false));
        }
    }
}
