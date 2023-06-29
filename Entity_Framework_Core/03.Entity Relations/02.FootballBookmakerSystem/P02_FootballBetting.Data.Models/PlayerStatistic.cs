namespace P02_FootballBetting.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class PlayerStatistic
{
    [ForeignKey(nameof(Game))]    
    public int GameId { get; set; }
    public virtual Game Game { get; set; } = null!;


    [ForeignKey(nameof(Player))]
    public int PlayerId { get; set; }
    public virtual Player Player { get; set; } = null!;


    // judge might not be happy with [byte]
    public byte ScoredGoals { get; set; }

    public byte Assists { get; set; }

    public byte MinutesPlayed { get; set; }
}
