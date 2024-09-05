using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Databases.CinemaDB;

public partial class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cast> Cast { get; set; }

    public virtual DbSet<Cinemas> Cinemas { get; set; }

    public virtual DbSet<Director> Director { get; set; }

    public virtual DbSet<Genre> Genre { get; set; }

    public virtual DbSet<Images> Images { get; set; }

    public virtual DbSet<Language> Language { get; set; }

    public virtual DbSet<Movies> Movies { get; set; }

    public virtual DbSet<MoviesCast> MoviesCast { get; set; }

    public virtual DbSet<MoviesGenre> MoviesGenre { get; set; }

    public virtual DbSet<MoviesLanguage> MoviesLanguage { get; set; }

    public virtual DbSet<MoviesRegion> MoviesRegion { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Region> Region { get; set; }

    public virtual DbSet<Screen> Screen { get; set; }

    public virtual DbSet<ScreenType> ScreenType { get; set; }

    public virtual DbSet<Sessions> Sessions { get; set; }

    public virtual DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_520_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Cast>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cast");

            entity.HasIndex(e => e.Uuid, "uuid_unq").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.CastName)
                .HasMaxLength(50)
                .HasColumnName("cast_name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<Cinemas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cinemas");

            entity.HasIndex(e => e.Uuid, "uuid_unq").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .HasColumnName("address");
            entity.Property(e => e.CinemaName)
                .HasMaxLength(50)
                .HasColumnName("cinema_name");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Status)
                .HasComment("0 - đang khóa, 1 - hoạt động")
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<Director>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("director");

            entity.HasIndex(e => e.Uuid, "uuid_unq").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DirectorName)
                .HasMaxLength(50)
                .HasColumnName("director_name");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("genre");

            entity.HasIndex(e => e.Uuid, "uuid_unq").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.GenreName)
                .HasMaxLength(50)
                .HasColumnName("genre_name");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<Images>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("images");

            entity.HasIndex(e => e.OwnerUuid, "fk_img_news_ref");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20)")
                .HasColumnName("id");
            entity.Property(e => e.OwnerType)
                .HasMaxLength(50)
                .HasColumnName("owner_type");
            entity.Property(e => e.OwnerUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("owner_uuid");
            entity.Property(e => e.Path)
                .HasMaxLength(255)
                .HasColumnName("path");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Type)
                .HasComment("1-Avatar, 2-poster, 3-Image")
                .HasColumnType("tinyint(4)")
                .HasColumnName("type");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("language");

            entity.HasIndex(e => e.Uuid, "uuid_unq").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.LanguageType)
                .HasComment("1-Phụ đề,2-Lồng tiếng")
                .HasColumnType("tinyint(4)")
                .HasColumnName("language_type");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<Movies>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("movies");

            entity.HasIndex(e => e.DirectorUuid, "fk_director_mv_ref");

            entity.HasIndex(e => e.Uuid, "uuid_unq").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AverageReview).HasColumnName("average_review");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DirectorUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("director_uuid");
            entity.Property(e => e.Duration)
                .HasColumnType("int(11)")
                .HasColumnName("duration");
            entity.Property(e => e.EngTitle)
                .HasMaxLength(50)
                .HasColumnName("eng_title");
            entity.Property(e => e.Format)
                .HasComment("0-Chiếu sớm, 1-Chiếu thường")
                .HasColumnType("tinyint(4)")
                .HasColumnName("format");
            entity.Property(e => e.Rated)
                .HasColumnType("int(11)")
                .HasColumnName("rated");
            entity.Property(e => e.RealeaseDate)
                .HasColumnType("datetime")
                .HasColumnName("realease_date");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
            entity.Property(e => e.Trailer)
                .HasMaxLength(255)
                .HasColumnName("trailer");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("uuid");

            entity.HasOne(d => d.DirectorUu).WithMany(p => p.Movies)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.DirectorUuid)
                .HasConstraintName("fk_director_mv_ref");
        });

        modelBuilder.Entity<MoviesCast>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("movies_cast");

            entity.HasIndex(e => e.CastUuid, "fk_cast_uuid_ref");

            entity.HasIndex(e => e.MoviesUuid, "fk_movies_cast_uuid_ref");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.CastUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("cast_uuid");
            entity.Property(e => e.MoviesUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("movies_uuid");

            entity.HasOne(d => d.CastUu).WithMany(p => p.MoviesCast)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.CastUuid)
                .HasConstraintName("fk_cast_uuid_ref");

            entity.HasOne(d => d.MoviesUu).WithMany(p => p.MoviesCast)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.MoviesUuid)
                .HasConstraintName("fk_movies_cast_uuid_ref");
        });

        modelBuilder.Entity<MoviesGenre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("movies_genre");

            entity.HasIndex(e => e.GenreUuid, "fk_genre_uuid_ref");

            entity.HasIndex(e => e.MoviesUuid, "fk_movies_genre_uuid_ref");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.GenreUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("genre_uuid");
            entity.Property(e => e.MoviesUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("movies_uuid");

            entity.HasOne(d => d.GenreUu).WithMany(p => p.MoviesGenre)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.GenreUuid)
                .HasConstraintName("fk_genre_uuid_ref");

            entity.HasOne(d => d.MoviesUu).WithMany(p => p.MoviesGenre)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.MoviesUuid)
                .HasConstraintName("fk_movies_mv_uuid_ref");
        });

        modelBuilder.Entity<MoviesLanguage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("movies_language");

            entity.HasIndex(e => e.LanguageUuid, "fk_lg_uuid_ref");

            entity.HasIndex(e => e.MoviesUuid, "fk_movies_lg_uuid_ref");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.LanguageUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("language_uuid");
            entity.Property(e => e.MoviesUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("movies_uuid");

            entity.HasOne(d => d.LanguageUu).WithMany(p => p.MoviesLanguage)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.LanguageUuid)
                .HasConstraintName("fk_lg_uuid_ref");

            entity.HasOne(d => d.MoviesUu).WithMany(p => p.MoviesLanguage)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.MoviesUuid)
                .HasConstraintName("fk_movies_lg_uuid_ref");
        });

        modelBuilder.Entity<MoviesRegion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("movies_region");

            entity.HasIndex(e => e.MoviesUuid, "fk_movies_region_uuid_ref");

            entity.HasIndex(e => e.RegionUuid, "fk_region_uuid_ref");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.MoviesUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("movies_uuid");
            entity.Property(e => e.RegionUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("region_uuid");

            entity.HasOne(d => d.MoviesUu).WithMany(p => p.MoviesRegion)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.MoviesUuid)
                .HasConstraintName("fk_movies_region_uuid_ref");

            entity.HasOne(d => d.RegionUu).WithMany(p => p.MoviesRegion)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.RegionUuid)
                .HasConstraintName("fk_region_uuid_ref");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("news");

            entity.HasIndex(e => e.Uuid, "uuid");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("region");

            entity.HasIndex(e => e.Uuid, "uuid_unq").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.RegionName)
                .HasMaxLength(50)
                .HasColumnName("region_name");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<Screen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("screen");

            entity.HasIndex(e => e.CinemaUuid, "fk_cinema_uuid_ref");

            entity.HasIndex(e => e.ScreenTypeUuid, "fk_stype_uuid_ref");

            entity.HasIndex(e => e.Uuid, "uuid_unq").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Capacity)
                .HasColumnType("int(11)")
                .HasColumnName("capacity");
            entity.Property(e => e.CinemaUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("cinema_uuid");
            entity.Property(e => e.ScreenName)
                .HasMaxLength(50)
                .HasColumnName("screen_name");
            entity.Property(e => e.ScreenTypeUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("screen_type_uuid");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");

            entity.HasOne(d => d.CinemaUu).WithMany(p => p.Screen)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.CinemaUuid)
                .HasConstraintName("fk_cinema_uuid_ref");

            entity.HasOne(d => d.ScreenTypeUu).WithMany(p => p.Screen)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.ScreenTypeUuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_stype_uuid_ref");
        });

        modelBuilder.Entity<ScreenType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("screen_type");

            entity.HasIndex(e => e.Uuid, "uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Type)
                .HasComment("1-2D,2-3D,3-IMAX")
                .HasColumnType("tinyint(4)")
                .HasColumnName("type");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<Sessions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("sessions");

            entity.HasIndex(e => e.UserUuid, "fk_ss_user_uid_ref");

            entity.HasIndex(e => e.Uuid, "uuid_unq").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20)")
                .HasColumnName("id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'1'")
                .HasComment("0: LogIn - 1: LogOut")
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeLogin)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_login");
            entity.Property(e => e.TimeLogout)
                .HasColumnType("timestamp")
                .HasColumnName("time_logout");
            entity.Property(e => e.UserUuid)
                .HasMaxLength(50)
                .HasColumnName("user_uuid");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");

            entity.HasOne(d => d.UserUu).WithMany(p => p.Sessions)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.UserUuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ss_user_uid_ref");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "email_unq").IsUnique();

            entity.HasIndex(e => e.Uuid, "uuid_unq").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(50)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender)
                .HasComment("0-Nam , 1-Nữ , 2 - khác")
                .HasColumnType("tinyint(4)")
                .HasColumnName("gender");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'1'")
                .HasComment("0-Admin, 1-Client")
                .HasColumnType("tinyint(4)")
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'1'")
                .HasComment("0 - đang khóa, 1 - hoạt động")
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
