using System.ComponentModel.DataAnnotations;

namespace Cmb.Database.Entities;

public class DbIngredient : DbEntity
{
    [Key]
    public int Id { get; set; }

    [MaxLength(128)]
    public string Name { get; set; }
}