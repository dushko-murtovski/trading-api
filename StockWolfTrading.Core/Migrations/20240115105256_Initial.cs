using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockWolfTrading.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Algorithm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlgorithmName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Algorithm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Certs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyAnalysis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Stock = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PocLevel = table.Column<float>(type: "real", nullable: true),
                    ResistanceG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResistanceGp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportGp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MS_Up = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MS_Down = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MS_Up5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MS_Down5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MS_Up15 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MS_Down15 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyAnalysis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: true),
                    MaxStocks = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    SettingsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SettingName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SettingValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.SettingsId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<byte[]>(type: "varbinary(64)", maxLength: 64, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsFirstLogin = table.Column<bool>(type: "bit", nullable: false),
                    Expire = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "('1900-01-01T00:00:00.000')"),
                    IsExpire = table.Column<bool>(type: "bit", nullable: false),
                    IsExistUserInSafetyReport = table.Column<bool>(type: "bit", nullable: true),
                    IsAuthenticated = table.Column<bool>(type: "bit", nullable: true),
                    VerificationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SignatureImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Trade",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlgorithmRefId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CandleOpen = table.Column<float>(type: "real", nullable: false),
                    CandleClose = table.Column<float>(type: "real", nullable: false),
                    CandleHigh = table.Column<float>(type: "real", nullable: false),
                    CandleLow = table.Column<float>(type: "real", nullable: false),
                    Interval = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Ticker = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BuySell = table.Column<bool>(type: "bit", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trades_Algorithms",
                        column: x => x.AlgorithmRefId,
                        principalTable: "Algorithm",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserRefId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateFinished = table.Column<DateTime>(type: "datetime", nullable: true),
                    TotalAmount = table.Column<float>(type: "real", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsExpired = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_User",
                        column: x => x.UserRefId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserUserId = table.Column<int>(type: "int", nullable: false),
                    ProductProductId = table.Column<int>(type: "int", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProducts_Product_ProductProductId",
                        column: x => x.ProductProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProducts_User_UserUserId",
                        column: x => x.UserUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    User_UserId = table.Column<int>(type: "int", nullable: false),
                    Role_RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.UserRoles", x => new { x.User_UserId, x.Role_RoleId });
                    table.ForeignKey(
                        name: "FK_dbo.UserRoles_Identity.Role_Role_RoleId",
                        column: x => x.Role_RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.UserRoles_Identity.User_User_UserId",
                        column: x => x.User_UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserRefId = table.Column<int>(type: "int", nullable: false),
                    CandleTime = table.Column<int>(type: "int", nullable: true),
                    CandleOpen = table.Column<float>(type: "real", nullable: true),
                    CandleClose = table.Column<float>(type: "real", nullable: true),
                    CandleHigh = table.Column<float>(type: "real", nullable: true),
                    CandleLow = table.Column<float>(type: "real", nullable: true),
                    Interval = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Ticker = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_User",
                        column: x => x.UserRefId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderRefId = table.Column<int>(type: "int", nullable: false),
                    ProductRefId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders",
                        column: x => x.OrderRefId,
                        principalTable: "Order",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products",
                        column: x => x.ProductRefId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetailsTicker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDetailsRefId = table.Column<int>(type: "int", nullable: false),
                    TickerName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DateSelected = table.Column<DateTime>(type: "datetime", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetailsTicker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetailsTickers_OrderDetails",
                        column: x => x.OrderDetailsRefId,
                        principalTable: "OrderDetail",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserRefId",
                table: "Order",
                column: "UserRefId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderRefId",
                table: "OrderDetail",
                column: "OrderRefId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProductRefId",
                table: "OrderDetail",
                column: "ProductRefId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailsTicker_OrderDetailsRefId",
                table: "OrderDetailsTicker",
                column: "OrderDetailsRefId");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_AlgorithmRefId",
                table: "Trade",
                column: "AlgorithmRefId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProducts_ProductProductId",
                table: "UserProducts",
                column: "ProductProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProducts_UserUserId",
                table: "UserProducts",
                column: "UserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_Role_RoleId",
                table: "UserRoles",
                column: "Role_RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserRefId",
                table: "UserSettings",
                column: "UserRefId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certs");

            migrationBuilder.DropTable(
                name: "DailyAnalysis");

            migrationBuilder.DropTable(
                name: "OrderDetailsTicker");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Trade");

            migrationBuilder.DropTable(
                name: "UserProducts");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "Algorithm");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
