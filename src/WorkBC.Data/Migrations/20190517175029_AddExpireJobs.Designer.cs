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
    [Migration("20190517175029_AddExpireJobs")]
    partial class AddExpireJobs
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("WorkBC.Data.Model.ExpiredJob", b =>
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

            modelBuilder.Entity("WorkBC.Data.Model.ImportedJobsFederal", b =>
                {
                    b.Property<int>("JobId");

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

            modelBuilder.Entity("WorkBC.Data.Model.ImportedJobsWanted", b =>
                {
                    b.Property<long>("JobId");

                    b.Property<DateTime>("DateImported");

                    b.Property<DateTime>("DateRefreshed");

                    b.Property<string>("JobXml");

                    b.Property<bool>("ReIndexNeeded");

                    b.HasKey("JobId");

                    b.ToTable("ImportedJobsWanted");
                });

            modelBuilder.Entity("WorkBC.Data.Model.Job", b =>
                {
                    b.Property<int>("JobId");

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

            modelBuilder.Entity("WorkBC.Data.Model.JobSkill", b =>
                {
                    b.Property<int>("JobId");

                    b.Property<int>("SkillId");

                    b.HasKey("JobId", "SkillId");

                    b.ToTable("JobSkill");
                });

            modelBuilder.Entity("WorkBC.Data.Model.Skill", b =>
                {
                    b.Property<int>("SkillId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CategoryId");

                    b.Property<string>("Name");

                    b.HasKey("SkillId");

                    b.ToTable("Skill");
                });

            modelBuilder.Entity("WorkBC.Data.Model.SkillCategory", b =>
                {
                    b.Property<int>("CategoryId");

                    b.Property<string>("Name");

                    b.HasKey("CategoryId", "Name");

                    b.ToTable("SkillCategory");
                });
#pragma warning restore 612, 618
        }
    }
}
