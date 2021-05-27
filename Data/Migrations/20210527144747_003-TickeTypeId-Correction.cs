using Microsoft.EntityFrameworkCore.Migrations;

namespace MVC_BugTracker.Data.Migrations
{
    public partial class _003TickeTypeIdCorrection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_TicketType_TypeId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_TypeId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Ticket");

            migrationBuilder.RenameColumn(
                name: "TicketType",
                table: "Ticket",
                newName: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TicketTypeId",
                table: "Ticket",
                column: "TicketTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_TicketType_TicketTypeId",
                table: "Ticket",
                column: "TicketTypeId",
                principalTable: "TicketType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_TicketType_TicketTypeId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_TicketTypeId",
                table: "Ticket");

            migrationBuilder.RenameColumn(
                name: "TicketTypeId",
                table: "Ticket",
                newName: "TicketType");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Ticket",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TypeId",
                table: "Ticket",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_TicketType_TypeId",
                table: "Ticket",
                column: "TypeId",
                principalTable: "TicketType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
