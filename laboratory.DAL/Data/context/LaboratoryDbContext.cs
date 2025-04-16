using laboratory.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using Section = laboratory.DAL.Models.Section;


namespace laboratory.DAL.Data.context
{
    public class LaboratoryDbContext : DbContext
    {
        public LaboratoryDbContext(DbContextOptions<LaboratoryDbContext> options) : base(options)
        {

        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Experiment> Experiments { get; set; } //
        public DbSet<Material> Materials { get; set; } //
        public DbSet<ExperimentMaterial> ExperimentMaterials { get; set; } //
        public DbSet<Section> Sections { get; set; }
        public DbSet<Group> Groups { get; set; } //
        public DbSet<Student> Students { get; set; }
        public DbSet<LabAdmin> LabAdmins { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);




            #region ensures that Email is unique in the database
            modelBuilder.Entity<Student>()
             .HasIndex(s => s.Email)
             .IsUnique();

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.Email)
                .IsUnique();

            modelBuilder.Entity<LabAdmin>()
                .HasIndex(a => a.Email)
                .IsUnique();

            #endregion


            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.Email)
                .IsUnique();



            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Student>()
            .HasOne(s => s.Group)
            .WithMany(g => g.Students) 
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Section>()
                .HasOne(r => r.Doctor)
                .WithMany()
                .HasForeignKey(r => r.DoctorId);      

            modelBuilder.Entity<Section>()
                .HasOne(r => r.Experiment)
                .WithMany()
                .HasForeignKey(r => r.ExperimentId);


             modelBuilder.Entity<Experiment>()
                .HasMany(s => s.Departments)
                .WithMany(c => c.Experiments)
                .UsingEntity(j => j.ToTable("DepartmentExperiment"));


            modelBuilder.Entity<Section>()
               .HasMany(s => s.Students)
               .WithMany(c => c.Sections)
               .UsingEntity(j => j.ToTable("StudentSection"));
            modelBuilder.Entity<Section>()
           .Ignore(s => s.AttendanceRecords);

            modelBuilder.Entity<Section>()
          .HasOne(s => s.Doctor)
          .WithMany(d => d.sections)
          .HasForeignKey(s => s.DoctorId)
          .OnDelete(DeleteBehavior.NoAction); // لا تقوم بحذف السجلات المرتبطة

            modelBuilder.Entity<Section>()
                .HasOne(s => s.Doctor)
                .WithMany(d => d.sections)
                .HasForeignKey(s => s.DoctorId)
                .OnDelete(DeleteBehavior.NoAction); // لا تقوم بحذف السجلات المرتبطة

            modelBuilder.Entity<Section>()
                .HasOne(s => s.Experiment)
                .WithMany(e => e.Sections)
                .HasForeignKey(s => s.ExperimentId)
                .OnDelete(DeleteBehavior.NoAction); // لا تقوم بحذف السجلات المرتبطة



        }
    }
}

