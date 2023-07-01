﻿namespace MusicHub.Data.Models;

using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.ComponentModel.DataAnnotations;

public class Writer
{
    public Writer()
    {
        this.Songs = new HashSet<Song>();
    }

    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    public string Name { get; set; } = null!;

    public string? Pseudonym { get; set; }

    public virtual ICollection<Song> Songs { get; set; }
}
