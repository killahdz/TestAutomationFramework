namespace Core.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScenarioLog",
                c => new
                {
                    ScenarioLogId = c.Int(nullable: false, identity: true),
                    LogDateTime = c.DateTime(nullable: false),
                    Message = c.String(),
                    ScenarioRun_ScenarioRunID = c.Int(),
                })
                .PrimaryKey(t => t.ScenarioLogId)
                .ForeignKey("dbo.ScenarioRun", t => t.ScenarioRun_ScenarioRunID)
                .Index(t => t.ScenarioRun_ScenarioRunID);

            CreateTable(
                "dbo.ScenarioRun",
                c => new
                {
                    ScenarioRunID = c.Int(nullable: false, identity: true),
                    ScenarioID = c.Int(nullable: false),
                    ScenarioVariant = c.Int(nullable: false),
                    StartDateTime = c.DateTime(nullable: false),
                    EndDateTime = c.DateTime(nullable: false),
                    Host = c.String(),
                    Thread = c.String(),
                    FeatureName = c.String(),
                    ScenarioName = c.String(),
                    TestName = c.String(),
                    WebRoot = c.String(),
                    CountryCode = c.String(),
                    Culture = c.String(),
                    FeatureTags = c.String(),
                    ScenarioTags = c.String(),
                    DurationInSeconds = c.Double(nullable: false),
                    Passed = c.Boolean(nullable: false),
                    Exception = c.String(),
                })
                .PrimaryKey(t => t.ScenarioRunID);

            CreateTable(
                "dbo.ScenarioWait",
                c => new
                {
                    ScenarioWaitId = c.Int(nullable: false, identity: true),
                    WaitTimeInSeconds = c.Double(nullable: false),
                    StepName = c.String(),
                    ScenarioRun_ScenarioRunID = c.Int(),
                })
                .PrimaryKey(t => t.ScenarioWaitId)
                .ForeignKey("dbo.ScenarioRun", t => t.ScenarioRun_ScenarioRunID)
                .Index(t => t.ScenarioRun_ScenarioRunID);
        }

        public override void Down()
        {
            DropForeignKey("dbo.ScenarioWait", "ScenarioRun_ScenarioRunID", "dbo.ScenarioRun");
            DropForeignKey("dbo.ScenarioLog", "ScenarioRun_ScenarioRunID", "dbo.ScenarioRun");
            DropIndex("dbo.ScenarioWait", new[] { "ScenarioRun_ScenarioRunID" });
            DropIndex("dbo.ScenarioLog", new[] { "ScenarioRun_ScenarioRunID" });
            DropTable("dbo.ScenarioWait");
            DropTable("dbo.ScenarioRun");
            DropTable("dbo.ScenarioLog");
        }
    }
}