using System;
using System.Collections.Generic;
using Gamificacion_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Gamificacion_API.Data;

public partial class BdgamificacionContext : DbContext
{
    public BdgamificacionContext()
    {
    }

    public BdgamificacionContext(DbContextOptions<BdgamificacionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcademicUnity> AcademicUnities { get; set; }

    public virtual DbSet<Achievement> Achievements { get; set; }

    public virtual DbSet<Admnistrator> Admnistrators { get; set; }

    public virtual DbSet<Badge> Badges { get; set; }

    public virtual DbSet<BadgeImage> BadgeImages { get; set; }

    public virtual DbSet<BadgeStudent> BadgeStudents { get; set; }

    public virtual DbSet<Career> Careers { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Gestor> Gestors { get; set; }

    public virtual DbSet<Rank> Ranks { get; set; }

    public virtual DbSet<Sanction> Sanctions { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentAchievement> StudentAchievements { get; set; }

    public virtual DbSet<TypeAchievement> TypeAchievements { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LUCIANAPC\\SQLEXPRESS ;Initial Catalog=BDGamificacion ; user=sa; password=Univalle ;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcademicUnity>(entity =>
        {
            entity.HasKey(e => e.IdAcademicUnity);

            entity.ToTable("AcademicUnity");

            entity.Property(e => e.IdAcademicUnity).HasColumnName("ID_AcademicUnity");
            entity.Property(e => e.AcademicUnityName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.IdAchievement);

            entity.Property(e => e.IdAchievement).HasColumnName("ID_achievement");
            entity.Property(e => e.IdAdministrator).HasColumnName("idAdministrator");
            entity.Property(e => e.IdGestor).HasColumnName("idGestor");
            entity.Property(e => e.IdTypeAchievement).HasColumnName("idTypeAchievement");
            entity.Property(e => e.NameAchievemt)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nameAchievemt");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("projectName");
            entity.Property(e => e.Punctuation).HasColumnName("punctuation");

            entity.HasOne(d => d.IdAdministratorNavigation).WithMany(p => p.Achievements)
                .HasForeignKey(d => d.IdAdministrator)
                .HasConstraintName("FK_Achievements_Admnistrator");

            entity.HasOne(d => d.IdGestorNavigation).WithMany(p => p.Achievements)
                .HasForeignKey(d => d.IdGestor)
                .HasConstraintName("FK_Achievements_Gestor1");

            entity.HasOne(d => d.IdTypeAchievementNavigation).WithMany(p => p.Achievements)
                .HasForeignKey(d => d.IdTypeAchievement)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Achievements_TypeAchievement");
        });

        modelBuilder.Entity<Admnistrator>(entity =>
        {
            entity.HasKey(e => e.IdAdministrator);

            entity.ToTable("Admnistrator");

            entity.Property(e => e.IdAdministrator)
                .ValueGeneratedNever()
                .HasColumnName("ID_administrator");
            entity.Property(e => e.FirstName)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("lastName");

            entity.HasOne(d => d.IdAdministratorNavigation).WithOne(p => p.Admnistrator)
                .HasForeignKey<Admnistrator>(d => d.IdAdministrator)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admnistrator_Usuario");
        });

        modelBuilder.Entity<Badge>(entity =>
        {
            entity.HasKey(e => e.IdBadge);

            entity.ToTable("Badge");

            entity.Property(e => e.IdBadge).HasColumnName("ID_badge");
            entity.Property(e => e.BadgeLevel)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.BadgeName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("badgeName");
            entity.Property(e => e.IdAdministrator).HasColumnName("idAdministrator");
            entity.Property(e => e.IdBadgeImage).HasColumnName("id_badgeImage");
            entity.Property(e => e.IdTypeAchivement).HasColumnName("idTypeAchivement");
            entity.Property(e => e.Points).HasColumnName("points");
            entity.Property(e => e.ImagePath)
              .HasMaxLength(150)
              .IsUnicode(false)
              .HasColumnName("imagePath");

            entity.HasOne(d => d.IdAdministratorNavigation).WithMany(p => p.Badges)
                .HasForeignKey(d => d.IdAdministrator)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Badge_Admnistrator");

            entity.HasOne(d => d.IdBadgeImageNavigation).WithMany(p => p.Badges)
                .HasForeignKey(d => d.IdBadgeImage)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Badge_Badge_Image");

            entity.HasOne(d => d.IdTypeAchivementNavigation).WithMany(p => p.Badges)
                .HasForeignKey(d => d.IdTypeAchivement)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Badge_TypeAchievement");

            modelBuilder.Entity<Badge>()
         .HasOne(b => b.NextLevelBadge)
         .WithMany() // Si no hay una colección inversa, puedes dejarlo vacío.
         .HasForeignKey(b => b.NextLevelBadgeId)
         .OnDelete(DeleteBehavior.Restrict);


        });

        modelBuilder.Entity<BadgeImage>(entity =>
        {
            entity.HasKey(e => e.IdBadgeImage);

            entity.ToTable("Badge_Image");

            entity.Property(e => e.IdBadgeImage).HasColumnName("ID_badgeImage");
            entity.Property(e => e.NameImage)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nameImage");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("title");
        });

        modelBuilder.Entity<BadgeStudent>(entity =>
        {
            entity.HasKey(e => e.IdBadgeStudent);

            entity.ToTable("BadgeStudent");

            entity.Property(e => e.IdBadgeStudent).HasColumnName("ID_badgeStudent");
            entity.Property(e => e.AccumulatedPoints).HasColumnName("accumulatedPoints");
            entity.Property(e => e.IdBadge).HasColumnName("idBadge");
            entity.Property(e => e.IdStudent).HasColumnName("idStudent");

            entity.HasOne(d => d.IdBadgeNavigation).WithMany(p => p.BadgeStudents)
                .HasForeignKey(d => d.IdBadge)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BadgeStudent_Badge");

            entity.HasOne(d => d.IdStudentNavigation).WithMany(p => p.BadgeStudents)
                .HasForeignKey(d => d.IdStudent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BadgeStudent_Student");
        });

        modelBuilder.Entity<Career>(entity =>
        {
            entity.HasKey(e => e.IdCareer);

            entity.ToTable("Career");

            entity.HasIndex(e => e.IdDepartment, "IX_Career_DepartmentID");

            entity.Property(e => e.IdCareer).HasColumnName("ID_Career");
            entity.Property(e => e.CareerName)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("careerName");
            entity.Property(e => e.IdDepartment).HasColumnName("idDepartment");

            entity.HasOne(d => d.IdDepartmentNavigation).WithMany(p => p.Careers)
                .HasForeignKey(d => d.IdDepartment)
                .HasConstraintName("FK_Career_Department_DepartmentID");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.IdDepartment);

            entity.ToTable("Department");

            entity.HasIndex(e => e.IdFaculty, "IX_Department_FacultyID");

            entity.Property(e => e.IdDepartment).HasColumnName("ID_Department");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(75)
                .IsUnicode(false);
            entity.Property(e => e.IdFaculty).HasColumnName("idFaculty");

            entity.HasOne(d => d.IdFacultyNavigation).WithMany(p => p.Departments)
                .HasForeignKey(d => d.IdFaculty)
                .HasConstraintName("FK_Department_Faculty_FacultyID");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.IdFaculty);

            entity.ToTable("Faculty");

            entity.Property(e => e.IdFaculty).HasColumnName("ID_Faculty");
            entity.Property(e => e.FacultyName)
                .HasMaxLength(75)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Gestor>(entity =>
        {
            entity.HasKey(e => e.IdGestor);

            entity.ToTable("Gestor");

            entity.Property(e => e.IdGestor)
                .ValueGeneratedNever()
                .HasColumnName("ID_gestor");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lastName");
            entity.Property(e => e.IsActive)
                  .HasColumnName("IsActive")
                  .HasColumnType("bit")
                  .HasDefaultValueSql("1");

            entity.HasOne(d => d.IdGestorNavigation).WithOne(p => p.Gestor)
                .HasForeignKey<Gestor>(d => d.IdGestor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gestor_Usuario");
        });

        modelBuilder.Entity<Rank>(entity =>
        {
            entity.HasKey(e => e.IdRank);

            entity.ToTable("Rank");

            entity.Property(e => e.IdRank).HasColumnName("ID_rank");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("imagePath");
            entity.Property(e => e.NameRank)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nameRank");
            entity.Property(e => e.NameSubrank)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nameSubrank");
        });

        modelBuilder.Entity<Sanction>(entity =>
        {
            entity.HasKey(e => e.IdSanctions);

            entity.Property(e => e.IdSanctions).HasColumnName("ID_Sanctions");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IdAdministrator).HasColumnName("idAdministrator");
            entity.Property(e => e.IdGestor).HasColumnName("idGestor");
            entity.Property(e => e.IdStudent).HasColumnName("idStudent");
            entity.Property(e => e.Sanction1).HasColumnName("sanction");

            entity.HasOne(d => d.IdAdministratorNavigation).WithMany(p => p.Sanctions)
                .HasForeignKey(d => d.IdAdministrator)
                .HasConstraintName("FK_Sanctions_Admnistrator");

            entity.HasOne(d => d.IdGestorNavigation).WithMany(p => p.Sanctions)
                .HasForeignKey(d => d.IdGestor)
                .HasConstraintName("FK_Sanctions_Gestor1");

            entity.HasOne(d => d.IdStudentNavigation).WithMany(p => p.Sanctions)
                .HasForeignKey(d => d.IdStudent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sanctions_Student");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.IdStudent);

            entity.ToTable("Student");

            entity.Property(e => e.IdStudent)
                .ValueGeneratedNever()
                .HasColumnName("ID_student");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("firstName");
            entity.Property(e => e.IdRank).HasColumnName("idRank");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lastName");
            entity.Property(e => e.Score).HasColumnName("score");

            entity.HasOne(d => d.IdRankNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.IdRank)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Rank");

            entity.HasOne(d => d.IdStudentNavigation).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.IdStudent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Usuario");
        });

        modelBuilder.Entity<StudentAchievement>(entity =>
        {
            entity.HasKey(e => e.IdStudentAchievement);

            entity.ToTable("Student_Achievement");

            entity.Property(e => e.IdStudentAchievement).HasColumnName("ID_studentAchievement");
            entity.Property(e => e.StudentPoints).HasColumnName("studentPoints");

            entity.HasOne(d => d.IdAchievementNavigation).WithMany(p => p.StudentAchievements)
                .HasForeignKey(d => d.IdAchievement)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Achievement_Achievements");

            entity.HasOne(d => d.IdStudentNavigation).WithMany(p => p.StudentAchievements)
                .HasForeignKey(d => d.IdStudent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Achievement_Student");
        });

        modelBuilder.Entity<TypeAchievement>(entity =>
        {
            entity.HasKey(e => e.IdTypeAchievement);

            entity.ToTable("TypeAchievement");

            entity.Property(e => e.IdTypeAchievement).HasColumnName("ID_typeAchievement");
            entity.Property(e => e.IdAdministrator).HasColumnName("idAdministrator");
            entity.Property(e => e.NameTypeAchievement)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nameTypeAchievement");
            entity.Property(e => e.Route)
                .IsUnicode(false)
                .HasColumnName("route");

            entity.HasOne(d => d.IdAdministratorNavigation).WithMany(p => p.TypeAchievements)
                .HasForeignKey(d => d.IdAdministrator)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TypeAchievement_Admnistrator");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("ID_usuario");
            entity.Property(e => e.Email)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                   .HasColumnName("IsActive")
                   .HasColumnType("bit")
                   .HasDefaultValueSql("1");
            entity.Property(e => e.IdAcademicUnity).HasColumnName("idAcademicUnity");
            entity.Property(e => e.IdCareer).HasColumnName("idCareer");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Rol).HasColumnName("rol");

            entity.HasOne(d => d.IdAcademicUnityNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdAcademicUnity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_AcademicUnity");

            entity.HasOne(d => d.IdCareerNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdCareer)
                .HasConstraintName("FK_Usuario_Career");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
