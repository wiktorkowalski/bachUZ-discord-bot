using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BachUZ.Database
{
    public partial class BachuzContext : DbContext
    {
        public BachuzContext()
        {
        }

        public BachuzContext(DbContextOptions<BachuzContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Emotes> Emotes { get; set; }
        public virtual DbSet<Guilds> Guilds { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersGuilds> UsersGuilds { get; set; }
        public virtual DbSet<UsersOnVoice> UsersOnVoice { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = Environment.GetEnvironmentVariable("bachuzConnectionString");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new Exception("Environment Variable bachuzConnectionString is required!");
                }

                optionsBuilder.UseNpgsql(connectionString);

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Emotes>(entity =>
            {
                entity.HasKey(e => e.EmoteId)
                    .HasName("emotes_pk");

                entity.ToTable("emotes");

                entity.Property(e => e.EmoteId)
                    .HasColumnName("emote_id")
                    .HasColumnType("numeric(20,0)");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.GuildId)
                    .HasColumnName("guild_id")
                    .HasColumnType("numeric(20,0)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(32);

                entity.HasOne(d => d.Guild)
                    .WithMany(p => p.Emotes)
                    .HasForeignKey(d => d.GuildId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("emotes_guilds_guild_id_fk");
            });

            modelBuilder.Entity<Guilds>(entity =>
            {
                entity.HasKey(e => e.GuildId)
                    .HasName("guilds_pk");

                entity.ToTable("guilds");

                entity.Property(e => e.GuildId)
                    .HasColumnName("guild_id")
                    .HasColumnType("numeric(20,0)");

                entity.Property(e => e.CustomPrefix)
                    .HasColumnName("custom_prefix")
                    .HasMaxLength(6)
                    .HasDefaultValueSql("NULL::character varying");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("users_pk");

                entity.ToTable("users");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("numeric(20,0)");

                entity.Property(e => e.LastUsedDaily)
                    .HasColumnName("last_used_daily")
                    .HasColumnType("date");

                entity.Property(e => e.Points).HasColumnName("points");
            });

            modelBuilder.Entity<UsersGuilds>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.GuildId })
                    .HasName("users_guilds_pk");

                entity.ToTable("users_guilds");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("numeric(20,0)");

                entity.Property(e => e.GuildId)
                    .HasColumnName("guild_id")
                    .HasColumnType("numeric(20,0)");

                entity.HasOne(d => d.Guild)
                    .WithMany(p => p.UsersGuilds)
                    .HasForeignKey(d => d.GuildId)
                    .HasConstraintName("users_guilds_guilds_guild_id_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersGuilds)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("users_guilds_users_user_id_fk");
            });

            modelBuilder.Entity<UsersOnVoice>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.VoiceChannelId, e.GuildId })
                    .HasName("users_on_voice_pk");

                entity.ToTable("users_on_voice");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("numeric(20,0)");

                entity.Property(e => e.VoiceChannelId)
                    .HasColumnName("voice_channel_id")
                    .HasColumnType("numeric(20,0)");

                entity.Property(e => e.GuildId)
                    .HasColumnName("guild_id")
                    .HasColumnType("numeric(20,0)");

                entity.Property(e => e.Time).HasColumnName("time");

                entity.HasOne(d => d.Guild)
                    .WithMany(p => p.UsersOnVoice)
                    .HasForeignKey(d => d.GuildId)
                    .HasConstraintName("users_on_voice_guilds_guild_id_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersOnVoice)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("users_on_voice_users_user_id_fk");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
