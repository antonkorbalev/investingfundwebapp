namespace InvestingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user : DbMigration
    {
        public override void Up()
        {
            AddColumn("public.Users", "Benefit", c => c.Double(nullable: false));
            AddColumn("public.Users", "LastAttemptTime", c => c.DateTime(nullable: false));
            AddColumn("public.Users", "LoginAttempts", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("public.Users", "LoginAttempts");
            DropColumn("public.Users", "LastAttemptTime");
            DropColumn("public.Users", "Benefit");
        }
    }
}
