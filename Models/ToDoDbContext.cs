using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ToDoAPI.Models;

public partial class ToDoDbContext : DbContext
{
    public ToDoDbContext()
    {
    }

    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activities> Activities { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;user=phupas;password=364290;database=ToDo", Microsoft.EntityFrameworkCore.ServerVersion.Parse("11.1.2-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Activities>(entity =>
        {
            entity.HasKey(e => e.IdActivity).HasName("PRIMARY");

            entity.HasIndex(e => e.IdActivity, "idActivity_UNIQUE").IsUnique();

            entity.HasIndex(e => e.IdUser, "isUser_idx");

            entity.Property(e => e.IdActivity)
                .HasColumnType("int(11) unsigned")
                .HasColumnName("idActivity");
            entity.Property(e => e.IdUser)
                .HasMaxLength(13)
                .HasColumnName("idUser");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.When).HasColumnType("datetime");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Activities)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("isUser");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PRIMARY");

            entity.HasIndex(e => e.IdUser, "idUser_UNIQUE").IsUnique();

            entity.Property(e => e.IdUser)
                .HasMaxLength(13)
                .HasColumnName("idUser");
            entity.Property(e => e.Password).HasMaxLength(44);
            entity.Property(e => e.Salt).HasMaxLength(24);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
