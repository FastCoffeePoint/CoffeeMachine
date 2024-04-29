using Cmb.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cmb.Database;

public class DbCoffeeMachineContext(DbContextOptions<DbCoffeeMachineContext> options) : DbContext(options)
{
    public DbSet<DbIngredient> Ingredients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (Database.IsSqlite()) 
        {
            // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
            // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
            // Solution: https://nitratine.net/blog/post/a-warning-for-ef-cores-datetimeoffsettobinaryconverter/
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?));
                
                foreach (var property in properties)
                {
                    modelBuilder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(new DateTimeOffsetToUtcDateTimeTicksConverter());
                }
            }
        }
    }
}