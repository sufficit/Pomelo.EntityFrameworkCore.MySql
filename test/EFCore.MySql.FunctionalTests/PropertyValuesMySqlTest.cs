using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class PropertyValuesMySqlTest : PropertyValuesTestBase<PropertyValuesMySqlTest.PropertyValuesMySqlFixture>
    {
        public PropertyValuesMySqlTest(PropertyValuesMySqlFixture fixture)
            : base(fixture)
        {
        }

        // Complex collections require JSON column mapping support, which Pomelo does not yet implement.
        // The following tests exercise School.Departments or related complex collection functionality.

        public override void Using_complex_property_value_not_list_throws()
        {
        }

        public override Task Complex_collection_original_values_can_be_accessed_as_a_property_dictionary()
            => Task.CompletedTask;

        public override Task Complex_collection_current_values_can_be_accessed_as_a_property_dictionary()
            => Task.CompletedTask;

        public override void Setting_complex_collection_values_from_DTO_with_nulls_works()
        {
        }

        public override void Setting_complex_collection_values_from_object_works()
        {
        }

        public override void Setting_complex_collection_original_values_from_object_with_nulls_works()
        {
        }

        public override void Setting_complex_collection_current_values_from_object_with_nulls_works()
        {
        }

        public override void Setting_complex_collection_current_values_from_dictionary_works()
        {
        }

        public override void Setting_complex_collection_current_values_from_dictionary_with_nulls_works()
        {
        }

        public override void Setting_complex_collection_original_values_from_dictionary_with_nulls_works()
        {
        }

        public override void Setting_complex_collection_current_values_from_DTO_with_complex_metadata_access_works()
        {
        }

        public override void SetValues_throws_for_complex_collection_with_non_dictionary_item()
        {
        }

        public override void SetValues_throws_for_complex_collection_with_non_list_value()
        {
        }

        public override void SetValues_throws_for_nested_complex_collection_with_non_dictionary_item()
        {
        }

        public override void SetValues_throws_for_nested_complex_collection_with_non_list_value()
        {
        }

        public override void Current_values_can_be_cloned()
        {
        }

        public override void Original_values_can_be_cloned()
        {
        }

        public override void Current_values_can_be_copied_to_object_using_ToObject()
        {
        }

        public override void Original_values_can_be_copied_to_object_using_ToObject()
        {
        }

        public class PropertyValuesMySqlFixture : PropertyValuesFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

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
}
