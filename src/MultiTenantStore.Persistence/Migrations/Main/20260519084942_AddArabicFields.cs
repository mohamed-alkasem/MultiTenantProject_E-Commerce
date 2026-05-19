using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiTenantStore.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddArabicFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "SUBSCRIPTION_PLAN",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreNameAr",
                table: "STORE",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "PRODUCT_VARIANT",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "PRODUCT",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "PRODUCT",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescriptionAr",
                table: "PRODUCT",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "CATEGORY",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "SUBSCRIPTION_PLAN");

            migrationBuilder.DropColumn(
                name: "StoreNameAr",
                table: "STORE");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "PRODUCT_VARIANT");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "PRODUCT");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "PRODUCT");

            migrationBuilder.DropColumn(
                name: "ShortDescriptionAr",
                table: "PRODUCT");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "CATEGORY");
        }
    }
}
