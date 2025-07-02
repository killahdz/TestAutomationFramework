using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Core.Library.Specflow;

namespace Core.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("AcceptanceTestStatsDb")
        {
        }

        public DbSet<ScenarioRun> ScenarioRuns { get; set; }
        public DbSet<ScenarioWait> ScenarioWaits { get; set; }
        public DbSet<ScenarioLog> ScenarioLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public static void LogScenarioRun(ScenarioRun scenarioRun)
        {
            using (var context = new DatabaseContext())
            {
                context.ScenarioRuns.Add(scenarioRun);
                context.SaveChanges();
            }
        }
    }
}