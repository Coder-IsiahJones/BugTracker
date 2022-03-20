using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracker.Data.Migrations
{
    public partial class TicketArchivedProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_DeveloperUserId1",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_DeveloperUserId1",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DeveloperUserId1",
                table: "Tickets");

            migrationBuilder.AlterColumn<string>(
                name: "DeveloperUserId",
                table: "Tickets",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "ArchivedByProject",
                table: "Tickets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InviteeEmail",
                table: "Invites",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InviteeFirstName",
                table: "Invites",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InviteeLastName",
                table: "Invites",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_DeveloperUserId",
                table: "Tickets",
                column: "DeveloperUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_DeveloperUserId",
                table: "Tickets",
                column: "DeveloperUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_DeveloperUserId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_DeveloperUserId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ArchivedByProject",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "InviteeEmail",
                table: "Invites");

            migrationBuilder.DropColumn(
                name: "InviteeFirstName",
                table: "Invites");

            migrationBuilder.DropColumn(
                name: "InviteeLastName",
                table: "Invites");

            migrationBuilder.AlterColumn<int>(
                name: "DeveloperUserId",
                table: "Tickets",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeveloperUserId1",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_DeveloperUserId1",
                table: "Tickets",
                column: "DeveloperUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_DeveloperUserId1",
                table: "Tickets",
                column: "DeveloperUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
