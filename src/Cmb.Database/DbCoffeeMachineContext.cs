using Cmb.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cmb.Database;

public class DbCoffeeMachineContext(DbContextOptions<DbCoffeeMachineContext> options) : DbContext(options)
{
    public const string DatabaseName = "FakeStore"; 
    
    public DbSet<DbFakeIngredient> Ingredients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}