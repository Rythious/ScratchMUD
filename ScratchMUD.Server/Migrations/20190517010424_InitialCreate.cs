using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScratchMUD.Server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    LanguageId = table.Column<short>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(unicode: false, maxLength: 25, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.LanguageId);
                });

            migrationBuilder.CreateTable(
                name: "World",
                columns: table => new
                {
                    WorldId = table.Column<short>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_World", x => x.WorldId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerCharacter",
                columns: table => new
                {
                    PlayerCharacterId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WorldId = table.Column<short>(nullable: false),
                    Name = table.Column<string>(unicode: false, maxLength: 25, nullable: false),
                    Level = table.Column<short>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCharacter", x => x.PlayerCharacterId);
                    table.ForeignKey(
                        name: "FK_PlayerCharacter_World",
                        column: x => x.WorldId,
                        principalTable: "World",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorldTranslation",
                columns: table => new
                {
                    WorldId = table.Column<short>(nullable: false),
                    LanguageId = table.Column<short>(nullable: false),
                    Name = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(unicode: false, maxLength: 1000, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldTranslation_WorldId_LanguageId", x => new { x.WorldId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_WorldTranslation_Language",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorldTranslation_World",
                        column: x => x.WorldId,
                        principalTable: "World",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    AreaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WorldId = table.Column<short>(nullable: false),
                    VirtualNumber = table.Column<short>(nullable: false),
                    CreatedByPlayerId = table.Column<int>(nullable: false),
                    OwnerPlayerId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.AreaId);
                    table.ForeignKey(
                        name: "FK_Area_PlayerCharacter_CreatedBy",
                        column: x => x.CreatedByPlayerId,
                        principalTable: "PlayerCharacter",
                        principalColumn: "PlayerCharacterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Area_PlayerCharacter_Owner",
                        column: x => x.OwnerPlayerId,
                        principalTable: "PlayerCharacter",
                        principalColumn: "PlayerCharacterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Area_World",
                        column: x => x.WorldId,
                        principalTable: "World",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AreaEditor",
                columns: table => new
                {
                    AreaId = table.Column<int>(nullable: false),
                    PlayerCharacterId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaEditor_AreaId_PlayerCharacterId", x => new { x.AreaId, x.PlayerCharacterId });
                    table.ForeignKey(
                        name: "FK_AreaEditor_Area",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AreaEditor_PlayerCharacter",
                        column: x => x.PlayerCharacterId,
                        principalTable: "PlayerCharacter",
                        principalColumn: "PlayerCharacterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AreaTranslation",
                columns: table => new
                {
                    AreaId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<short>(nullable: false),
                    Title = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaTranslation_AreaId_LanguageId", x => new { x.AreaId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_AreaTranslation_Area",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AreaTranslation_Language",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    ItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AreaId = table.Column<int>(nullable: false),
                    CreatedByPlayerId = table.Column<int>(nullable: false),
                    VirtualNumber = table.Column<short>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Item_Area",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Item_PlayerCharacter",
                        column: x => x.CreatedByPlayerId,
                        principalTable: "PlayerCharacter",
                        principalColumn: "PlayerCharacterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Npc",
                columns: table => new
                {
                    NpcId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AreaId = table.Column<int>(nullable: false),
                    CreatedByPlayerId = table.Column<int>(nullable: false),
                    VirtualNumber = table.Column<short>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Npc", x => x.NpcId);
                    table.ForeignKey(
                        name: "FK_Npc_Area",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Npc_PlayerCharacter",
                        column: x => x.CreatedByPlayerId,
                        principalTable: "PlayerCharacter",
                        principalColumn: "PlayerCharacterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    RoomId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AreaId = table.Column<int>(nullable: false),
                    CreatedByPlayerId = table.Column<int>(nullable: false),
                    VirtualNumber = table.Column<short>(nullable: false),
                    NorthRoom = table.Column<short>(nullable: true),
                    EastRoom = table.Column<short>(nullable: true),
                    SouthRoom = table.Column<short>(nullable: true),
                    WestRoom = table.Column<short>(nullable: true),
                    UpRoom = table.Column<short>(nullable: true),
                    DownRoom = table.Column<short>(nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_Room_Area",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Room_PlayerCharacter",
                        column: x => x.CreatedByPlayerId,
                        principalTable: "PlayerCharacter",
                        principalColumn: "PlayerCharacterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemTranslation",
                columns: table => new
                {
                    ItemId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<short>(nullable: false),
                    ShortDescription = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    FullDescription = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTranslation_ItemId_LanguageId", x => new { x.ItemId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_ItemTranslation_Item",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemTranslation_Language",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerCharacterItem",
                columns: table => new
                {
                    PlayerCharacterId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCharacterItem_PlayerCharacterId_ItemId", x => new { x.PlayerCharacterId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_PlayerCharacterItem_Item",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerCharacterItem_PlayerCharacter",
                        column: x => x.PlayerCharacterId,
                        principalTable: "PlayerCharacter",
                        principalColumn: "PlayerCharacterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NpcItem",
                columns: table => new
                {
                    NpcId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NpcItem_NpcId_ItemId", x => new { x.NpcId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_NpcItem_Item",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NpcItem_Npc",
                        column: x => x.NpcId,
                        principalTable: "Npc",
                        principalColumn: "NpcId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NpcTranslation",
                columns: table => new
                {
                    NpcId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<short>(nullable: false),
                    ShortDescription = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    FullDescription = table.Column<string>(unicode: false, maxLength: 500, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NpcTranslation_NpcId_LanguageId", x => new { x.NpcId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_NpcTranslation_Language",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NpcTranslation_Npc",
                        column: x => x.NpcId,
                        principalTable: "Npc",
                        principalColumn: "NpcId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomItem",
                columns: table => new
                {
                    RoomId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomItem_RoomId_ItemId", x => new { x.RoomId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_RoomItem_Item",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomItem_Room",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomNpc",
                columns: table => new
                {
                    RoomId = table.Column<int>(nullable: false),
                    NpcId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomNpc_RoomId_NpcId", x => new { x.RoomId, x.NpcId });
                    table.ForeignKey(
                        name: "FK_RoomNpc_Npc",
                        column: x => x.NpcId,
                        principalTable: "Npc",
                        principalColumn: "NpcId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomNpc_Room",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomTranslation",
                columns: table => new
                {
                    RoomId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<short>(nullable: false),
                    Title = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    ShortDescription = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    FullDescription = table.Column<string>(unicode: false, maxLength: 5000, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTranslation_RoomId_LanguageId", x => new { x.RoomId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_RoomTranslation_Language",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomTranslation_Room",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Area_CreatedByPlayerId",
                table: "Area",
                column: "CreatedByPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Area_OwnerPlayerId",
                table: "Area",
                column: "OwnerPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Area_WorldId",
                table: "Area",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaEditor_PlayerCharacterId",
                table: "AreaEditor",
                column: "PlayerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaTranslation_LanguageId",
                table: "AreaTranslation",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_AreaId",
                table: "Item",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_CreatedByPlayerId",
                table: "Item",
                column: "CreatedByPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTranslation_LanguageId",
                table: "ItemTranslation",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Npc_AreaId",
                table: "Npc",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Npc_CreatedByPlayerId",
                table: "Npc",
                column: "CreatedByPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_NpcItem_ItemId",
                table: "NpcItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_NpcTranslation_LanguageId",
                table: "NpcTranslation",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCharacter_WorldId",
                table: "PlayerCharacter",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCharacterItem_ItemId",
                table: "PlayerCharacterItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_AreaId",
                table: "Room",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_CreatedByPlayerId",
                table: "Room",
                column: "CreatedByPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomItem_ItemId",
                table: "RoomItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomNpc_NpcId",
                table: "RoomNpc",
                column: "NpcId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTranslation_LanguageId",
                table: "RoomTranslation",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_WorldTranslation_LanguageId",
                table: "WorldTranslation",
                column: "LanguageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AreaEditor");

            migrationBuilder.DropTable(
                name: "AreaTranslation");

            migrationBuilder.DropTable(
                name: "ItemTranslation");

            migrationBuilder.DropTable(
                name: "NpcItem");

            migrationBuilder.DropTable(
                name: "NpcTranslation");

            migrationBuilder.DropTable(
                name: "PlayerCharacterItem");

            migrationBuilder.DropTable(
                name: "RoomItem");

            migrationBuilder.DropTable(
                name: "RoomNpc");

            migrationBuilder.DropTable(
                name: "RoomTranslation");

            migrationBuilder.DropTable(
                name: "WorldTranslation");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "Npc");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropTable(
                name: "PlayerCharacter");

            migrationBuilder.DropTable(
                name: "World");
        }
    }
}
