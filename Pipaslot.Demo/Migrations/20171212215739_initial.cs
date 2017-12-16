using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Pipaslot.Demo.Migrations
{
#pragma warning disable 1591
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecurityPrivilege",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsAllowed = table.Column<bool>(nullable: false),
                    Permission = table.Column<string>(nullable: true),
                    Resource = table.Column<string>(nullable: true),
                    ResourceInstance = table.Column<int>(nullable: false),
                    Role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityPrivilege", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimpleRecord",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Count = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleRecord", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecurityPrivilege");

            migrationBuilder.DropTable(
                name: "SimpleRecord");
        }
    }
}
