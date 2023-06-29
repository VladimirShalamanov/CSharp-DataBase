﻿namespace P01_StudentSystem.Data;

using Microsoft.EntityFrameworkCore;

using P01_StudentSystem.Data.Models;

public class StudentSystemContext : DbContext
{
    public StudentSystemContext()
    {

    }

    public StudentSystemContext(DbContextOptions options)
        : base(options)
    {

    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Homework> Homeworks { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentCourse> StudentsCourses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=StudentSystem;Integrated Security=True;");
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StudentCourse>(e =>
        {
            e.HasKey(e => new { e.StudentId, e.CourseId });
        });
    }
}