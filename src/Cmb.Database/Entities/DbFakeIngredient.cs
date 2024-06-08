using System.ComponentModel.DataAnnotations;

namespace Cmb.Database.Entities;

public class DbFakeIngredient
{
    [Key]
    public Guid Id { get; set; }

    public int Amount { get; set; }
}