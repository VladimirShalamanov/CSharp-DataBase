namespace P01_StudentSystem.Data.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Student
{
    public Student()
    {
        this.StudentsCourses = new HashSet<Course>();
        this.Homeworks = new HashSet<Homework>();
    }

    //[Key]
    public int StudentId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Column(TypeName = "char(10)")]
    public string PhoneNumber { get; set; }

    public DateTime RegisteredOn { get; set; }

    public DateTime? Birthday { get; set; }

    public virtual ICollection<Course> StudentsCourses { get; set; }
    public virtual ICollection<Homework> Homeworks{ get; set; }
}
