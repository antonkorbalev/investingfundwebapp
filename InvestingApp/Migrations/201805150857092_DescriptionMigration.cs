namespace InvestingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DescriptionMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("public.FlowRows", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("public.FlowRows", "Description");
        }
    }
}
