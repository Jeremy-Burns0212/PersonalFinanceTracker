using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceTracker.Migrations
{
	/// <inheritdoc />
	public partial class AddTranscriptsArchive : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "TranscriptTransactions");

			migrationBuilder.CreateTable(
				name: "TranscriptTransactions",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					TranscriptId = table.Column<int>(type: "int", nullable: false),
					TransactionId = table.Column<int>(type: "int", nullable: false),
					Date = table.Column<DateOnly>(type: "date", nullable: false),
					Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
					Type = table.Column<int>(type: "int", nullable: false),
					CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_TranscriptTransactions", x => x.Id);
					table.ForeignKey(
						name: "FK_TranscriptTransactions_Transcripts_TranscriptId",
						column: x => x.TranscriptId,
						principalTable: "Transcripts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_TranscriptTransactions_TranscriptId",
				table: "TranscriptTransactions",
				column: "TranscriptId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "TranscriptTransactions");

			migrationBuilder.CreateTable(
				name: "TranscriptTransactions",
				columns: table => new
				{
					TranscriptId = table.Column<int>(type: "int", nullable: false),
					TransactionId = table.Column<int>(type: "int", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_TranscriptTransactions", x => new { x.TranscriptId, x.TransactionId });
					table.ForeignKey(
						name: "FK_TranscriptTransactions_Transactions_TransactionId",
						column: x => x.TransactionId,
						principalTable: "Transactions",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_TranscriptTransactions_Transcripts_TranscriptId",
						column: x => x.TranscriptId,
						principalTable: "Transcripts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});
		}
	}
}