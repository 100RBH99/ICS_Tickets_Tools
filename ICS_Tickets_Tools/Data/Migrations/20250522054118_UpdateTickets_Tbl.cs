using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ICS_Tickets_Tools.Data.Migrations
{
    public partial class UpdateTickets_Tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Tickets_Tbl");

            migrationBuilder.RenameColumn(
                name: "SubCategory",
                table: "Tickets_Tbl",
                newName: "SubCategoryId");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Tickets_Tbl",
                newName: "CategoryId");

            migrationBuilder.AlterColumn<string>(
                name: "TicketDescription",
                table: "Tickets_Tbl",
                type: "nvarchar(Max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(Max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedBy",
                table: "Tickets_Tbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedDate",
                table: "Tickets_Tbl",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedRemark",
                table: "Tickets_Tbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "Tickets_Tbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Tickets_Tbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedRemark",
                table: "Tickets_Tbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CloseingDate",
                table: "Tickets_Tbl",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Tickets_Tbl",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EmpName",
                table: "Tickets_Tbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoldDate",
                table: "Tickets_Tbl",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoldRemark",
                table: "Tickets_Tbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Tickets_Tbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Tickets_Tbl",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedBy",
                table: "Tickets_Tbl");

            migrationBuilder.DropColumn(
                name: "AssignedDate",
                table: "Tickets_Tbl");

            migrationBuilder.DropColumn(
                name: "AssignedRemark",
                table: "Tickets_Tbl");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "Tickets_Tbl");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Tickets_Tbl");

            migrationBuilder.DropColumn(
                name: "ClosedRemark",
                table: "Tickets_Tbl");

            migrationBuilder.DropColumn(
                name: "CloseingDate",
                table: "Tickets_Tbl");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Tickets_Tbl");

            migrationBuilder.DropColumn(
                name: "EmpName",
                table: "Tickets_Tbl");

            migrationBuilder.DropColumn(
                name: "HoldDate",
                table: "Tickets_Tbl");

            migrationBuilder.DropColumn(
                name: "HoldRemark",
                table: "Tickets_Tbl");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Tickets_Tbl");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Tickets_Tbl");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SubCategoryId",
                table: "Tickets_Tbl",
                newName: "SubCategory");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Tickets_Tbl",
                newName: "Category");

            migrationBuilder.AlterColumn<string>(
                name: "TicketDescription",
                table: "Tickets_Tbl",
                type: "nvarchar(Max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(Max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Tickets_Tbl",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
