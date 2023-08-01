﻿namespace Footballers.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Team
    {
        public Team()
        {
            this.TeamsFootballers = new HashSet<TeamFootballer>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Nationality { get; set; } = null!;

        [Required]
        public int Trophies { get; set; }


        public ICollection<TeamFootballer> TeamsFootballers { get; set; }
    }
}
