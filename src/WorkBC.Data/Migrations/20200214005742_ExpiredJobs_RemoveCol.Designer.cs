﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WorkBC.Data;

namespace WorkBC.Data.Migrations
{
    [DbContext(typeof(JobBoardContext))]
    [Migration("20200214005742_ExpiredJobs_RemoveCol")]
    partial class ExpiredJobs_RemoveCol
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("varchar");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("varchar");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("varchar");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("varchar");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("varchar");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("varchar");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasColumnType("varchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Value")
                        .HasColumnType("varchar");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.AdminUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AdminLevel")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("DateLocked")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("timestamp");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(40)")
                        .HasMaxLength(40);

                    b.Property<string>("FirstName")
                        .HasColumnType("varchar(20)")
                        .HasMaxLength(20);

                    b.Property<string>("Guid")
                        .HasColumnType("varchar(40)")
                        .HasMaxLength(40);

                    b.Property<string>("LastName")
                        .HasColumnType("varchar(20)")
                        .HasMaxLength(20);

                    b.Property<int?>("LockedByAdminUserId")
                        .HasColumnType("int");

                    b.Property<int?>("ModifiedByAdminUserId")
                        .HasColumnType("int");

                    b.Property<string>("SamAccountName")
                        .HasColumnType("varchar(20)")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.HasIndex("LockedByAdminUserId");

                    b.HasIndex("ModifiedByAdminUserId");

                    b.ToTable("AdminUsers");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("CountryTwoLetterCode")
                        .HasColumnType("varchar(2)")
                        .HasMaxLength(2);

                    b.Property<string>("Name")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.ExpiredJob", b =>
                {
                    b.Property<long>("JobId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ApiDate")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("DateLastImported")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("DateRemoved")
                        .HasColumnType("timestamp");

                    b.Property<string>("JobPostEnglish")
                        .HasColumnType("varchar");

                    b.Property<string>("JobPostFrench")
                        .HasColumnType("varchar");

                    b.Property<byte>("JobSource")
                        .HasColumnType("tinyint");

                    b.Property<bool>("RemovedFromIndex")
                        .HasColumnType("bit");

                    b.HasKey("JobId");

                    b.ToTable("ExpiredJobs");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.GeocodedLocationCache", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateGeocoded")
                        .HasColumnType("timestamp");

                    b.Property<bool>("IsPermanent")
                        .HasColumnType("bit");

                    b.Property<string>("Latitude")
                        .HasColumnType("varchar(25)")
                        .HasMaxLength(25);

                    b.Property<string>("Longitude")
                        .HasColumnType("varchar(25)")
                        .HasMaxLength(25);

                    b.Property<string>("Name")
                        .HasColumnType("varchar(120)")
                        .HasMaxLength(120);

                    b.HasKey("Id");

                    b.ToTable("GeocodedLocationCache");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.ImportedJobFederal", b =>
                {
                    b.Property<long>("JobId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ApiDate")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("DateFirstImported")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("DateLastImported")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("DisplayUntil")
                        .HasColumnType("timestamp");

                    b.Property<string>("JobPostEnglish")
                        .HasColumnType("varchar");

                    b.Property<string>("JobPostFrench")
                        .HasColumnType("varchar");

                    b.Property<bool>("ReIndexNeeded")
                        .HasColumnType("bit");

                    b.HasKey("JobId");

                    b.ToTable("ImportedJobsFederal");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.ImportedJobWanted", b =>
                {
                    b.Property<long>("JobId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ApiDate")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("DateFirstImported")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("DateLastImported")
                        .HasColumnType("timestamp");

                    b.Property<bool>("IsFederal")
                        .HasColumnType("bit");

                    b.Property<string>("JobPostEnglish")
                        .HasColumnType("varchar");

                    b.Property<bool>("ReIndexNeeded")
                        .HasColumnType("bit");

                    b.HasKey("JobId");

                    b.ToTable("ImportedJobsWanted");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.Job", b =>
                {
                    b.Property<long>("JobId")
                        .HasColumnType("bigint");

                    b.Property<bool>("Casual")
                        .HasColumnType("bit");

                    b.Property<string>("City")
                        .HasColumnType("varchar(80)")
                        .HasMaxLength(80);

                    b.Property<DateTime>("DateFirstImported")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("DateLastImported")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("timestamp");

                    b.Property<string>("EmployerName")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<DateTime>("ExpireDate")
                        .HasColumnType("timestamp");

                    b.Property<string>("ExternalSourceName")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("ExternalSourceUrl")
                        .HasColumnType("varchar(800)")
                        .HasMaxLength(800);

                    b.Property<bool>("FullTime")
                        .HasColumnType("bit");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp");

                    b.Property<bool>("LeadingToFullTime")
                        .HasColumnType("bit");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<short>("NaicsId")
                        .HasColumnType("smallint");

                    b.Property<short>("NocCode")
                        .HasColumnType("smallint");

                    b.Property<bool>("PartTime")
                        .HasColumnType("bit");

                    b.Property<bool>("Permanent")
                        .HasColumnType("bit");

                    b.Property<short>("PositionsAvailable")
                        .HasColumnType("smallint");

                    b.Property<decimal?>("Salary")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("SalarySummary")
                        .HasColumnType("varchar(60)")
                        .HasMaxLength(60);

                    b.Property<bool>("Seasonal")
                        .HasColumnType("bit");

                    b.Property<byte>("Source")
                        .HasColumnType("tinyint");

                    b.Property<bool>("Temporary")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .HasColumnType("varchar(300)")
                        .HasMaxLength(300);

                    b.HasKey("JobId");

                    b.HasIndex("LocationId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobAlert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<byte>("AlertFrequency")
                        .HasColumnType("tinyint");

                    b.Property<string>("AspNetUserId")
                        .HasColumnType("varchar(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("timestamp");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("JobSearchFilters")
                        .HasColumnType("varchar");

                    b.Property<int>("JobSearchFiltersVersion")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("UrlParameters")
                        .HasColumnType("varchar(1000)")
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.HasIndex("AspNetUserId");

                    b.ToTable("JobAlerts");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobId", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("DateFirstSeen")
                        .HasColumnType("timestamp");

                    b.Property<byte>("JobSource")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.ToTable("JobIds");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobSeeker", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<short>("AccountStatus")
                        .HasColumnType("smallint");

                    b.Property<string>("City")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("varchar");

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("LastLogon")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp");

                    b.Property<string>("LastName")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<int?>("LegacyWebUserId")
                        .HasColumnType("int");

                    b.Property<int?>("LocationId")
                        .HasColumnType("int");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("varchar");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("varchar");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<int?>("ProvinceId")
                        .HasColumnType("int");

                    b.Property<string>("SecurityAnswer")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<int?>("SecurityQuestionId")
                        .HasColumnType("int");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("varchar");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<Guid?>("VerificationGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.HasIndex("LocationId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("SecurityQuestionId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobSeekerFlags", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AspNetUserId")
                        .HasColumnType("varchar(450)");

                    b.Property<bool>("IsApprentice")
                        .HasColumnType("bit");

                    b.Property<bool>("IsIndigenousPerson")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMatureWorker")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNewImmigrant")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPersonWithDisability")
                        .HasColumnType("bit");

                    b.Property<bool>("IsReservist")
                        .HasColumnType("bit");

                    b.Property<bool>("IsStudent")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVeteran")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVisibleMinority")
                        .HasColumnType("bit");

                    b.Property<bool>("IsYouth")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AspNetUserId")
                        .IsUnique()
                        .HasFilter("[AspNetUserId] IS NOT NULL");

                    b.ToTable("JobSeekerFlags");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobView", b =>
                {
                    b.Property<long>("JobId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("DateLastViewed")
                        .HasColumnType("timestamp");

                    b.Property<int>("Views")
                        .HasColumnType("int");

                    b.HasKey("JobId");

                    b.ToTable("JobViews");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.LocationLookup", b =>
                {
                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<int>("DistrictId")
                        .HasColumnType("int");

                    b.Property<int?>("FederalCityId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDuplicate")
                        .HasColumnType("bit");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("bit");

                    b.Property<string>("Label")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Latitude")
                        .HasColumnType("varchar(25)")
                        .HasMaxLength(25);

                    b.Property<string>("Longitude")
                        .HasColumnType("varchar(25)")
                        .HasMaxLength(25);

                    b.Property<int>("RegionId")
                        .HasColumnType("int");

                    b.HasKey("LocationId");

                    b.ToTable("LocationLookups");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.NocCode", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("varchar(4)")
                        .HasMaxLength(4);

                    b.Property<string>("Title")
                        .HasColumnType("varchar(150)")
                        .HasMaxLength(150);

                    b.HasKey("Code");

                    b.ToTable("NocCodes");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SavedCareerProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AspNetUserId")
                        .HasColumnType("varchar(450)");

                    b.Property<int>("CareerProfileId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("DateSaved")
                        .HasColumnType("timestamp");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AspNetUserId");

                    b.ToTable("SavedCareerProfiles");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SavedIndustryProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AspNetUserId")
                        .HasColumnType("varchar(450)");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("DateSaved")
                        .HasColumnType("timestamp");

                    b.Property<int>("IndustryProfileId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AspNetUserId");

                    b.ToTable("SavedIndustryProfiles");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SavedJob", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AspNetUserId")
                        .HasColumnType("varchar(450)");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("DateSaved")
                        .HasColumnType("timestamp");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<long>("JobId")
                        .HasColumnType("bigint");

                    b.Property<string>("Note")
                        .HasColumnType("varchar(800)")
                        .HasMaxLength(800);

                    b.Property<DateTime?>("NoteUpdatedDate")
                        .HasColumnType("timestamp");

                    b.HasKey("Id");

                    b.HasIndex("AspNetUserId");

                    b.HasIndex("JobId");

                    b.ToTable("SavedJobs");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SecurityQuestion", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("QuestionText")
                        .HasColumnType("varchar(40)")
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.ToTable("SecurityQuestions");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SystemSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("timestamp");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(2000)")
                        .HasMaxLength(2000);

                    b.Property<int>("FieldType")
                        .HasColumnType("int");

                    b.Property<int>("ModifiedByAdminUserId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(400)")
                        .HasMaxLength(400);

                    b.Property<string>("Value")
                        .HasColumnType("varchar");

                    b.HasKey("Id");

                    b.HasAlternateKey("Name");

                    b.HasIndex("ModifiedByAdminUserId");

                    b.ToTable("SystemSettings");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.AdminUser", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.AdminUser", "LockedByAdminUser")
                        .WithMany()
                        .HasForeignKey("LockedByAdminUserId");

                    b.HasOne("WorkBC.Data.Model.JobBoard.AdminUser", "ModifiedByAdminUser")
                        .WithMany()
                        .HasForeignKey("ModifiedByAdminUserId");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.ExpiredJob", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobId", "Id")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.ImportedJobFederal", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobId", "Id")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.ImportedJobWanted", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobId", "Id")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.Job", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobId", "Id")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WorkBC.Data.Model.JobBoard.LocationLookup", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobAlert", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker", "JobSeeker")
                        .WithMany()
                        .HasForeignKey("AspNetUserId");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobSeeker", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId");

                    b.HasOne("WorkBC.Data.Model.JobBoard.LocationLookup", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.HasOne("WorkBC.Data.Model.JobBoard.SecurityQuestion", "SecurityQuestion")
                        .WithMany()
                        .HasForeignKey("SecurityQuestionId");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobSeekerFlags", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker", "JobSeeker")
                        .WithOne("JobSeekerFlags")
                        .HasForeignKey("WorkBC.Data.Model.JobBoard.JobSeekerFlags", "AspNetUserId");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobView", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SavedCareerProfile", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker", "JobSeeker")
                        .WithMany()
                        .HasForeignKey("AspNetUserId");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SavedIndustryProfile", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker", "JobSeeker")
                        .WithMany()
                        .HasForeignKey("AspNetUserId");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SavedJob", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker", "JobSeeker")
                        .WithMany()
                        .HasForeignKey("AspNetUserId");

                    b.HasOne("WorkBC.Data.Model.JobBoard.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SystemSetting", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.AdminUser", "ModifiedByAdminUser")
                        .WithMany()
                        .HasForeignKey("ModifiedByAdminUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
