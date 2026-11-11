using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            Run(args.Contains("--offline"), args.Contains("--synchronous-io")).GetAwaiter().GetResult();
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    private static async Task Run(bool offline, bool synchronousIO)
    {
        var assembly = typeof(MySqlDbContextOptionsExtensions).Assembly;
        Require(assembly.GetName().Version == new Version(3, 2, 7, 0), "Provider assembly identity changed.");
        Require(BitConverter.ToString(assembly.GetName().GetPublicKeyToken()).Replace("-", "").ToLowerInvariant() == "2cc498582444921b", "Provider signing key changed.");
        Require(assembly.GetManifestResourceNames().Contains("Pomelo.EntityFrameworkCore.MySql.Properties.MySqlStrings.resources"), "Provider resources are missing.");
        var source = offline ? "Server=127.0.0.1;Database=pomelo_compat_test_offline;GuidFormat=Binary16" :
            Environment.GetEnvironmentVariable("SUFFICIT_POMELO_COMPAT_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(source)) throw new InvalidOperationException("An isolated test connection is required.");
        var connection = new MySqlConnectionStringBuilder(source);
        Require(connection.Server == "127.0.0.1" && connection.Database.StartsWith("pomelo_compat_test_", StringComparison.Ordinal), "Only disposable loopback databases may be used.");
        var options = new DbContextOptionsBuilder<ProbeContext>().UseMySql(source).Options;
        using (var context = new ProbeContext(options))
        {
            Require(context.Model.FindEntityType(typeof(Probe)) != null, "Model creation failed.");
            Require(context.Database.GetDbConnection().GetType() == typeof(MySqlConnection), "Provider uses the wrong connector.");
            if (!synchronousIO)
            {
                var helpers = typeof(Microsoft.EntityFrameworkCore.Storage.RelationalConnection).Assembly
                    .GetType("System.Data.Common.DbConnectionExtensions", true);
                var close = helpers.GetMethod("CloseAsync", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                Require(close != null, "EF Core async connection helper is missing.");
                await (Task)close.Invoke(null, new object[] { context.Database.GetDbConnection() });
            }
            if (offline)
            {
                Console.WriteLine("PASS assembly identity, resources, model and connector; no connection opened.");
                return;
            }
            if (synchronousIO) context.Database.EnsureCreated();
            else await context.Database.EnsureCreatedAsync();
            var id = Guid.NewGuid();
            var stamp = new DateTime(2026, 11, 11, 12, 0, 0, DateTimeKind.Unspecified);
            context.Rows.Add(new Probe { Id = id, Name = "Ação çã 漢字", Amount = 123.4567m, Enabled = true, Stamp = stamp });
            if (synchronousIO) context.SaveChanges();
            else await context.SaveChangesAsync();
            var row = synchronousIO
                ? context.Rows.AsNoTracking().Single(x => x.Id == id && x.Enabled && x.Amount > 100)
                : await context.Rows.AsNoTracking().SingleAsync(x => x.Id == id && x.Enabled && x.Amount > 100);
            Require(row.Name == "Ação çã 漢字" && row.Amount == 123.4567m && row.Stamp == stamp, "Typed round trip failed.");
            using (var transaction = context.Database.BeginTransaction())
            {
                context.Rows.Add(new Probe { Id = Guid.NewGuid(), Name = "rollback", Stamp = stamp });
                context.SaveChanges();
                transaction.Rollback();
            }
            Require(context.Rows.AsNoTracking().Count() == 1, "Transaction rollback failed.");
            using (var transaction = context.Database.BeginTransaction())
            {
                var tracked = context.Rows.Single(x => x.Id == id);
                tracked.Name = null;
                tracked.Enabled = false;
                context.SaveChanges();
                transaction.Commit();
            }
            using (var verify = new ProbeContext(options))
            {
                var saved = verify.Rows.Single(x => x.Id == id);
                Require(saved.Name == null && !saved.Enabled, "Transaction commit/update failed.");
                verify.Rows.Remove(saved);
                verify.SaveChanges();
                Require(!verify.Rows.Any(), "Delete failed.");
            }
        }
        Console.WriteLine("PASS CRUD, Unicode, binary Guid, decimal, DateTime, bool, null, commit and rollback; I/O=" + (synchronousIO ? "sync" : "async"));
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed class ProbeContext : DbContext
    {
        public ProbeContext(DbContextOptions<ProbeContext> options) : base(options) { }
        public DbSet<Probe> Rows { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            var entity = builder.Entity<Probe>();
            entity.ToTable("connector_probe");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Amount).HasColumnType("decimal(18,4)");
            entity.Property(x => x.Stamp).HasColumnType("datetime(6)");
        }
    }

    private sealed class Probe
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public bool Enabled { get; set; }
        public DateTime Stamp { get; set; }
    }
}
