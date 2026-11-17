using KlikajZGlowa.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KlikajZGlowa.Database
{
    public class Db : IdentityDbContext<ApplicationUser>
    {
        private readonly string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Studia\Projekt Inżynierski\KlikajZGlowa\KlikajZGlowa.Database\Database.mdf"";Integrated Security=True";
        //ConnectionStringProvider.GetConnectionString(
        //    "Database.mdf",
        //    @"KlikajZGłowa.Database"
        //);
        public Db() { }
        public Db(DbContextOptions<Db> options) : base(options) { }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<EnrolledUser> EnrolledUsers { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<AuthorApplication> AuthorApplications { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlServer(_connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Author)
                .WithOne(a => a.ApplicationUser)
                .HasForeignKey<Author>(a => a.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(e => e.EnrolledUser)
                .WithOne(a => a.ApplicationUser)
                .HasForeignKey<EnrolledUser>(e => e.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuthorApplication>()
                .HasOne(a => a.ApplicationUser)
                .WithMany()
                .HasForeignKey(a => a.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Author>()
                .HasIndex(a => a.ApplicationUserId)
                .IsUnique();

            modelBuilder.Entity<EnrolledUser>()
                .HasIndex(e => e.ApplicationUserId)
                .IsUnique();
            
            modelBuilder.Entity<AuthorApplication>()
                .Property(a => a.Status)
                .HasConversion<string>();
        }
    }

    
}
