using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ScratchMUD.Server.EntityFramework
{
    public partial class ScratchMUDContext : DbContext
    {
        public ScratchMUDContext()
        {
        }

        public ScratchMUDContext(DbContextOptions<ScratchMUDContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<AreaEditor> AreaEditor { get; set; }
        public virtual DbSet<AreaTranslation> AreaTranslation { get; set; }
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<ItemTranslation> ItemTranslation { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<Npc> Npc { get; set; }
        public virtual DbSet<NpcItem> NpcItem { get; set; }
        public virtual DbSet<NpcTranslation> NpcTranslation { get; set; }
        public virtual DbSet<PlayerCharacter> PlayerCharacter { get; set; }
        public virtual DbSet<PlayerCharacterItem> PlayerCharacterItem { get; set; }
        public virtual DbSet<Room> Room { get; set; }
        public virtual DbSet<RoomItem> RoomItem { get; set; }
        public virtual DbSet<RoomNpc> RoomNpc { get; set; }
        public virtual DbSet<RoomTranslation> RoomTranslation { get; set; }
        public virtual DbSet<World> World { get; set; }
        public virtual DbSet<WorldTranslation> WorldTranslation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Area>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByPlayer)
                    .WithMany(p => p.AreaCreatedByPlayer)
                    .HasForeignKey(d => d.CreatedByPlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Area_PlayerCharacter_CreatedBy");

                entity.HasOne(d => d.OwnerPlayer)
                    .WithMany(p => p.AreaOwnerPlayer)
                    .HasForeignKey(d => d.OwnerPlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Area_PlayerCharacter_Owner");

                entity.HasOne(d => d.World)
                    .WithMany(p => p.Area)
                    .HasForeignKey(d => d.WorldId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Area_World");
            });

            modelBuilder.Entity<AreaEditor>(entity =>
            {
                entity.HasKey(e => new { e.AreaId, e.PlayerCharacterId })
                    .HasName("PK_AreaEditor_AreaId_PlayerCharacterId");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.AreaEditor)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AreaEditor_Area");

                entity.HasOne(d => d.PlayerCharacter)
                    .WithMany(p => p.AreaEditor)
                    .HasForeignKey(d => d.PlayerCharacterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AreaEditor_PlayerCharacter");
            });

            modelBuilder.Entity<AreaTranslation>(entity =>
            {
                entity.HasKey(e => new { e.AreaId, e.LanguageId })
                    .HasName("PK_AreaTranslation_AreaId_LanguageId");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.AreaTranslation)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AreaTranslation_Area");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.AreaTranslation)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AreaTranslation_Language");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Item)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Item_Area");

                entity.HasOne(d => d.CreatedByPlayer)
                    .WithMany(p => p.Item)
                    .HasForeignKey(d => d.CreatedByPlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Item_PlayerCharacter");
            });

            modelBuilder.Entity<ItemTranslation>(entity =>
            {
                entity.HasKey(e => new { e.ItemId, e.LanguageId })
                    .HasName("PK_ItemTranslation_ItemId_LanguageId");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.FullDescription)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.ShortDescription)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemTranslation)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemTranslation_Item");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.ItemTranslation)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemTranslation_Language");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Npc>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Npc)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Npc_Area");

                entity.HasOne(d => d.CreatedByPlayer)
                    .WithMany(p => p.Npc)
                    .HasForeignKey(d => d.CreatedByPlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Npc_PlayerCharacter");
            });

            modelBuilder.Entity<NpcItem>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.NpcItem)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NpcItem_Item");

                entity.HasOne(d => d.Npc)
                    .WithMany(p => p.NpcItem)
                    .HasForeignKey(d => d.NpcId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NpcItem_Npc");
            });

            modelBuilder.Entity<NpcTranslation>(entity =>
            {
                entity.HasKey(e => new { e.NpcId, e.LanguageId })
                    .HasName("PK_NpcTranslation_NpcId_LanguageId");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.FullDescription)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.ShortDescription)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.NpcTranslation)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NpcTranslation_Language");

                entity.HasOne(d => d.Npc)
                    .WithMany(p => p.NpcTranslation)
                    .HasForeignKey(d => d.NpcId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NpcTranslation_Npc");
            });

            modelBuilder.Entity<PlayerCharacter>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.PlayerCharacter)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlayerCharacter_Room");

                entity.HasOne(d => d.World)
                    .WithMany(p => p.PlayerCharacter)
                    .HasForeignKey(d => d.WorldId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlayerCharacter_World");
            });

            modelBuilder.Entity<PlayerCharacterItem>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.PlayerCharacterItem)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlayerCharacterItem_Item");

                entity.HasOne(d => d.PlayerCharacter)
                    .WithMany(p => p.PlayerCharacterItem)
                    .HasForeignKey(d => d.PlayerCharacterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlayerCharacterItem_PlayerCharacter");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Room)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Room_Area");

                entity.HasOne(d => d.CreatedByPlayer)
                    .WithMany(p => p.RoomNavigation)
                    .HasForeignKey(d => d.CreatedByPlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Room_PlayerCharacter");
            });

            modelBuilder.Entity<RoomItem>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.RoomItem)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoomItem_Item");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomItem)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoomItem_Room");
            });

            modelBuilder.Entity<RoomNpc>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Npc)
                    .WithMany(p => p.RoomNpc)
                    .HasForeignKey(d => d.NpcId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoomNpc_Npc");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomNpc)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoomNpc_Room");
            });

            modelBuilder.Entity<RoomTranslation>(entity =>
            {
                entity.HasKey(e => new { e.RoomId, e.LanguageId })
                    .HasName("PK_RoomTranslation_RoomId_LanguageId");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.FullDescription)
                    .IsRequired()
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.ShortDescription)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.RoomTranslation)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoomTranslation_Language");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomTranslation)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoomTranslation_Room");
            });

            modelBuilder.Entity<World>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<WorldTranslation>(entity =>
            {
                entity.HasKey(e => new { e.WorldId, e.LanguageId })
                    .HasName("PK_WorldTranslation_WorldId_LanguageId");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.WorldTranslation)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorldTranslation_Language");

                entity.HasOne(d => d.World)
                    .WithMany(p => p.WorldTranslation)
                    .HasForeignKey(d => d.WorldId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorldTranslation_World");
            });
        }
    }
}
