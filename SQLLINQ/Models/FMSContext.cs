using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace SQLLINQ.Models
{
    public partial class FMSContext : DbContext
    {
        public FMSContext()
        {
        }

        public FMSContext(DbContextOptions<FMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<CompanyFile> CompanyFile { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                // optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=FMS;Trusted_Connection=True;");
                var setting = Newtonsoft.Json.JsonConvert
                    .DeserializeObject<SQLLINQ.SettingsClass>(System.IO.File.ReadAllText("settings.json"));
                    optionsBuilder.UseSqlServer(setting.SqlConnectionString);
                // var setting = Newtonsoft.Json.JsonConvert
                //     .DeserializeObject<SQLLINQ.SettingsClass>(System.IO.File.ReadAllText("settings.json"));
                //     optionsBuilder.UseMySql(setting.SqlConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.CompanyLogo).IsUnicode(false);

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.ContactNumber)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CompanyFile>(entity =>
            {
                entity.HasKey(e => e.FileId)
                    .HasName("PK_File");

                entity.Property(e => e.FileId).HasColumnName("FileID");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.FileType).HasDefaultValueSql("((-1))");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.RawFileName)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Size).HasDefaultValueSql("((-1))");

                entity.Property(e => e.UploadDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
