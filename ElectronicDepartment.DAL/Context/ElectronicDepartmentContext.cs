using ElectronicDepartment.Common.Enums;
using ElectronicDepartment.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectronicDepartment.DAL.Context;

public class ElectronicDepartmentContext(DbContextOptions<ElectronicDepartmentContext> options) : DbContext(options)
{
    public DbSet<Department> Departments { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<StudentSubject> StudentSubjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Конфігурація Admin
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("Admins");
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Конфігурація Manager
        modelBuilder.Entity<Manager>(entity =>
        {
            entity.ToTable("Managers");
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Конфігурація Teacher
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teachers");
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Конфігурація Student
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Конфігурація Department
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.Property(d => d.Name).HasMaxLength(200).IsRequired();
        });

        // Конфігурація Group
        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("Groups");
            entity.Property(g => g.Name).HasMaxLength(100).IsRequired();

            entity.HasOne(g => g.Department)
                .WithMany(d => d.Groups)
                .HasForeignKey(g => g.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Конфігурація Subject
        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subjects");
            entity.Property(s => s.Name).HasMaxLength(200).IsRequired();

            entity.HasOne(s => s.Department)
                .WithMany(d => d.Subjects)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Group)
                .WithMany(g => g.Subjects)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Teacher)
                .WithMany(t => t.Subjects)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Конфігурація StudentSubject
        modelBuilder.Entity<StudentSubject>(entity =>
        {
            entity.ToTable("StudentSubjects");
            entity.HasKey(ss => new { ss.StudentId, ss.SubjectId });

            entity.HasOne(ss => ss.Student)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(ss => ss.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ss => ss.Subject)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(ss => ss.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}