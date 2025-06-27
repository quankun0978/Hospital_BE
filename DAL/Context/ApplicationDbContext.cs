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
        public DbSet<DoctorClinicSpecialty> DoctorClinicSpecialties { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<ClinicImage> ClinicImages { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Article> Articles { get; set; }


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
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");

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

            // Cấu hình UTF-8 cho Markdown
            modelBuilder.Entity<Markdown>()
                .Property(m => m.ContentHTML)
                .HasColumnType("NVARCHAR(MAX)")
                .IsUnicode(true);

            modelBuilder.Entity<Markdown>()
                .Property(m => m.ContentMarkdown)
                .HasColumnType("NVARCHAR(MAX)")
                .IsUnicode(true);

            modelBuilder.Entity<Markdown>()
                .Property(m => m.Description)
                .HasColumnType("NVARCHAR(MAX)")
                .IsUnicode(true);

            // Cấu hình bảng Specialty
            modelBuilder.Entity<Specialty>()
                .HasKey(s => s.SpecialtyId);

            // Cấu hình bảng DoctorClinicSpecialty
            modelBuilder.Entity<DoctorClinicSpecialty>()
                .HasKey(dcs => dcs.Id);

            // Quan hệ: DoctorClinicSpecialty - DoctorInfo
            modelBuilder.Entity<DoctorClinicSpecialty>()
                .HasOne(dcs => dcs.Doctor)
                .WithMany()
                .HasForeignKey(dcs => dcs.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ: DoctorClinicSpecialty - Clinic
            modelBuilder.Entity<DoctorClinicSpecialty>()
                .HasOne(dcs => dcs.Clinic)
                .WithMany()
                .HasForeignKey(dcs => dcs.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ: DoctorClinicSpecialty - Specialty
            modelBuilder.Entity<DoctorClinicSpecialty>()
                .HasOne(dcs => dcs.Specialty)
                .WithMany()
                .HasForeignKey(dcs => dcs.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Đảm bảo không bị trùng dữ liệu bác sĩ - cơ sở - chuyên khoa
            modelBuilder.Entity<DoctorClinicSpecialty>()
                .HasIndex(dcs => new { dcs.DoctorId, dcs.ClinicId, dcs.SpecialtyId })
                .IsUnique();
            // Quan hệ ClinicImage - Clinic
            modelBuilder.Entity<ClinicImage>()
                .HasOne(ci => ci.Clinic)
                .WithMany(c => c.ClinicImages)
                .HasForeignKey(ci => ci.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình bảng Appointment
            modelBuilder.Entity<Appointment>()
                .HasKey(a => a.AppointmentId);

            // Quan hệ: Appointment - PatientRecord
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ: Appointment - User (Doctor)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ràng buộc cho Status
            modelBuilder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasMaxLength(1);

            // Index cho tìm kiếm nhanh
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.DoctorId, a.AppointmentDate });

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => a.PatientId);

            // Cấu hình bảng Schedule
            modelBuilder.Entity<Schedule>()
                .HasKey(s => s.Id);

            // Quan hệ: Schedule - User (Doctor)
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Doctor)
                .WithMany()
                .HasForeignKey(s => s.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ: Schedule - Allcode (TimeType)
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.TimeTypeAllcode)
                .WithMany(a => a.ScheduleTimeTypes)
                .HasForeignKey(s => s.TimeType)
                .HasPrincipalKey(a => a.CodeKey)
                .OnDelete(DeleteBehavior.Restrict);

            // Index để tránh trùng lịch khám
            modelBuilder.Entity<Schedule>()
                .HasIndex(s => new { s.DoctorId, s.Date, s.TimeType })
                .IsUnique();

            // Cấu hình bảng Article
            modelBuilder.Entity<Article>()
                .HasKey(a => a.ArticleId);

            // Cấu hình UTF-8 cho các trường text
            modelBuilder.Entity<Article>()
                .Property(a => a.Title)
                .HasMaxLength(255)
                .IsUnicode(true);

            modelBuilder.Entity<Article>()
                .Property(a => a.Slug)
                .HasMaxLength(500)
                .IsUnicode(true);

            modelBuilder.Entity<Article>()
                .Property(a => a.Description)
                .HasColumnType("NVARCHAR(MAX)")
                .IsUnicode(true);

            modelBuilder.Entity<Article>()
                .Property(a => a.ContentHtml)
                .HasColumnType("NVARCHAR(MAX)")
                .IsUnicode(true);

            modelBuilder.Entity<Article>()
                .Property(a => a.Content)
                .HasColumnType("NVARCHAR(MAX)")
                .IsUnicode(true);

            modelBuilder.Entity<Article>()
                .Property(a => a.Category)
                .HasMaxLength(100)
                .IsUnicode(true);

            // Quan hệ: Article - User (Author)
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Author)
                .WithMany()
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index cho Article
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.Slug)
                .IsUnique()
                .HasFilter("[Slug] IS NOT NULL");

            modelBuilder.Entity<Article>()
                .HasIndex(a => a.PublishedAt);

            modelBuilder.Entity<Article>()
                .HasIndex(a => a.Category)
                .HasFilter("[Category] IS NOT NULL");

            base.OnModelCreating(modelBuilder);

        }
    }
} 