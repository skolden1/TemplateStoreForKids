﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MallarEmelieMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedEmailToOrdertbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Orders");
        }
    }
}
