﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WorkBC.Data;

namespace WorkBC.Data.Migrations
{
    [DbContext(typeof(JobBoardContext))]
    [Migration("20200210220521_AddedJBFields")]
    partial class AddedJBFields
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(256)")
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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.AdminUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AdminLevel")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(40)")
                        .HasMaxLength(40);

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.Property<int>("LockBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LockDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SamAccountName")
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("AdminUsers");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.ConfigurationSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FieldType")
                        .HasColumnType("int");

                    b.Property<bool>("JbAccount")
                        .HasColumnType("bit");

                    b.Property<bool>("JbSearch")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ConfigurationSettings");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CountryTwoLetterCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.ExpiredJob", b =>
                {
                    b.Property<long>("JobId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ApiDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateFirstImported")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateLastImported")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateRemoved")
                        .HasColumnType("datetime2");

                    b.Property<string>("JobPostEnglish")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JobPostFrench")
                        .HasColumnType("nvarchar(max)");

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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateGeocoded")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsPermanent")
                        .HasColumnType("bit");

                    b.Property<string>("Latitude")
                        .HasColumnType("nvarchar(25)")
                        .HasMaxLength(25);

                    b.Property<string>("Longitude")
                        .HasColumnType("nvarchar(25)")
                        .HasMaxLength(25);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(120)")
                        .HasMaxLength(120);

                    b.HasKey("Id");

                    b.ToTable("GeocodedLocationCache");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.ImportedJobFederal", b =>
                {
                    b.Property<long>("JobId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ApiDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateFirstImported")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateLastImported")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DisplayUntil")
                        .HasColumnType("datetime2");

                    b.Property<string>("JobPostEnglish")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JobPostFrench")
                        .HasColumnType("nvarchar(max)");

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
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateFirstImported")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateLastImported")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsFederal")
                        .HasColumnType("bit");

                    b.Property<string>("JobPostEnglish")
                        .HasColumnType("nvarchar(max)");

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
                        .HasColumnType("nvarchar(80)")
                        .HasMaxLength(80);

                    b.Property<DateTime>("DateFirstImported")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateLastImported")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmployerName")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<DateTime>("ExpireDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalSourceName")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("ExternalSourceUrl")
                        .HasColumnType("nvarchar(800)")
                        .HasMaxLength(800);

                    b.Property<bool>("FullTime")
                        .HasColumnType("bit");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("LeadingToFullTime")
                        .HasColumnType("bit");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<short>("NaicsId")
                        .HasColumnType("smallint");

                    b.Property<short>("Noc2016")
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
                        .HasColumnType("nvarchar(60)")
                        .HasMaxLength(60);

                    b.Property<bool>("Seasonal")
                        .HasColumnType("bit");

                    b.Property<byte>("Source")
                        .HasColumnType("tinyint");

                    b.Property<bool>("Temporary")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(300)")
                        .HasMaxLength(300);

                    b.HasKey("JobId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobAlert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte>("AlertFrequency")
                        .HasColumnType("tinyint");

                    b.Property<string>("AspNetUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("ElasticSearchQuery")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ElasticSearchQueryVersion")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("UrlParameters")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.HasIndex("AspNetUserId");

                    b.ToTable("JobAlerts");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobSeeker", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<short>("AccountStatus")
                        .HasColumnType("smallint");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("LastLogon")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(50)")
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
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<int?>("ProvinceId")
                        .HasColumnType("int");

                    b.Property<string>("SecurityAnswer")
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<int?>("SecurityQuestionId")
                        .HasColumnType("int");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<Guid?>("VerificationGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AspNetUserId")
                        .HasColumnType("nvarchar(450)");

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
                        .HasColumnType("datetime2");

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
                        .HasColumnType("nvarchar(50)")
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
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Latitude")
                        .HasColumnType("nvarchar(25)")
                        .HasMaxLength(25);

                    b.Property<string>("Longitude")
                        .HasColumnType("nvarchar(25)")
                        .HasMaxLength(25);

                    b.Property<int>("RegionId")
                        .HasColumnType("int");

                    b.HasKey("LocationId");

                    b.ToTable("LocationLookups");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.NocCodes", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Code");

                    b.ToTable("NocCodes");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SavedJob", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AspNetUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateSaved")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<long>("JobId")
                        .HasColumnType("bigint");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(800)")
                        .HasMaxLength(800);

                    b.Property<DateTime?>("NoteUpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AspNetUserId");

                    b.ToTable("SavedJobs");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SecurityQuestion", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("QuestionText")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SecurityQuestions");
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

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobAlert", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker", "JobSeeker")
                        .WithMany()
                        .HasForeignKey("AspNetUserId");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobSeeker", b =>
                {
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

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SavedJob", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker", "JobSeeker")
                        .WithMany()
                        .HasForeignKey("AspNetUserId");
                });
#pragma warning restore 612, 618
        }
    }
}
