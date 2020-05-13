using Microsoft.EntityFrameworkCore.Migrations;

namespace CalculateWeekdaysApi.Migrations
{
    public partial class Migration_20200514_1255 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HolidayType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeekDays",
                columns: table => new
                {
                    Index = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekDays", x => x.Index);
                });

            migrationBuilder.CreateTable(
                name: "Holiday",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Date = table.Column<int>(nullable: true),
                    WeekDayIndex = table.Column<int>(nullable: true),
                    Month = table.Column<int>(nullable: true),
                    WeekCount = table.Column<int>(nullable: true),
                    Year = table.Column<int>(nullable: true),
                    TypeId = table.Column<int>(nullable: true),
                    HolidaysListID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holiday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Holiday_Holidays_HolidaysListID",
                        column: x => x.HolidaysListID,
                        principalTable: "Holidays",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Holiday_HolidayType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "HolidayType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Holiday_WeekDays_WeekDayIndex",
                        column: x => x.WeekDayIndex,
                        principalTable: "WeekDays",
                        principalColumn: "Index",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Holiday_HolidaysListID",
                table: "Holiday",
                column: "HolidaysListID");

            migrationBuilder.CreateIndex(
                name: "IX_Holiday_TypeId",
                table: "Holiday",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Holiday_WeekDayIndex",
                table: "Holiday",
                column: "WeekDayIndex");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Holiday");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "HolidayType");

            migrationBuilder.DropTable(
                name: "WeekDays");
        }
    }
}
