namespace BlackjackSimulator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add3NewCols : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScenarioPlayerResults",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ScenarioName = c.String(),
                        AverageBet = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartingCash = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RunCount = c.Int(nullable: false),
                        AverageCountOfHandsUntilBroke = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AverageMoneyLostPerHand = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LossRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WonHandsPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LostHandsPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PushHandsPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ScenarioPlayerResults");
        }
    }
}
