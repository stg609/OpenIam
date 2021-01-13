using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Charlie.OpenIam.Infra.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    name = table.Column<string>(maxLength: 256, nullable: true),
                    normalizedname = table.Column<string>(maxLength: 256, nullable: true),
                    concurrencystamp = table.Column<string>(nullable: true),
                    clientid = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    isadmin = table.Column<bool>(nullable: false),
                    issuperadmin = table.Column<bool>(nullable: false),
                    createdby = table.Column<string>(nullable: true),
                    createdat = table.Column<DateTime>(nullable: false),
                    lastupdatedby = table.Column<string>(nullable: true),
                    lastupdatedat = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    username = table.Column<string>(maxLength: 256, nullable: true),
                    normalizedusername = table.Column<string>(maxLength: 256, nullable: true),
                    email = table.Column<string>(maxLength: 256, nullable: true),
                    normalizedemail = table.Column<string>(maxLength: 256, nullable: true),
                    emailconfirmed = table.Column<bool>(nullable: false),
                    passwordhash = table.Column<string>(nullable: true),
                    securitystamp = table.Column<string>(nullable: true),
                    concurrencystamp = table.Column<string>(nullable: true),
                    phonenumber = table.Column<string>(nullable: true),
                    phonenumberconfirmed = table.Column<bool>(nullable: false),
                    twofactorenabled = table.Column<bool>(nullable: false),
                    lockoutend = table.Column<DateTimeOffset>(nullable: true),
                    lockoutenabled = table.Column<bool>(nullable: false),
                    accessfailedcount = table.Column<int>(nullable: false),
                    jobno = table.Column<string>(nullable: true),
                    firstname = table.Column<string>(nullable: true),
                    lastname = table.Column<string>(nullable: true),
                    gender = table.Column<int>(nullable: false),
                    position = table.Column<string>(nullable: true),
                    idcard = table.Column<string>(nullable: true),
                    idcardfaceimg = table.Column<string>(nullable: true),
                    idcardbackimg = table.Column<string>(nullable: true),
                    homeaddress = table.Column<string>(nullable: true),
                    lastip = table.Column<string>(nullable: true),
                    lastloginat = table.Column<DateTime>(nullable: false),
                    isactive = table.Column<bool>(nullable: false),
                    createdby = table.Column<string>(nullable: true),
                    createdat = table.Column<DateTime>(nullable: false),
                    lastupdatedby = table.Column<string>(nullable: true),
                    lastupdatedat = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organizations",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    desc = table.Column<string>(nullable: true),
                    enabled = table.Column<bool>(nullable: false),
                    mobile = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    parentid = table.Column<string>(nullable: true),
                    createdby = table.Column<string>(nullable: true),
                    createdat = table.Column<DateTime>(nullable: false),
                    lastupdatedby = table.Column<string>(nullable: true),
                    lastupdatedat = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organizations", x => x.id);
                    table.ForeignKey(
                        name: "fk_organizations_organizations_parentid",
                        column: x => x.parentid,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    key = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    enabled = table.Column<bool>(nullable: false),
                    clientid = table.Column<string>(nullable: true),
                    type = table.Column<int>(nullable: false),
                    parentid = table.Column<string>(nullable: true),
                    url = table.Column<string>(nullable: true),
                    icon = table.Column<string>(nullable: true),
                    order = table.Column<int>(nullable: false),
                    level = table.Column<int>(nullable: false),
                    createdby = table.Column<string>(nullable: true),
                    createdat = table.Column<DateTime>(nullable: false),
                    lastupdatedby = table.Column<string>(nullable: true),
                    lastupdatedat = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_permissions_permissions_parentid",
                        column: x => x.parentid,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sysinfo",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    isjobnounique = table.Column<bool>(nullable: false),
                    isuserphoneunique = table.Column<bool>(nullable: false),
                    isphonepwdloginenabled = table.Column<bool>(nullable: false),
                    isjobnopwdloginenabled = table.Column<bool>(nullable: false),
                    isregisteruserenabled = table.Column<bool>(nullable: false),
                    createdby = table.Column<string>(nullable: true),
                    createdat = table.Column<DateTime>(nullable: false),
                    lastupdatedby = table.Column<string>(nullable: true),
                    lastupdatedat = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sysinfo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roleid = table.Column<string>(nullable: false),
                    claimtype = table.Column<string>(nullable: true),
                    claimvalue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roleclaims", x => x.id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_roleid",
                        column: x => x.roleid,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<string>(nullable: false),
                    claimtype = table.Column<string>(nullable: true),
                    claimvalue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_userclaims", x => x.id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_userid",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    loginprovider = table.Column<string>(nullable: false),
                    providerkey = table.Column<string>(nullable: false),
                    providerdisplayname = table.Column<string>(nullable: true),
                    userid = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.loginprovider, x.providerkey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_userid",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    userid = table.Column<string>(nullable: false),
                    roleid = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.userid, x.roleid });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_roleid",
                        column: x => x.roleid,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_userid",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    userid = table.Column<string>(nullable: false),
                    loginprovider = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.userid, x.loginprovider, x.name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_userid",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "organizationrole",
                columns: table => new
                {
                    roleid = table.Column<string>(nullable: false),
                    organizationid = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organizationrole", x => new { x.roleid, x.organizationid });
                    table.ForeignKey(
                        name: "fk_organizationrole_organizations_organizationid",
                        column: x => x.organizationid,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_organizationrole_roles_roleid",
                        column: x => x.roleid,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userorganization",
                columns: table => new
                {
                    userid = table.Column<string>(nullable: false),
                    organizationid = table.Column<string>(nullable: false),
                    ischarger = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userorganization", x => new { x.userid, x.organizationid });
                    table.ForeignKey(
                        name: "fk_userorganization_organizations_organizationid",
                        column: x => x.organizationid,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_userorganization_users_userid",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rolepermission",
                columns: table => new
                {
                    roleid = table.Column<string>(nullable: false),
                    permissionid = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rolepermission", x => new { x.roleid, x.permissionid });
                    table.ForeignKey(
                        name: "fk_rolepermission_permissions_permissionid",
                        column: x => x.permissionid,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rolepermission_roles_roleid",
                        column: x => x.roleid,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userpermission",
                columns: table => new
                {
                    userid = table.Column<string>(nullable: false),
                    permissionid = table.Column<string>(nullable: false),
                    permissionroleids = table.Column<string[]>(nullable: true),
                    action = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userpermission", x => new { x.userid, x.permissionid });
                    table.ForeignKey(
                        name: "fk_userpermission_permissions_permissionid",
                        column: x => x.permissionid,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_userpermission_users_userid",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_roleid",
                table: "AspNetRoleClaims",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "normalizedname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_userid",
                table: "AspNetUserClaims",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_userid",
                table: "AspNetUserLogins",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_roleid",
                table: "AspNetUserRoles",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_jobno",
                table: "AspNetUsers",
                column: "jobno");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "normalizedemail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "normalizedusername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organizationrole_organizationid",
                table: "organizationrole",
                column: "organizationid");

            migrationBuilder.CreateIndex(
                name: "IX_organizations_parentid",
                table: "organizations",
                column: "parentid");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_parentid",
                table: "permissions",
                column: "parentid");

            migrationBuilder.CreateIndex(
                name: "ix_rolepermission_permissionid",
                table: "rolepermission",
                column: "permissionid");

            migrationBuilder.CreateIndex(
                name: "ix_userorganization_organizationid",
                table: "userorganization",
                column: "organizationid");

            migrationBuilder.CreateIndex(
                name: "ix_userpermission_permissionid",
                table: "userpermission",
                column: "permissionid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "organizationrole");

            migrationBuilder.DropTable(
                name: "rolepermission");

            migrationBuilder.DropTable(
                name: "sysinfo");

            migrationBuilder.DropTable(
                name: "userorganization");

            migrationBuilder.DropTable(
                name: "userpermission");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "organizations");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
