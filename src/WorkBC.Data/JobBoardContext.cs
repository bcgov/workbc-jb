using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Data.Model.JobBoard.ReportData;

namespace WorkBC.Data
{
    public class JobBoardContext : IdentityDbContext<JobSeeker, IdentityRole, string>, IDataProtectionKeyContext
    {
        private readonly string _connectionString;

        public JobBoardContext(DbContextOptions<JobBoardContext> options) : base(options)
        {
            try
            {
#pragma warning disable EF1001 // Internal EF Core API usage.
                _connectionString = options.GetExtension<SqlServerOptionsExtension>().ConnectionString;
#pragma warning restore EF1001 // Internal EF Core API usage.
            }
            catch
            {
                // ignore if using InMemory DB
            }
        }

        public JobBoardContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<AdminUser> AdminUsers { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        public DbSet<DeletedJob> DeletedJobs { get; set; }

        public DbSet<ExpiredJob> ExpiredJobs { get; set; }

        public DbSet<GeocodedLocationCache> GeocodedLocationCache { get; set; }

        public DbSet<Impersonation> ImpersonationLog { get; set; }

        public DbSet<ImportedJobFederal> ImportedJobsFederal { get; set; }

        public DbSet<ImportedJobWanted> ImportedJobsWanted { get; set; }

        public DbSet<Industry> Industries { get; set; }

        public DbSet<IndustryNaics> IndustryNaics { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<JobAlert> JobAlerts { get; set; }

        public DbSet<JobId> JobIds { get; set; }

        public DbSet<JobSeekerAdminComment> JobSeekerAdminComments { get; set; }

        public DbSet<JobSeekerChangeEvent> JobSeekerChangeLog { get; set; }

        public DbSet<JobSeekerEvent> JobSeekerEventLog { get; set; }

        public DbSet<JobSeekerFlags> JobSeekerFlags { get; set; }

        public DbSet<JobSeekerStatLabel> JobSeekerStatLabels { get; set; }

        public DbSet<JobSeekerStat> JobSeekerStats { get; set; }

        public DbSet<JobSeekerVersion> JobSeekerVersions { get; set; }

        public DbSet<JobSource> JobSources { get; set; }

        public DbSet<JobStat> JobStats { get; set; }

        public DbSet<JobVersion> JobVersions { get; set; }

        public DbSet<JobView> JobViews { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<NocCategory> NocCategories { get; set; }

        public DbSet<NocCode> NocCodes { get; set; }

        public DbSet<Province> Provinces { get; set; }

        public DbSet<Region> Regions { get; set; }

        public DbSet<ReportPersistenceControl> ReportPersistenceControl { get; set; }

        public DbSet<SavedCareerProfile> SavedCareerProfiles { get; set; }

        public DbSet<SavedIndustryProfile> SavedIndustryProfiles { get; set; }

        public DbSet<SavedJob> SavedJobs { get; set; }
        
        public DbSet<SecurityQuestion> SecurityQuestions { get; set; }

        public DbSet<SystemSetting> SystemSettings { get; set; }

        public DbSet<WeeklyPeriod> WeeklyPeriods { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GeocodedLocationCache>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<ImportedJobWanted>().HasIndex(x => x.HashId).IsUnique();
            modelBuilder.Entity<IndustryNaics>().HasKey(x => new { x.IndustryId, x.NaicsId });
            modelBuilder.Entity<JobAlert>().HasIndex(x => new { x.DateCreated });
            modelBuilder.Entity<JobStat>().HasKey(x => new { ReportWeeklyPeriodId = x.WeeklyPeriodId, x.RegionId, JobSource = x.JobSourceId });
            modelBuilder.Entity<JobSeeker>().HasIndex(x => x.Email).IsUnique();
            modelBuilder.Entity<JobSeekerEvent>().HasIndex(x => x.DateLogged);
            modelBuilder.Entity<JobSeekerStat>().HasKey(x => new { ReportWeeklyPeriodId = x.WeeklyPeriodId, ReportMetadataKey = x.LabelKey, x.RegionId });
            modelBuilder.Entity<JobSeekerVersion>().HasIndex(x => new { x.AspNetUserId, x.VersionNumber }).IsUnique();
            modelBuilder.Entity<JobVersion>().HasIndex(x => new { x.JobId, x.VersionNumber }).IsUnique();
            modelBuilder.Entity<JobView>().Property(et => et.JobId).ValueGeneratedNever();
            modelBuilder.Entity<ReportPersistenceControl>().HasKey(x => new { ReportWeeklyPeriodId = x.WeeklyPeriodId, Report = x.TableName });
            modelBuilder.Entity<SavedCareerProfile>().HasIndex(x => new { x.DateSaved });
            modelBuilder.Entity<SavedCareerProfile>().HasIndex(x => x.DateDeleted);
            modelBuilder.Entity<SavedIndustryProfile>().HasIndex(x => new { x.DateSaved });
            modelBuilder.Entity<SavedIndustryProfile>().HasIndex(x => x.DateDeleted);
            modelBuilder.Entity<SystemSetting>().HasAlternateKey(x => x.Name);
            modelBuilder.Entity<WeeklyPeriod>().HasIndex(x => x.WeekEndDate);

            // indexes for sorting job seekers on the admin screen

            // Status
            modelBuilder.Entity<JobSeeker>()
                .HasIndex(x => new { x.AccountStatus, x.LastName, x.FirstName })
                .IncludeProperties(x => new { x.Email });

            // First Name
            modelBuilder.Entity<JobSeeker>()
                .HasIndex(x => new { x.FirstName, x.LastName })
                .IncludeProperties(x => new { x.Email, x.AccountStatus });

            // Last Name
            modelBuilder.Entity<JobSeeker>()
                .HasIndex(x => new { x.LastName, x.FirstName })
                .IncludeProperties(x => new { x.Email, x.AccountStatus });

            // Email Address
            modelBuilder.Entity<JobSeeker>()
                .HasIndex(x => new { x.Email })
                .IncludeProperties(x => new { x.LastName, x.FirstName, x.AccountStatus });

            // Last Updated
            modelBuilder.Entity<JobSeeker>()
                .HasIndex(x => new { x.LastModified })
                .IncludeProperties(x => new { x.LastName, x.FirstName, x.Email, x.AccountStatus });

            // Registered Date
            modelBuilder.Entity<JobSeeker>()
                .HasIndex(x => new { x.DateRegistered })
                .IncludeProperties(x => new { x.LastName, x.FirstName, x.Email, x.AccountStatus });

            modelBuilder
                .Entity<AdminUser>()
                .HasOne(x => x.LockedByAdminUser)
                .WithMany()
                .HasForeignKey("LockedByAdminUserId")
                .HasConstraintName("FK_AdminUsers_AdminUsers_LockedByAdminUserId");

            modelBuilder
                .Entity<AdminUser>()
                .HasOne(x => x.ModifiedByAdminUser)
                .WithMany()
                .HasForeignKey("ModifiedByAdminUserId")
                .HasConstraintName("FK_AdminUsers_AdminUsers_ModifiedByAdminUserId");
        }
    }
}