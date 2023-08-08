using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpServices.Migrations
{
    /// <inheritdoc />
    public partial class create_payment_history : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentHistories",
                columns: table => new
                {
                    CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentSucceeded = table.Column<bool>(type: "bit", nullable: false),
                    PaymentValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentWithFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceAtTheMoment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentHistories", x => x.CardId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentHistories");
        }
    }
}
