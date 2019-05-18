﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScratchMUD.Server.Models;

namespace ScratchMUD.Server.Migrations
{
    [DbContext(typeof(ScratchMUDContext))]
    partial class ScratchMUDContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ScratchMUD.Server.Models.Area", b =>
                {
                    b.Property<int>("AreaId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CreatedByPlayerId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.Property<int>("OwnerPlayerId");

                    b.Property<short>("VirtualNumber");

                    b.Property<short>("WorldId");

                    b.HasKey("AreaId");

                    b.HasIndex("CreatedByPlayerId");

                    b.HasIndex("OwnerPlayerId");

                    b.HasIndex("WorldId");

                    b.ToTable("Area");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.AreaEditor", b =>
                {
                    b.Property<int>("AreaId");

                    b.Property<int>("PlayerCharacterId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.HasKey("AreaId", "PlayerCharacterId")
                        .HasName("PK_AreaEditor_AreaId_PlayerCharacterId");

                    b.HasIndex("PlayerCharacterId");

                    b.ToTable("AreaEditor");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.AreaTranslation", b =>
                {
                    b.Property<int>("AreaId");

                    b.Property<short>("LanguageId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.HasKey("AreaId", "LanguageId")
                        .HasName("PK_AreaTranslation_AreaId_LanguageId");

                    b.HasIndex("LanguageId");

                    b.ToTable("AreaTranslation");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AreaId");

                    b.Property<int>("CreatedByPlayerId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.Property<short>("VirtualNumber");

                    b.HasKey("ItemId");

                    b.HasIndex("AreaId");

                    b.HasIndex("CreatedByPlayerId");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.ItemTranslation", b =>
                {
                    b.Property<int>("ItemId");

                    b.Property<short>("LanguageId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<string>("FullDescription")
                        .IsRequired()
                        .HasMaxLength(500)
                        .IsUnicode(false);

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.HasKey("ItemId", "LanguageId")
                        .HasName("PK_ItemTranslation_ItemId_LanguageId");

                    b.HasIndex("LanguageId");

                    b.ToTable("ItemTranslation");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.Language", b =>
                {
                    b.Property<short>("LanguageId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25)
                        .IsUnicode(false);

                    b.HasKey("LanguageId");

                    b.ToTable("Language");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.Npc", b =>
                {
                    b.Property<int>("NpcId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AreaId");

                    b.Property<int>("CreatedByPlayerId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.Property<short>("VirtualNumber");

                    b.HasKey("NpcId");

                    b.HasIndex("AreaId");

                    b.HasIndex("CreatedByPlayerId");

                    b.ToTable("Npc");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.NpcItem", b =>
                {
                    b.Property<int>("NpcId");

                    b.Property<int>("ItemId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.HasKey("NpcId", "ItemId")
                        .HasName("PK_NpcItem_NpcId_ItemId");

                    b.HasIndex("ItemId");

                    b.ToTable("NpcItem");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.NpcTranslation", b =>
                {
                    b.Property<int>("NpcId");

                    b.Property<short>("LanguageId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<string>("FullDescription")
                        .IsRequired()
                        .HasMaxLength(500)
                        .IsUnicode(false);

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.HasKey("NpcId", "LanguageId")
                        .HasName("PK_NpcTranslation_NpcId_LanguageId");

                    b.HasIndex("LanguageId");

                    b.ToTable("NpcTranslation");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.PlayerCharacter", b =>
                {
                    b.Property<int>("PlayerCharacterId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<short>("Level");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25)
                        .IsUnicode(false);

                    b.Property<short>("WorldId");

                    b.HasKey("PlayerCharacterId");

                    b.HasIndex("WorldId");

                    b.ToTable("PlayerCharacter");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.PlayerCharacterItem", b =>
                {
                    b.Property<int>("PlayerCharacterId");

                    b.Property<int>("ItemId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.HasKey("PlayerCharacterId", "ItemId")
                        .HasName("PK_PlayerCharacterItem_PlayerCharacterId_ItemId");

                    b.HasIndex("ItemId");

                    b.ToTable("PlayerCharacterItem");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.Room", b =>
                {
                    b.Property<int>("RoomId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AreaId");

                    b.Property<int>("CreatedByPlayerId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<short?>("DownRoom");

                    b.Property<short?>("EastRoom");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.Property<short?>("NorthRoom");

                    b.Property<short?>("SouthRoom");

                    b.Property<short?>("UpRoom");

                    b.Property<short>("VirtualNumber");

                    b.Property<short?>("WestRoom");

                    b.HasKey("RoomId");

                    b.HasIndex("AreaId");

                    b.HasIndex("CreatedByPlayerId");

                    b.ToTable("Room");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.RoomItem", b =>
                {
                    b.Property<int>("RoomId");

                    b.Property<int>("ItemId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.HasKey("RoomId", "ItemId")
                        .HasName("PK_RoomItem_RoomId_ItemId");

                    b.HasIndex("ItemId");

                    b.ToTable("RoomItem");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.RoomNpc", b =>
                {
                    b.Property<int>("RoomId");

                    b.Property<int>("NpcId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.HasKey("RoomId", "NpcId")
                        .HasName("PK_RoomNpc_RoomId_NpcId");

                    b.HasIndex("NpcId");

                    b.ToTable("RoomNpc");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.RoomTranslation", b =>
                {
                    b.Property<int>("RoomId");

                    b.Property<short>("LanguageId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<string>("FullDescription")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .IsUnicode(false);

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.HasKey("RoomId", "LanguageId")
                        .HasName("PK_RoomTranslation_RoomId_LanguageId");

                    b.HasIndex("LanguageId");

                    b.ToTable("RoomTranslation");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.World", b =>
                {
                    b.Property<short>("WorldId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.Property<int>("UserId");

                    b.HasKey("WorldId");

                    b.ToTable("World");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.WorldTranslation", b =>
                {
                    b.Property<short>("WorldId");

                    b.Property<short>("LanguageId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .IsUnicode(false);

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.HasKey("WorldId", "LanguageId")
                        .HasName("PK_WorldTranslation_WorldId_LanguageId");

                    b.HasIndex("LanguageId");

                    b.ToTable("WorldTranslation");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.Area", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.PlayerCharacter", "CreatedByPlayer")
                        .WithMany("AreaCreatedByPlayer")
                        .HasForeignKey("CreatedByPlayerId")
                        .HasConstraintName("FK_Area_PlayerCharacter_CreatedBy");

                    b.HasOne("ScratchMUD.Server.Models.PlayerCharacter", "OwnerPlayer")
                        .WithMany("AreaOwnerPlayer")
                        .HasForeignKey("OwnerPlayerId")
                        .HasConstraintName("FK_Area_PlayerCharacter_Owner");

                    b.HasOne("ScratchMUD.Server.Models.World", "World")
                        .WithMany("Area")
                        .HasForeignKey("WorldId")
                        .HasConstraintName("FK_Area_World");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.AreaEditor", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.Area", "Area")
                        .WithMany("AreaEditor")
                        .HasForeignKey("AreaId")
                        .HasConstraintName("FK_AreaEditor_Area");

                    b.HasOne("ScratchMUD.Server.Models.PlayerCharacter", "PlayerCharacter")
                        .WithMany("AreaEditor")
                        .HasForeignKey("PlayerCharacterId")
                        .HasConstraintName("FK_AreaEditor_PlayerCharacter");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.AreaTranslation", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.Area", "Area")
                        .WithMany("AreaTranslation")
                        .HasForeignKey("AreaId")
                        .HasConstraintName("FK_AreaTranslation_Area");

                    b.HasOne("ScratchMUD.Server.Models.Language", "Language")
                        .WithMany("AreaTranslation")
                        .HasForeignKey("LanguageId")
                        .HasConstraintName("FK_AreaTranslation_Language");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.Item", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.Area", "Area")
                        .WithMany("Item")
                        .HasForeignKey("AreaId")
                        .HasConstraintName("FK_Item_Area");

                    b.HasOne("ScratchMUD.Server.Models.PlayerCharacter", "CreatedByPlayer")
                        .WithMany("Item")
                        .HasForeignKey("CreatedByPlayerId")
                        .HasConstraintName("FK_Item_PlayerCharacter");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.ItemTranslation", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.Item", "Item")
                        .WithMany("ItemTranslation")
                        .HasForeignKey("ItemId")
                        .HasConstraintName("FK_ItemTranslation_Item");

                    b.HasOne("ScratchMUD.Server.Models.Language", "Language")
                        .WithMany("ItemTranslation")
                        .HasForeignKey("LanguageId")
                        .HasConstraintName("FK_ItemTranslation_Language");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.Npc", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.Area", "Area")
                        .WithMany("Npc")
                        .HasForeignKey("AreaId")
                        .HasConstraintName("FK_Npc_Area");

                    b.HasOne("ScratchMUD.Server.Models.PlayerCharacter", "CreatedByPlayer")
                        .WithMany("Npc")
                        .HasForeignKey("CreatedByPlayerId")
                        .HasConstraintName("FK_Npc_PlayerCharacter");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.NpcItem", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.Item", "Item")
                        .WithMany("NpcItem")
                        .HasForeignKey("ItemId")
                        .HasConstraintName("FK_NpcItem_Item");

                    b.HasOne("ScratchMUD.Server.Models.Npc", "Npc")
                        .WithMany("NpcItem")
                        .HasForeignKey("NpcId")
                        .HasConstraintName("FK_NpcItem_Npc");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.NpcTranslation", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.Language", "Language")
                        .WithMany("NpcTranslation")
                        .HasForeignKey("LanguageId")
                        .HasConstraintName("FK_NpcTranslation_Language");

                    b.HasOne("ScratchMUD.Server.Models.Npc", "Npc")
                        .WithMany("NpcTranslation")
                        .HasForeignKey("NpcId")
                        .HasConstraintName("FK_NpcTranslation_Npc");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.PlayerCharacter", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.World", "World")
                        .WithMany("PlayerCharacter")
                        .HasForeignKey("WorldId")
                        .HasConstraintName("FK_PlayerCharacter_World");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.PlayerCharacterItem", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.Item", "Item")
                        .WithMany("PlayerCharacterItem")
                        .HasForeignKey("ItemId")
                        .HasConstraintName("FK_PlayerCharacterItem_Item");

                    b.HasOne("ScratchMUD.Server.Models.PlayerCharacter", "PlayerCharacter")
                        .WithMany("PlayerCharacterItem")
                        .HasForeignKey("PlayerCharacterId")
                        .HasConstraintName("FK_PlayerCharacterItem_PlayerCharacter");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.Room", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.Area", "Area")
                        .WithMany("Room")
                        .HasForeignKey("AreaId")
                        .HasConstraintName("FK_Room_Area");

                    b.HasOne("ScratchMUD.Server.Models.PlayerCharacter", "CreatedByPlayer")
                        .WithMany("Room")
                        .HasForeignKey("CreatedByPlayerId")
                        .HasConstraintName("FK_Room_PlayerCharacter");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.RoomItem", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.Item", "Item")
                        .WithMany("RoomItem")
                        .HasForeignKey("ItemId")
                        .HasConstraintName("FK_RoomItem_Item");

                    b.HasOne("ScratchMUD.Server.Models.Room", "Room")
                        .WithMany("RoomItem")
                        .HasForeignKey("RoomId")
                        .HasConstraintName("FK_RoomItem_Room");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.RoomNpc", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.Npc", "Npc")
                        .WithMany("RoomNpc")
                        .HasForeignKey("NpcId")
                        .HasConstraintName("FK_RoomNpc_Npc");

                    b.HasOne("ScratchMUD.Server.Models.Room", "Room")
                        .WithMany("RoomNpc")
                        .HasForeignKey("RoomId")
                        .HasConstraintName("FK_RoomNpc_Room");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.RoomTranslation", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.Language", "Language")
                        .WithMany("RoomTranslation")
                        .HasForeignKey("LanguageId")
                        .HasConstraintName("FK_RoomTranslation_Language");

                    b.HasOne("ScratchMUD.Server.Models.Room", "Room")
                        .WithMany("RoomTranslation")
                        .HasForeignKey("RoomId")
                        .HasConstraintName("FK_RoomTranslation_Room");
                });

            modelBuilder.Entity("ScratchMUD.Server.Models.WorldTranslation", b =>
                {
                    b.HasOne("ScratchMUD.Server.Models.Language", "Language")
                        .WithMany("WorldTranslation")
                        .HasForeignKey("LanguageId")
                        .HasConstraintName("FK_WorldTranslation_Language");

                    b.HasOne("ScratchMUD.Server.Models.World", "World")
                        .WithMany("WorldTranslation")
                        .HasForeignKey("WorldId")
                        .HasConstraintName("FK_WorldTranslation_World");
                });
#pragma warning restore 612, 618
        }
    }
}
