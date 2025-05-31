using Microsoft.EntityFrameworkCore;
using Hospital_BE.DAL.Models;

namespace Hospital_BE.DAL.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<DoctorInfo> DoctorInfos { get; set; }
        public DbSet<Allcode> Allcodes { get; set; }
        public DbSet<PatientRecord> PatientRecords { get; set; }
        public DbSet<Markdown> Markdowns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Khóa chính
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);
                
            modelBuilder.Entity<Clinic>()
                .HasKey(c => c.ClinicId);
                
            modelBuilder.Entity<DoctorInfo>()
                .HasKey(d => d.Id);
                
            modelBuilder.Entity<Allcode>()
                .HasKey(a => a.Id);
                
            modelBuilder.Entity<PatientRecord>()
                .HasKey(pr => pr.PatientId);
                
            // Khóa thay thế cho Allcode (alternate key)
            modelBuilder.Entity<Allcode>()
                .HasAlternateKey(a => a.CodeKey);
                
            // Index cho User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Phone)
                .IsUnique()
                .HasFilter("[Phone] IS NOT NULL");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique()
                .HasFilter("[Username] IS NOT NULL");
                
            // Index cho Allcode
            modelBuilder.Entity<Allcode>()
                .HasIndex(a => new { a.CodeType, a.CodeKey });
                
            // ---- CẤU HÌNH CÁC MỐI QUAN HỆ ----
            
            // Quan hệ: User - Role (User - Allcode)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(a => a.UserRoles)
                .HasForeignKey(u => u.RoleId)
                .HasPrincipalKey(a => a.CodeKey)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Quan hệ: DoctorInfo - User
            modelBuilder.Entity<DoctorInfo>()
                .HasOne(d => d.Doctor)
                .WithMany(u => u.DoctorInfos)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Quan hệ: DoctorInfo - Price (DoctorInfo - Allcode)
            modelBuilder.Entity<DoctorInfo>()
                .HasOne(d => d.Price)
                .WithMany(a => a.DoctorPrices)
                .HasForeignKey(d => d.PriceId)
                .HasPrincipalKey(a => a.CodeKey)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Quan hệ: DoctorInfo - Position (DoctorInfo - Allcode)
            modelBuilder.Entity<DoctorInfo>()
                .HasOne(d => d.Position)
                .WithMany(a => a.DoctorPositions)
                .HasForeignKey(d => d.PositionId)
                .HasPrincipalKey(a => a.CodeKey)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Quan hệ: DoctorInfo - Clinic
            modelBuilder.Entity<DoctorInfo>()
                .HasOne(d => d.Clinic)
                .WithMany(c => c.Doctors)
                .HasForeignKey(d => d.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ: PatientRecord - User
            modelBuilder.Entity<PatientRecord>()
                .HasOne(pr => pr.User)
                .WithMany()
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ: Markdown - User (Doctor) (nullable)
            modelBuilder.Entity<Markdown>()
                .HasOne(m => m.Doctor)
                .WithMany()
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ: Markdown - Clinic (nullable)
            modelBuilder.Entity<Markdown>()
                .HasOne(m => m.Clinic)
                .WithMany()
                .HasForeignKey(m => m.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
} 