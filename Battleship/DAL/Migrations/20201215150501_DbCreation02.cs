using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class DbCreation02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardDbFriendly",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "IsViewingOwnBoard",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "PPlayerDbFriendly",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "fOffsetX",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "fOffsetY",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "fScaleX",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "fScaleY",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "fSelectedTileX",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "fSelectedTileY",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "BoardHeight",
                table: "GameData");

            migrationBuilder.DropColumn(
                name: "BoardWidth",
                table: "GameData");

            migrationBuilder.DropColumn(
                name: "Phase",
                table: "GameData");

            migrationBuilder.DropColumn(
                name: "WinningPlayer",
                table: "GameData");

            migrationBuilder.AddColumn<string>(
                name: "BoardBoundsDbFriendly",
                table: "Player",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SpriteDbFriendly",
                table: "Player",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "fCameraPixelPosX",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "fCameraPixelPosY",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "fCameraScaleX",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "fCameraScaleY",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "fMouseSelectedTileX",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "fMouseSelectedTileY",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "BoardDbFriendly",
                table: "GameData",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SpritesDbFriendly",
                table: "GameData",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "GameData",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardBoundsDbFriendly",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "SpriteDbFriendly",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "fCameraPixelPosX",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "fCameraPixelPosY",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "fCameraScaleX",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "fCameraScaleY",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "fMouseSelectedTileX",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "fMouseSelectedTileY",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "BoardDbFriendly",
                table: "GameData");

            migrationBuilder.DropColumn(
                name: "SpritesDbFriendly",
                table: "GameData");

            migrationBuilder.DropColumn(
                name: "State",
                table: "GameData");

            migrationBuilder.AddColumn<string>(
                name: "BoardDbFriendly",
                table: "Player",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsViewingOwnBoard",
                table: "Player",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PPlayerDbFriendly",
                table: "Player",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "fOffsetX",
                table: "Player",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "fOffsetY",
                table: "Player",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "fScaleX",
                table: "Player",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "fScaleY",
                table: "Player",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "fSelectedTileX",
                table: "Player",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "fSelectedTileY",
                table: "Player",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "BoardHeight",
                table: "GameData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BoardWidth",
                table: "GameData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Phase",
                table: "GameData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WinningPlayer",
                table: "GameData",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
