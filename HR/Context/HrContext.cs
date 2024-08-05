using HR.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace HR.Context
{
    public class HrContext : DbContext
    {
        public HrContext(DbContextOptions<HrContext> options) : base(options) { }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<StructuralDivision> StructuralDivisions { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder builder) =>
                builder
                .UseNpgsql("Host=localhost;Port=5433;Username=postgres;Password=postgres;Database=hr");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>()
                .HasOne(p => p.ChiefForStructuralDivision)
                .WithOne(s => s.PositionChief)
                .HasForeignKey<StructuralDivision>(p => p.PositionChiefId)
                .IsRequired(false);

            modelBuilder.Entity<StructuralDivision>()
                .HasMany(e => e.Positions)
                .WithOne(e => e.StructuralDivision)
                .HasForeignKey(e => e.DepartmentId)
                .IsRequired();
        }
    }
}



