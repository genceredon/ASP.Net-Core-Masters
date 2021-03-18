using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Repositories.Migrations
{
    public partial class AddTodoList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodoList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Todo = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false)

                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoList", x => x.Id);
                });            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "TodoList");
        }
    }
}
