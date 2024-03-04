using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkBC.Data.Model.Enterprise;

namespace WorkBC.Data
{
    public partial class SsotContext : DbContext
    {
        private string ssotConnectionString;
        
        public SsotContext(DbContextOptions<SsotContext> options, IConfiguration configuration) 
            : base(options)
        {
            ssotConnectionString = configuration.GetConnectionString("SsotConnection");
        }

        public virtual DbSet<CareerProfile> CareerProfiles { get; set; }
        public virtual DbSet<IndustryProfile> IndustryProfiles { get; set; }
        public virtual DbSet<Noc> Nocs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ssotConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CareerProfile>(entity =>
            {
                entity.HasKey(e => e.CareerProfileId);

                entity.ToTable("edm_careerprofile");

                entity.HasIndex(e => e.NocId).IsUnique();
                
                entity.Property(e => e.CareerProfileId).HasColumnName("careerprofileid");

                entity.Property(e => e.AnnualSalarySource).HasMaxLength(255);

                entity.Property(e => e.CareerImage).HasColumnType("image");

                entity.Property(e => e.CareerImageHeadShot).HasColumnType("image");

                entity.Property(e => e.CareerLicensingId).HasColumnName("CareerLicensingID");

                entity.Property(e => e.CareerOutlookTitle).HasMaxLength(255);

                entity.Property(e => e.CareerTrekId)
                    .HasColumnName("CareerTrekID")
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CareerTrekImageUrl).HasColumnName("CareerTrekImageURL");

                entity.Property(e => e.EstimatedTime).HasMaxLength(255);

                entity.Property(e => e.HighDemandTitle).HasMaxLength(255);

                entity.Property(e => e.HighOpportunityNocgroupNocId).HasColumnName("HighOpportunityNOCGroupNOC_ID");

                entity.Property(e => e.IsFeatured).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsFeaturedCategory).HasDefaultValueSql("((0))");

                entity.Property(e => e.MinimumEducation).HasMaxLength(250);

                entity.Property(e => e.NocId).HasColumnName("noc_id");

                entity.Property(e => e.PageDescription).HasMaxLength(300);

                entity.Property(e => e.PageKeywords).HasMaxLength(135);

                entity.Property(e => e.PageTitle).HasMaxLength(62);

                entity.Property(e => e.RegionOutlookSummaryCar).HasColumnName("RegionOutlookSummaryCAR");

                entity.Property(e => e.RegionOutlookSummaryKoo).HasColumnName("RegionOutlookSummaryKOO");

                entity.Property(e => e.RegionOutlookSummaryMsw).HasColumnName("RegionOutlookSummaryMSW");

                entity.Property(e => e.RegionOutlookSummaryNcn).HasColumnName("RegionOutlookSummaryNCN");

                entity.Property(e => e.RegionOutlookSummaryNe).HasColumnName("RegionOutlookSummaryNE");

                entity.Property(e => e.RegionOutlookSummaryTok).HasColumnName("RegionOutlookSummaryTOK");

                entity.Property(e => e.RegionOutlookSummaryVi).HasColumnName("RegionOutlookSummaryVI");

                entity.Property(e => e.RegionalEmploymentCarall).HasColumnName("RegionalEmploymentCARAll");

                entity.Property(e => e.RegionalEmploymentCarthis).HasColumnName("RegionalEmploymentCARThis");

                entity.Property(e => e.RegionalEmploymentKooall).HasColumnName("RegionalEmploymentKOOAll");

                entity.Property(e => e.RegionalEmploymentKoothis).HasColumnName("RegionalEmploymentKOOThis");

                entity.Property(e => e.RegionalEmploymentMswall).HasColumnName("RegionalEmploymentMSWAll");

                entity.Property(e => e.RegionalEmploymentMswthis).HasColumnName("RegionalEmploymentMSWThis");

                entity.Property(e => e.RegionalEmploymentNcnall).HasColumnName("RegionalEmploymentNCNAll");

                entity.Property(e => e.RegionalEmploymentNcnthis).HasColumnName("RegionalEmploymentNCNThis");

                entity.Property(e => e.RegionalEmploymentTokall).HasColumnName("RegionalEmploymentTOKAll");

                entity.Property(e => e.RegionalEmploymentTokthis).HasColumnName("RegionalEmploymentTOKThis");

                entity.Property(e => e.RegionalEmploymentViall).HasColumnName("RegionalEmploymentVIAll");

                entity.Property(e => e.RegionalEmploymentVithis).HasColumnName("RegionalEmploymentVIThis");

                entity.Property(e => e.SiteId)
                    .HasColumnName("SiteID")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.Property(e => e.TotalApproximateFees).HasMaxLength(255);
                
            });

            modelBuilder.Entity<IndustryProfile>(entity =>
            {
                entity.HasKey(e => e.IndustryProfileId);

                entity.ToTable("edm_industryprofile");

                entity.HasIndex(e => e.NaicsId)
                    .IsUnique();

                entity.Property(e => e.IndustryProfileId).HasColumnName("IndustryProfileID");

                entity.Property(e => e.IndustryHeaderImage).HasColumnType("image");

                entity.Property(e => e.IndustryImage).HasColumnType("image");

                entity.Property(e => e.NaicsId).HasColumnName("NAICS_ID");

                entity.Property(e => e.PageDescription).HasMaxLength(300);

                entity.Property(e => e.PageKeywords).HasMaxLength(135);

                entity.Property(e => e.PageTitle).HasMaxLength(62);
            });

            modelBuilder.Entity<Noc>(entity =>
            {
                entity.HasKey(e => e.NocId);

                entity.ToTable("edm_noc");

                entity.HasIndex(e => e.NocgroupTypeId);

                entity.HasIndex(e => e.Nocs2006);

                entity.Property(e => e.NocId).HasColumnName("noc_id");

                entity.Property(e => e.EnglishShortAlias).HasMaxLength(200);

                entity.Property(e => e.NameEnglish).HasMaxLength(200);

                entity.Property(e => e.NameFrench).HasMaxLength(200);

                entity.Property(e => e.NocId2006).HasColumnName("NOC_ID2006");

                entity.Property(e => e.Noccode)
                    .IsRequired()
                    .HasColumnName("NOCCode")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.NocgroupTypeId).HasColumnName("NOCGroupTypeID");

                entity.Property(e => e.Nocs2006)
                    .HasColumnName("NOCS2006")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Nocyear)
                    .HasColumnName("NOCYear")
                    .HasDefaultValueSql("((2006))");

                entity.Property(e => e.ParentNocId).HasColumnName("ParentNOC_ID");

                entity.Property(e => e.SkillLevel)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.TradesOutlookNoc).HasColumnName("TradesOutlookNOC");

                entity.HasOne(d => d.NocId2006Navigation)
                    .WithMany(p => p.InverseNocId2006Navigation)
                    .HasForeignKey(d => d.NocId2006);

                entity.HasOne(d => d.ParentNoc)
                    .WithMany(p => p.InverseParentNoc)
                    .HasForeignKey(d => d.ParentNocId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}