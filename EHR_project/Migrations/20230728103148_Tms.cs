using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHR_project.Migrations
{
    /// <inheritdoc />
    public partial class Tms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "speciality",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    speciality = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_speciality", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "transactionHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Appointment_Id = table.Column<int>(type: "int", nullable: false),
                    Charge_Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactionHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "varchar(20)", nullable: true),
                    User_type = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "varchar(50)", nullable: true),
                    isValidate = table.Column<bool>(type: "bit", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    profile_Path = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "otp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Otp = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_otp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_otp_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "OTPVerification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OTP = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTPVerification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OTPVerification_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "patient",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(20)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(20)", nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", nullable: true),
                    Phone = table.Column<string>(type: "varchar(20)", nullable: true),
                    DOB = table.Column<DateTime>(type: "date", nullable: true),
                    InsuranceNo = table.Column<string>(type: "varchar(20)", nullable: true),
                    Address = table.Column<string>(type: "varchar(100)", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_patient_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "provider",
                columns: table => new
                {
                    ProviderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    First_name = table.Column<string>(type: "varchar(20)", nullable: false),
                    Last_name = table.Column<string>(type: "varchar(20)", nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", nullable: false),
                    DOB = table.Column<DateTime>(type: "Date", nullable: false),
                    Experience = table.Column<int>(type: "int", nullable: false),
                    Speciality = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "varchar(20)", nullable: true),
                    Phone = table.Column<string>(type: "varchar(20)", nullable: true),
                    Address = table.Column<string>(type: "varchar(100)", nullable: true),
                    Fees = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_provider", x => x.ProviderId);
                    table.ForeignKey(
                        name: "FK_provider_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "appointment",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "date", nullable: true),
                    AppointmentTime = table.Column<string>(type: "varchar(10)", nullable: false),
                    Note = table.Column<string>(type: "varchar(100)", nullable: true),
                    AppointmentStatus = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointment", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_appointment_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_appointment_provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "provider",
                            principalColumn: "ProviderId");
                });

            migrationBuilder.CreateTable(
                name: "soap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    Subjective = table.Column<string>(type: "varchar(200)", nullable: false),
                    Objective = table.Column<string>(type: "varchar(200)", nullable: false),
                    Assessment = table.Column<string>(type: "varchar(200)", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Plan = table.Column<string>(type: "varchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_soap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_soap_appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "appointment",
                        principalColumn: "AppointmentId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointment_PatientId",
                table: "appointment",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_ProviderId",
                table: "appointment",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_otp_UserId",
                table: "otp",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OTPVerification_UserId",
                table: "OTPVerification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_patient_UserId",
                table: "patient",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_provider_UserId",
                table: "provider",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_soap_AppointmentId",
                table: "soap",
                column: "AppointmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "otp");

            migrationBuilder.DropTable(
                name: "OTPVerification");

            migrationBuilder.DropTable(
                name: "soap");

            migrationBuilder.DropTable(
                name: "speciality");

            migrationBuilder.DropTable(
                name: "transactionHistory");

            migrationBuilder.DropTable(
                name: "appointment");

            migrationBuilder.DropTable(
                name: "patient");

            migrationBuilder.DropTable(
                name: "provider");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
