﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestTwinCoreProject.Migrations
{
    public partial class InviteUserEntityAddUserIdProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "InviteUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "InviteUsers");
        }
    }
}
