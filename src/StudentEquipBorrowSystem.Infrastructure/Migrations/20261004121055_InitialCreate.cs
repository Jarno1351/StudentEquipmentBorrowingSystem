using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentEquipBorrowSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EquipmentName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EquipmentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment_Id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentID = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    College = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Course = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    YearLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ContactNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students_StudentID", x => x.StudentID);
                });

            migrationBuilder.CreateTable(
                name: "Borrows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StudentBorrower_StudentID = table.Column<string>(type: "TEXT", nullable: true),
                    EquipmentBorrowed_Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BorrowDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Borrows_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Borrows_Equipment_Id",
                        column: x => x.EquipmentBorrowed_Id,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Borrows_Students_StudentID",
                        column: x => x.StudentBorrower_StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Borrows_DueDate",
                table: "Borrows",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Borrows_EquipmentId",
                table: "Borrows",
                column: "EquipmentBorrowed_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Borrows_Status",
                table: "Borrows",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Borrows_StudentID",
                table: "Borrows",
                column: "StudentBorrower_StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_EquipmentType",
                table: "Equipment",
                column: "EquipmentType");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_IsAvailable",
                table: "Equipment",
                column: "IsAvailable");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ContactNumber",
                table: "Students",
                column: "ContactNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentID_Unique",
                table: "Students",
                column: "StudentID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Borrows");

            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
