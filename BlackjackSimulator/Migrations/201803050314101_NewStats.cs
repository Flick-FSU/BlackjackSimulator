namespace BlackjackSimulator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewStats : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ScenarioPlayerResults", newName: "PlayerSimulationsStatistics");
            AddColumn("dbo.PlayerSimulationsStatistics", "StrategyName", c => c.String());
            DropColumn("dbo.PlayerSimulationsStatistics", "ScenarioName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlayerSimulationsStatistics", "ScenarioName", c => c.String());
            DropColumn("dbo.PlayerSimulationsStatistics", "StrategyName");
            RenameTable(name: "dbo.PlayerSimulationsStatistics", newName: "ScenarioPlayerResults");
        }
    }
}
