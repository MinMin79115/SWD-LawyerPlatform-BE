using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokensTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:appointment_status", "Pending,Confirmed,Completed,Cancelled")
                .Annotation("Npgsql:Enum:duration_type", "30Minutes,60Minutes,90Minutes,120Minutes")
                .Annotation("Npgsql:Enum:form_status", "Draft,Submitted,Processing,Completed,Cancelled")
                .Annotation("Npgsql:Enum:law_type", "RealEstateLaw,CriminalLaw,LaborLaw,EnterpriseLaw")
                .Annotation("Npgsql:Enum:payment_status", "Pending,Completed,Failed,Refunded")
                .Annotation("Npgsql:Enum:service_type", "BookConsultant,LawForm")
                .Annotation("Npgsql:Enum:user_role", "Customer,Lawyer,Admin");

            migrationBuilder.CreateTable(
                name: "durations",
                columns: table => new
                {
                    durationid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("durations_pkey", x => x.durationid);
                });

            migrationBuilder.CreateTable(
                name: "lawtypes",
                columns: table => new
                {
                    lawtypeid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lawtypes_pkey", x => x.lawtypeid);
                });

            migrationBuilder.CreateTable(
                name: "packages",
                columns: table => new
                {
                    packageid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    packagename = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    bookingcount = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    lawformcount = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValueSql: "0.00"),
                    description = table.Column<string>(type: "text", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("packages_pkey", x => x.packageid);
                });

            migrationBuilder.CreateTable(
                name: "servicestypes",
                columns: table => new
                {
                    servicetypeid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("servicestypes_pkey", x => x.servicetypeid);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    userid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    avatar = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.userid);
                });

            migrationBuilder.CreateTable(
                name: "lawform",
                columns: table => new
                {
                    lawformid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    servicestypeid = table.Column<int>(type: "integer", nullable: true),
                    lawtypeid = table.Column<int>(type: "integer", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValueSql: "0.00"),
                    formpath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("lawform_pkey", x => x.lawformid);
                    table.ForeignKey(
                        name: "lawform_lawtypeid_fkey",
                        column: x => x.lawtypeid,
                        principalTable: "lawtypes",
                        principalColumn: "lawtypeid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "lawform_servicestypeid_fkey",
                        column: x => x.servicestypeid,
                        principalTable: "servicestypes",
                        principalColumn: "servicetypeid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    serviceid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    servicestypeid = table.Column<int>(type: "integer", nullable: true),
                    lawtypeid = table.Column<int>(type: "integer", nullable: true),
                    durationid = table.Column<int>(type: "integer", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValueSql: "0.00"),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("services_pkey", x => x.serviceid);
                    table.ForeignKey(
                        name: "services_durationid_fkey",
                        column: x => x.durationid,
                        principalTable: "durations",
                        principalColumn: "durationid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "services_lawtypeid_fkey",
                        column: x => x.lawtypeid,
                        principalTable: "lawtypes",
                        principalColumn: "lawtypeid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "services_servicestypeid_fkey",
                        column: x => x.servicestypeid,
                        principalTable: "servicestypes",
                        principalColumn: "servicetypeid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feedbacks",
                columns: table => new
                {
                    feedbackid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: true),
                    rating = table.Column<int>(type: "integer", nullable: true),
                    comment = table.Column<string>(type: "text", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("feedbacks_pkey", x => x.feedbackid);
                    table.ForeignKey(
                        name: "feedbacks_userid_fkey",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lawyers",
                columns: table => new
                {
                    lawyerid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: true),
                    experience = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    description = table.Column<string>(type: "text", nullable: true),
                    qualification = table.Column<string>(type: "text", nullable: true),
                    specialties = table.Column<List<string>>(type: "text[]", nullable: true),
                    rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true, defaultValueSql: "0.00"),
                    consultationfee = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true, defaultValueSql: "0.00"),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("lawyers_pkey", x => x.lawyerid);
                    table.ForeignKey(
                        name: "lawyers_userid_fkey",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refreshtokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    jwtid = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    isused = table.Column<bool>(type: "boolean", nullable: false),
                    isrevoked = table.Column<bool>(type: "boolean", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    expirydate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    userid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("refreshtokens_pkey", x => x.id);
                    table.ForeignKey(
                        name: "refreshtokens_userid_fkey",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usercredit",
                columns: table => new
                {
                    usercreditid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: true),
                    bookingremaining = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    lawformremaining = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("usercredit_pkey", x => x.usercreditid);
                    table.ForeignKey(
                        name: "usercredit_userid_fkey",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customerforms",
                columns: table => new
                {
                    customerformid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: true),
                    lawformid = table.Column<int>(type: "integer", nullable: true),
                    totalamount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValueSql: "0.00"),
                    formdata = table.Column<string>(type: "jsonb", nullable: true),
                    linkform = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("customerforms_pkey", x => x.customerformid);
                    table.ForeignKey(
                        name: "customerforms_lawformid_fkey",
                        column: x => x.lawformid,
                        principalTable: "lawform",
                        principalColumn: "lawformid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "customerforms_userid_fkey",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    appointmentid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: true),
                    serviceid = table.Column<int>(type: "integer", nullable: true),
                    lawyerid = table.Column<int>(type: "integer", nullable: true),
                    scheduletime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    scheduledate = table.Column<DateOnly>(type: "date", nullable: false),
                    meetinglink = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    totalamount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValueSql: "0.00"),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("appointments_pkey", x => x.appointmentid);
                    table.ForeignKey(
                        name: "appointments_lawyerid_fkey",
                        column: x => x.lawyerid,
                        principalTable: "lawyers",
                        principalColumn: "lawyerid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "appointments_serviceid_fkey",
                        column: x => x.serviceid,
                        principalTable: "services",
                        principalColumn: "serviceid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "appointments_userid_fkey",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    paymentid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: true),
                    appointmentid = table.Column<int>(type: "integer", nullable: true),
                    customerformid = table.Column<int>(type: "integer", nullable: true),
                    packageid = table.Column<int>(type: "integer", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    transactionid = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    paymentdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    createdat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("payment_pkey", x => x.paymentid);
                    table.ForeignKey(
                        name: "payment_appointmentid_fkey",
                        column: x => x.appointmentid,
                        principalTable: "appointments",
                        principalColumn: "appointmentid",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "payment_customerformid_fkey",
                        column: x => x.customerformid,
                        principalTable: "customerforms",
                        principalColumn: "customerformid",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "payment_packageid_fkey",
                        column: x => x.packageid,
                        principalTable: "packages",
                        principalColumn: "packageid",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "payment_userid_fkey",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_appointments_date",
                table: "appointments",
                column: "scheduledate");

            migrationBuilder.CreateIndex(
                name: "idx_appointments_lawyerid",
                table: "appointments",
                column: "lawyerid");

            migrationBuilder.CreateIndex(
                name: "idx_appointments_userid",
                table: "appointments",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_serviceid",
                table: "appointments",
                column: "serviceid");

            migrationBuilder.CreateIndex(
                name: "idx_customerforms_userid",
                table: "customerforms",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_customerforms_lawformid",
                table: "customerforms",
                column: "lawformid");

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_userid",
                table: "feedbacks",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_lawform_lawtypeid",
                table: "lawform",
                column: "lawtypeid");

            migrationBuilder.CreateIndex(
                name: "IX_lawform_servicestypeid",
                table: "lawform",
                column: "servicestypeid");

            migrationBuilder.CreateIndex(
                name: "idx_lawyers_rating",
                table: "lawyers",
                column: "rating");

            migrationBuilder.CreateIndex(
                name: "idx_lawyers_userid",
                table: "lawyers",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "idx_payment_transactionid",
                table: "payment",
                column: "transactionid");

            migrationBuilder.CreateIndex(
                name: "idx_payment_userid",
                table: "payment",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_payment_appointmentid",
                table: "payment",
                column: "appointmentid");

            migrationBuilder.CreateIndex(
                name: "IX_payment_customerformid",
                table: "payment",
                column: "customerformid");

            migrationBuilder.CreateIndex(
                name: "IX_payment_packageid",
                table: "payment",
                column: "packageid");

            migrationBuilder.CreateIndex(
                name: "payment_transactionid_key",
                table: "payment",
                column: "transactionid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refreshtokens_token",
                table: "refreshtokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refreshtokens_userid",
                table: "refreshtokens",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_services_durationid",
                table: "services",
                column: "durationid");

            migrationBuilder.CreateIndex(
                name: "IX_services_lawtypeid",
                table: "services",
                column: "lawtypeid");

            migrationBuilder.CreateIndex(
                name: "IX_services_servicestypeid",
                table: "services",
                column: "servicestypeid");

            migrationBuilder.CreateIndex(
                name: "idx_usercredit_userid",
                table: "usercredit",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "idx_users_email",
                table: "users",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "users_email_key",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "feedbacks");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "refreshtokens");

            migrationBuilder.DropTable(
                name: "usercredit");

            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "customerforms");

            migrationBuilder.DropTable(
                name: "packages");

            migrationBuilder.DropTable(
                name: "lawyers");

            migrationBuilder.DropTable(
                name: "services");

            migrationBuilder.DropTable(
                name: "lawform");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "durations");

            migrationBuilder.DropTable(
                name: "lawtypes");

            migrationBuilder.DropTable(
                name: "servicestypes");
        }
    }
}
