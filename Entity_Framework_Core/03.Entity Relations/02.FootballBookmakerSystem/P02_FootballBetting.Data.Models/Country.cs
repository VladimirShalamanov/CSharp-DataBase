namespace P02_FootballBetting.Data.Models;

using P02_FootballBetting.Data.Common;
using System.ComponentModel.DataAnnotations;

public class Country
{
    public Country()
    {
        this.Towns = new HashSet<Town>();
    }
    [Key]
    public int CountryId { get; set; }

    [Required]
    [MaxLength(ValidationConstants.CountryNameMaxLength)]
    public string Name { get; set; } = null!;

    public virtual ICollection<Town> Towns { get; set;}
}
