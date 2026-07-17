using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jira.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixProjectTaskAssigneeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.AddColumn<Guid>(
                name: "AssigneeId",
                table: "ProjectTasks",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "ProjectTasks");
        }
    }
}
