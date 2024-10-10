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
    [Migration("20191028173223_AddTable_Country")]
    partial class AddTable_Country
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.AdminUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AdminLevel");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<bool>("Disabled");

                    b.Property<string>("Email")
                        .HasMaxLength(40);

                    b.Property<string>("FirstName")
                        .HasMaxLength(20);

                    b.Property<string>("LastName")
                        .HasMaxLength(20);

                    b.Property<string>("SamAccountName")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("AdminUsers");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("CountryTwoLetterCode");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.ExpiredJob", b =>
                {
                    b.Property<long>("JobId");

                    b.Property<DateTime>("DateRemoved");

                    b.Property<bool>("IsProcessed");

                    b.Property<string>("JobPostEnglish");

                    b.Property<string>("JobPostFrench");

                    b.Property<string>("JobSource");

                    b.HasKey("JobId");

                    b.ToTable("ExpiredJob");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.GeocodedLocationCache", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateGeocoded");

                    b.Property<string>("Latitude");

                    b.Property<string>("Longitude");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("GeocodedLocationCache");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.ImportedJobsFederal", b =>
                {
                    b.Property<long>("JobId");

                    b.Property<DateTime>("DateImported");

                    b.Property<DateTime?>("DisplayUntil");

                    b.Property<DateTime>("FileUpdateDate");

                    b.Property<string>("JobPostEnglish");

                    b.Property<string>("JobPostFrench")
                        .HasColumnName("JobPostFrench");

                    b.Property<bool>("ReIndexNeeded");

                    b.HasKey("JobId");

                    b.ToTable("ImportedJobsFederal");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.ImportedJobsWanted", b =>
                {
                    b.Property<long>("JobId");

                    b.Property<DateTime>("DateImported");

                    b.Property<DateTime>("DateRefreshed");

                    b.Property<string>("JobXml");

                    b.Property<bool>("ReIndexNeeded");

                    b.HasKey("JobId");

                    b.ToTable("ImportedJobsWanted");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.Job", b =>
                {
                    b.Property<long>("JobId");

                    b.Property<DateTime>("DatePosted");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<DateTime>("DisplayUntil");

                    b.Property<string>("EmployerName");

                    b.Property<string>("JobType");

                    b.Property<int>("Noc");

                    b.Property<int>("PositionsAvailable");

                    b.Property<string>("Title");

                    b.HasKey("JobId");

                    b.ToTable("Job");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobSeeker", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<short>("AccountStatus");

                    b.Property<string>("City")
                        .HasMaxLength(50);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<int?>("CountryId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("LastLogon");

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("LastName")
                        .HasMaxLength(50);

                    b.Property<int?>("LegacyWebUserId");

                    b.Property<int?>("LocationId");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<int?>("ProvinceId");

                    b.Property<string>("SecurityAnswer")
                        .HasMaxLength(50);

                    b.Property<int?>("SecurityQuestionId");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<Guid?>("VerificationGuid");

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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AspNetUserId");

                    b.Property<bool>("IsApprentice");

                    b.Property<bool>("IsIndigenousPerson");

                    b.Property<bool>("IsMatureWorker");

                    b.Property<bool>("IsNewImmigrant");

                    b.Property<bool>("IsPersonWithDisability");

                    b.Property<bool>("IsReservist");

                    b.Property<bool>("IsStudent");

                    b.Property<bool>("IsVeteran");

                    b.Property<bool>("IsVisibleMinority");

                    b.Property<bool>("IsYouth");

                    b.HasKey("Id");

                    b.HasIndex("AspNetUserId")
                        .IsUnique()
                        .HasFilter("[AspNetUserId] IS NOT NULL");

                    b.ToTable("JobSeekerFlags");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobSkill", b =>
                {
                    b.Property<long>("JobId");

                    b.Property<int>("SkillId");

                    b.HasKey("JobId", "SkillId");

                    b.ToTable("JobSkill");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.JobView", b =>
                {
                    b.Property<long>("JobId");

                    b.Property<DateTime>("DateLastViewed");

                    b.Property<int>("Views");

                    b.HasKey("JobId");

                    b.ToTable("JobView");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.NocCodes", b =>
                {
                    b.Property<string>("Code")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Title");

                    b.HasKey("Code");

                    b.ToTable("NocCodes");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SecurityQuestion", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("QuestionText");

                    b.HasKey("Id");

                    b.ToTable("SecurityQuestion");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.Skill", b =>
                {
                    b.Property<int>("SkillId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CategoryId");

                    b.Property<string>("Name");

                    b.HasKey("SkillId");

                    b.ToTable("Skill");
                });

            modelBuilder.Entity("WorkBC.Data.Model.JobBoard.SkillCategory", b =>
                {
                    b.Property<int>("CategoryId");

                    b.Property<string>("Name");

                    b.HasKey("CategoryId", "Name");

                    b.ToTable("SkillCategory");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("WorkBC.Data.Model.JobBoard.JobSeeker")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
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
#pragma warning restore 612, 618
        }
    }
}
