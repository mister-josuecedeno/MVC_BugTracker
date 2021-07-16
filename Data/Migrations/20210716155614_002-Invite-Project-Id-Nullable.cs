using Microsoft.EntityFrameworkCore.Migrations;

namespace MVC_BugTracker.Data.Migrations
{
    public partial class _002InviteProjectIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invite_Project_ProjectId",
                table: "Invite");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Invite",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Invite_Project_ProjectId",
                table: "Invite",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invite_Project_ProjectId",
                table: "Invite");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Invite",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invite_Project_ProjectId",
                table: "Invite",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
