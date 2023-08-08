using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpServices.Migrations
{
    /// <inheritdoc />
    public partial class update_columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BalanceAfter",
                table: "PaymentHistories");

            migrationBuilder.RenameColumn(
                name: "BalanceAtTheMoment",
                table: "PaymentHistories",
                newName: "CurrentBalance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentBalance",
                table: "PaymentHistories",
                newName: "BalanceAtTheMoment");

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceAfter",
                table: "PaymentHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
