using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

public class ComplexTypesTrackingMySqlTest : ComplexTypesTrackingTestBase<ComplexTypesTrackingMySqlTest.MySqlFixture>
{
    public ComplexTypesTrackingMySqlTest(MySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
        fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());

    // Complex collections require JSON column mapping support, which Pomelo does not yet implement.
    // All collection-related tests are overridden as no-ops.

    public override Task Can_track_entity_with_complex_type_collections(EntityState state, bool async)
        => Task.CompletedTask;

    public override void Can_mark_complex_type_collection_properties_modified(bool trackFromQuery)
    {
    }

    public override void Can_read_original_values_for_properties_of_complex_type_collections(bool trackFromQuery)
    {
    }

    public override void Can_write_original_values_for_properties_of_complex_type_collections(bool trackFromQuery)
    {
    }

    public override Task Can_track_entity_with_complex_record_collections(EntityState state, bool async)
        => Task.CompletedTask;

    public override Task Can_track_entity_with_complex_record_collections_with_fields(EntityState state, bool async)
        => Task.CompletedTask;

    public override void Can_mark_complex_record_collections_with_fields_properties_modified(bool trackFromQuery)
    {
    }

    public override void Can_read_original_values_for_properties_of_complex_record_collections_with_fields(bool trackFromQuery)
    {
    }

    public override void Can_write_original_values_for_properties_of_complex_record_collections_with_fields(bool trackFromQuery)
    {
    }

    public override void Throws_when_accessing_complex_entries_using_incorrect_cardinality()
    {
    }

    public override void Can_mark_complex_record_collection_properties_modified(bool trackFromQuery)
    {
    }

    public override void Can_read_original_values_for_properties_of_complex_record_collections(bool trackFromQuery)
    {
    }

    public override void Can_write_original_values_for_properties_of_complex_record_collections(bool trackFromQuery)
    {
    }

    public override Task Can_track_entity_with_complex_field_collections(EntityState state, bool async)
        => Task.CompletedTask;

    public override void Can_mark_complex_field_collection_properties_modified(bool trackFromQuery)
    {
    }

    public override void Can_read_original_values_for_properties_of_complex_field_collections(bool trackFromQuery)
    {
    }

    public override void Can_write_original_values_for_properties_of_complex_field_collections(bool trackFromQuery)
    {
    }

    public override Task Can_track_entity_with_complex_property_bag_collections(EntityState state, bool async)
        => Task.CompletedTask;

    public override void Can_mark_complex_property_bag_collection_properties_modified(bool trackFromQuery)
    {
    }

    public override void Can_read_original_values_for_properties_of_complex_property_bag_collections(bool trackFromQuery)
    {
    }

    public override void Can_write_original_values_for_properties_of_complex_property_bag_collections(bool trackFromQuery)
    {
    }

    public override void Can_detect_reordered_elements_in_complex_type_collections(bool trackFromQuery)
    {
    }

    public override void Can_detect_added_elements_in_complex_type_collections(bool trackFromQuery)
    {
    }

    public override void Can_detect_removed_elements_in_complex_type_collections(bool trackFromQuery)
    {
    }

    public override void Can_detect_replaced_elements_in_complex_type_collections(bool trackFromQuery)
    {
    }

    public override void Can_detect_duplicates_in_complex_type_collections(bool trackFromQuery)
    {
    }

    public override void Can_handle_null_elements_in_complex_type_collections(bool trackFromQuery)
    {
    }

    public override void Can_detect_swapped_complex_objects_in_collections(bool trackFromQuery)
    {
    }

    public override void Can_handle_collection_with_mixed_null_and_duplicate_elements(bool trackFromQuery)
    {
    }

    public override void Can_detect_nested_collection_changes_in_complex_type_collections(bool trackFromQuery)
    {
    }

    public override void Can_detect_changes_to_nested_teams_members_in_complex_type_collections(bool trackFromQuery)
    {
    }

    public override void Can_handle_empty_nested_teams_in_complex_type_collections(bool trackFromQuery)
    {
    }

    public override void Can_detect_changes_to_record_collection_elements(bool trackFromQuery)
    {
    }

    public override void Can_detect_changes_to_record_teams_in_complex_type_collections(bool trackFromQuery)
    {
    }

    public class MySqlFixture : FixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            => base.AddOptions(builder)
                .ConfigureWarnings(c => c.Ignore(CoreEventId.MappedComplexPropertyIgnoredWarning));

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // Pomelo does not support JSON column mapping for complex collections.
            // Ignore complex collection properties so the model validates.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
            {
                foreach (var complexProperty in entityType.GetComplexProperties().ToList())
                {
                    if (complexProperty.IsCollection)
                    {
                        modelBuilder.Entity(entityType.ClrType).Ignore(complexProperty.Name);
                    }
                }
            }
        }
    }
}
