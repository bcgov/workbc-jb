-- ------------ Write DROP-FUNCTION-stage scripts -----------

DROP ROUTINE IF EXISTS public."tvf_GetJobSeekersForDate"(IN TIMESTAMP WITHOUT TIME ZONE, OUT VARCHAR, OUT VARCHAR, OUT INTEGER, OUT INTEGER, OUT INTEGER, OUT TIMESTAMP WITHOUT TIME ZONE, OUT SMALLINT, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC);

DROP ROUTINE IF EXISTS public."tvf_GetJobsForDate"(IN TIMESTAMP WITHOUT TIME ZONE, OUT BIGINT, OUT SMALLINT, OUT INTEGER, OUT INTEGER, OUT SMALLINT, OUT TIMESTAMP WITHOUT TIME ZONE, OUT SMALLINT);

-- ------------ Write DROP-PROCEDURE-stage scripts -----------

DROP ROUTINE IF EXISTS public."usp_GenerateJobSeekerStats"(IN TIMESTAMP WITHOUT TIME ZONE, INOUT int);

DROP ROUTINE IF EXISTS public."usp_GenerateJobStats"(IN TIMESTAMP WITHOUT TIME ZONE, INOUT int);

-- ------------ Write DROP-FOREIGN-KEY-CONSTRAINT-stage scripts -----------

ALTER TABLE public."AdminUsers" DROP CONSTRAINT "FK_AdminUsers_AdminUsers_LockedByAdminUserId_1346103836";

ALTER TABLE public."AdminUsers" DROP CONSTRAINT "FK_AdminUsers_AdminUsers_ModifiedByAdminUserId_1378103950";

ALTER TABLE public."AspNetRoleClaims" DROP CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId_1829581556";

ALTER TABLE public."AspNetUserClaims" DROP CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId_1877581727";

ALTER TABLE public."AspNetUserLogins" DROP CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId_1925581898";

ALTER TABLE public."AspNetUserRoles" DROP CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId_1973582069";

ALTER TABLE public."AspNetUserRoles" DROP CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId_1989582126";

ALTER TABLE public."AspNetUserTokens" DROP CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId_2037582297";

ALTER TABLE public."AspNetUsers" DROP CONSTRAINT "FK_AspNetUsers_AdminUsers_LockedByAdminUserId_2050106344";

ALTER TABLE public."AspNetUsers" DROP CONSTRAINT "FK_AspNetUsers_Countries_CountryId_414624520";

ALTER TABLE public."AspNetUsers" DROP CONSTRAINT "FK_AspNetUsers_LocationLookups_LocationId_190623722";

ALTER TABLE public."AspNetUsers" DROP CONSTRAINT "FK_AspNetUsers_Provinces_ProvinceId_206623779";

ALTER TABLE public."AspNetUsers" DROP CONSTRAINT "FK_AspNetUsers_SecurityQuestions_SecurityQuestionId_850102069";

ALTER TABLE public."DeletedJobs" DROP CONSTRAINT "FK_DeletedJobs_AdminUsers_DeletedByAdminUserId_974626515";

ALTER TABLE public."DeletedJobs" DROP CONSTRAINT "FK_DeletedJobs_Jobs_JobId_990626572";

ALTER TABLE public."ExpiredJobs" DROP CONSTRAINT "FK_ExpiredJobs_JobIds_JobId_1726629194";

ALTER TABLE public."ImpersonationLog" DROP CONSTRAINT "FK_ImpersonationLog_AdminUsers_AdminUserId_1038626743";

ALTER TABLE public."ImpersonationLog" DROP CONSTRAINT "FK_ImpersonationLog_AspNetUsers_AspNetUserId_1054626800";

ALTER TABLE public."ImportedJobsFederal" DROP CONSTRAINT "FK_ImportedJobsFederal_JobIds_JobId_1570104634";

ALTER TABLE public."ImportedJobsWanted" DROP CONSTRAINT "FK_ImportedJobsWanted_JobIds_JobId_1586104691";

ALTER TABLE public."JobAlerts" DROP CONSTRAINT "FK_JobAlerts_AspNetUsers_AspNetUserId_1122103038";

ALTER TABLE public."JobIds" DROP CONSTRAINT "FK_JobIds_JobSources_JobSourceId_766625774";

ALTER TABLE public."JobSeekerAdminComments" DROP CONSTRAINT "FK_JobSeekerAdminComments_AdminUsers_EnteredByAdminUserId_1906105831";

ALTER TABLE public."JobSeekerAdminComments" DROP CONSTRAINT "FK_JobSeekerAdminComments_AspNetUsers_AspNetUserId_1890105774";

ALTER TABLE public."JobSeekerChangeLog" DROP CONSTRAINT "FK_JobSeekerAdminLog_AspNetUsers_AspNetUserId_1662628966";

ALTER TABLE public."JobSeekerChangeLog" DROP CONSTRAINT "FK_JobSeekerChangeLog_AdminUsers_ModifiedByAdminUserId_1630628852";

ALTER TABLE public."JobSeekerEventLog" DROP CONSTRAINT "FK_JobSeekerEventLog_AspNetUsers_AspNetUserId_2018106230";

ALTER TABLE public."JobSeekerFlags" DROP CONSTRAINT "FK_JobSeekerFlags_AspNetUsers_AspNetUserId_178099675";

ALTER TABLE public."JobSeekerStats" DROP CONSTRAINT "FK_JobSeekerStats_JobSeekerStatLabels_LabelKey_1454628225";

ALTER TABLE public."JobSeekerStats" DROP CONSTRAINT "FK_JobSeekerStats_Regions_RegionId_1502628396";

ALTER TABLE public."JobSeekerStats" DROP CONSTRAINT "FK_JobSeekerStats_WeeklyPeriods_WeeklyPeriodId_1262627541";

ALTER TABLE public."JobSeekerVersions" DROP CONSTRAINT "FK_JobSeekerVersions_AspNetUsers_AspNetUserId_302624121";

ALTER TABLE public."JobSeekerVersions" DROP CONSTRAINT "FK_JobSeekerVersions_Countries_CountryId_430624577";

ALTER TABLE public."JobSeekerVersions" DROP CONSTRAINT "FK_JobSeekerVersions_Locations_LocationId_366624349";

ALTER TABLE public."JobSeekerVersions" DROP CONSTRAINT "FK_JobSeekerVersions_Provinces_ProvinceId_350624292";

ALTER TABLE public."JobStats" DROP CONSTRAINT "FK_JobStats_JobSources_JobSourceId_1358627883";

ALTER TABLE public."JobStats" DROP CONSTRAINT "FK_JobStats_Regions_RegionId_1518628453";

ALTER TABLE public."JobStats" DROP CONSTRAINT "FK_JobStats_WeeklyPeriods_WeeklyPeriodId_1374627940";

ALTER TABLE public."JobVersions" DROP CONSTRAINT "FK_JobVersions_Jobs_JobId_62623266";

ALTER TABLE public."JobVersions" DROP CONSTRAINT "FK_JobVersions_LocationLookups_LocationId_238623893";

ALTER TABLE public."JobVersions" DROP CONSTRAINT "FK_JobVersions_NaicsCodes_NaicsId_94623380";

ALTER TABLE public."JobVersions" DROP CONSTRAINT "FK_JobVersions_NocCodes2021_NocCodeId2021_891150220";

ALTER TABLE public."JobVersions" DROP CONSTRAINT "FK_JobVersions_NocCodes_NocCodeId_110623437";

ALTER TABLE public."JobViews" DROP CONSTRAINT "FK_JobViews_Jobs_JobId_1442104178";

ALTER TABLE public."Jobs" DROP CONSTRAINT "FK_Jobs_JobIds_JobId_1602104748";

ALTER TABLE public."Jobs" DROP CONSTRAINT "FK_Jobs_JobSources_JobSourceId_782625831";

ALTER TABLE public."Jobs" DROP CONSTRAINT "FK_Jobs_LocationLookups_LocationId_222623836";

ALTER TABLE public."Jobs" DROP CONSTRAINT "FK_Jobs_NaicsCodes_NaicsId_14623095";

ALTER TABLE public."Jobs" DROP CONSTRAINT "FK_Jobs_NocCodes2021_NocCodeId2021_875150163";

ALTER TABLE public."Jobs" DROP CONSTRAINT "FK_Jobs_NocCodes_NocCodeId_1650104919";

ALTER TABLE public."Locations" DROP CONSTRAINT "FK_LocationLookups_Regions_RegionId_254623950";

ALTER TABLE public."Locations" DROP CONSTRAINT "FK_Locations_Regions_RegionId_446624634";

ALTER TABLE public."ReportPersistenceControl" DROP CONSTRAINT "FK_ReportPersistenceControl_WeeklyPeriods_WeeklyPeriodId_1150627142";

ALTER TABLE public."SavedCareerProfiles" DROP CONSTRAINT "FK_SavedCareerProfiles_AspNetUsers_AspNetUserId_1458104235";

ALTER TABLE public."SavedCareerProfiles" DROP CONSTRAINT "FK_SavedCareerProfiles_NocCodes2021_Id_907150277";

ALTER TABLE public."SavedIndustryProfiles" DROP CONSTRAINT "FK_SavedIndustryProfiles_AspNetUsers_AspNetUserId_1474104292";

ALTER TABLE public."SavedIndustryProfiles" DROP CONSTRAINT "FK_SavedIndustryProfiles_Industries_Id_923150334";

ALTER TABLE public."SavedJobs" DROP CONSTRAINT "FK_SavedJobs_AspNetUsers_AspNetUserId_514100872";

ALTER TABLE public."SavedJobs" DROP CONSTRAINT "FK_SavedJobs_Jobs_JobId_1362103893";

ALTER TABLE public."SystemSettings" DROP CONSTRAINT "FK_SystemSettings_AdminUsers_ModifiedByAdminUserId_1490104349";

-- ------------ Write DROP-CONSTRAINT-stage scripts -----------

ALTER TABLE public."AdminUsers" DROP CONSTRAINT "PK_AdminUsers_322100188";

ALTER TABLE public."AspNetRoleClaims" DROP CONSTRAINT "PK_AspNetRoleClaims_1813581499";

ALTER TABLE public."AspNetRoles" DROP CONSTRAINT "PK_AspNetRoles_1749581271";

ALTER TABLE public."AspNetUserClaims" DROP CONSTRAINT "PK_AspNetUserClaims_1861581670";

ALTER TABLE public."AspNetUserLogins" DROP CONSTRAINT "PK_AspNetUserLogins_418100530";

ALTER TABLE public."AspNetUserRoles" DROP CONSTRAINT "PK_AspNetUserRoles_1957582012";

ALTER TABLE public."AspNetUserTokens" DROP CONSTRAINT "PK_AspNetUserTokens_402100473";

ALTER TABLE public."AspNetUsers" DROP CONSTRAINT "PK_AspNetUsers_1781581385";

ALTER TABLE public."Countries" DROP CONSTRAINT "PK_Countries_398624463";

ALTER TABLE public."DataProtectionKeys" DROP CONSTRAINT "PK_DataProtectionKeys_2078630448";

ALTER TABLE public."DeletedJobs" DROP CONSTRAINT "PK_DeletedJobs_958626458";

ALTER TABLE public."ExpiredJobs" DROP CONSTRAINT "PK_ExpiredJobs_1710629137";

ALTER TABLE public."GeocodedLocationCache" DROP CONSTRAINT "PK_GeocodedLocationCache_1669580986";

ALTER TABLE public."ImpersonationLog" DROP CONSTRAINT "PK_ImpersonationLog_1022626686";

ALTER TABLE public."ImportedJobsFederal" DROP CONSTRAINT "PK_ImportedJobsFederal_290100074";

ALTER TABLE public."ImportedJobsWanted" DROP CONSTRAINT "PK_ImportedJobsWanted_1509580416";

ALTER TABLE public."Industries" DROP CONSTRAINT "PK_NaicsCodes_2146106686";

ALTER TABLE public."JobAlerts" DROP CONSTRAINT "PK_JobAlerts_1106102981";

ALTER TABLE public."JobIds" DROP CONSTRAINT "PK_JobIds_1538104520";

ALTER TABLE public."JobSeekerAdminComments" DROP CONSTRAINT "PK_JobSeekerAdminComments_1874105717";

ALTER TABLE public."JobSeekerChangeLog" DROP CONSTRAINT "PK_JobSeekerChangeLog_1646628909";

ALTER TABLE public."JobSeekerEventLog" DROP CONSTRAINT "PK_JobSeekerEventLog_2002106173";

ALTER TABLE public."JobSeekerFlags" DROP CONSTRAINT "PK_JobSeekerFlags_98099390";

ALTER TABLE public."JobSeekerStatLabels" DROP CONSTRAINT "PK_JobSeekerStatLabels_1438628168";

ALTER TABLE public."JobSeekerStats" DROP CONSTRAINT "PK_JobSeekerStats_1406628054";

ALTER TABLE public."JobSeekerVersions" DROP CONSTRAINT "PK_JobSeekerVersions_286624064";

ALTER TABLE public."JobSources" DROP CONSTRAINT "PK_JobSources_734625660";

ALTER TABLE public."JobStats" DROP CONSTRAINT "PK_JobStats_1342627826";

ALTER TABLE public."JobVersions" DROP CONSTRAINT "PK_JobVersions_46623209";

ALTER TABLE public."JobViews" DROP CONSTRAINT "PK_JobViews_786101841";

ALTER TABLE public."Jobs" DROP CONSTRAINT "PK_Jobs_802101898";

ALTER TABLE public."Locations" DROP CONSTRAINT "PK_LocationLookups_466100701";

ALTER TABLE public."NocCategories" DROP CONSTRAINT "PK_NocCategories_2114106572";

ALTER TABLE public."NocCategories2021" DROP CONSTRAINT "PK_NocCategories2021_955150448";

ALTER TABLE public."NocCodes" DROP CONSTRAINT "PK_NocCodes_1634104862";

ALTER TABLE public."NocCodes2021" DROP CONSTRAINT "PK_NocCodes2021_859150106";

ALTER TABLE public."Provinces" DROP CONSTRAINT "PK_Provinces_142623551";

ALTER TABLE public."Regions" DROP CONSTRAINT "PK_Regions_174623665";

ALTER TABLE public."ReportPersistenceControl" DROP CONSTRAINT "PK_ReportPersistenceControl_1566628624";

ALTER TABLE public."SavedCareerProfiles" DROP CONSTRAINT "PK_SavedCareerProfiles_1250103494";

ALTER TABLE public."SavedIndustryProfiles" DROP CONSTRAINT "PK_SavedIndustryProfiles_1282103608";

ALTER TABLE public."SavedJobs" DROP CONSTRAINT "PK_SavedJobs_498100815";

ALTER TABLE public."SecurityQuestions" DROP CONSTRAINT "PK_SecurityQuestions_770101784";

ALTER TABLE public."SystemSettings" DROP CONSTRAINT "PK_SystemSettings_1678629023";

ALTER TABLE public."WeeklyPeriods" DROP CONSTRAINT "PK_WeeklyPeriods_542624976";

ALTER TABLE public."__EFMigrationsHistory" DROP CONSTRAINT "PK___EFMigrationsHistory_1221579390";

-- ------------ Write DROP-INDEX-stage scripts -----------

DROP INDEX IF EXISTS public."IX_AdminUsers_IX_AdminUsers_LockedByAdminUserId";

DROP INDEX IF EXISTS public."IX_AdminUsers_IX_AdminUsers_ModifiedByAdminUserId";

DROP INDEX IF EXISTS public."IX_AspNetRoleClaims_IX_AspNetRoleClaims_RoleId";

DROP INDEX IF EXISTS public."IX_AspNetRoles_RoleNameIndex";

DROP INDEX IF EXISTS public."IX_AspNetUserClaims_IX_AspNetUserClaims_UserId";

DROP INDEX IF EXISTS public."IX_AspNetUserLogins_IX_AspNetUserLogins_UserId";

DROP INDEX IF EXISTS public."IX_AspNetUserRoles_IX_AspNetUserRoles_RoleId";

DROP INDEX IF EXISTS public."IX_AspNetUsers_EmailIndex";

DROP INDEX IF EXISTS public."IX_AspNetUsers_IX_AspNetUsers_AccountStatus_LastName_FirstName";

DROP INDEX IF EXISTS public."IX_AspNetUsers_IX_AspNetUsers_CountryId";

DROP INDEX IF EXISTS public."IX_AspNetUsers_IX_AspNetUsers_DateRegistered";

DROP INDEX IF EXISTS public."IX_AspNetUsers_IX_AspNetUsers_Email";

DROP INDEX IF EXISTS public."IX_AspNetUsers_IX_AspNetUsers_FirstName_LastName";

DROP INDEX IF EXISTS public."IX_AspNetUsers_IX_AspNetUsers_LastModified";

DROP INDEX IF EXISTS public."IX_AspNetUsers_IX_AspNetUsers_LastName_FirstName";

DROP INDEX IF EXISTS public."IX_AspNetUsers_IX_AspNetUsers_LocationId";

DROP INDEX IF EXISTS public."IX_AspNetUsers_IX_AspNetUsers_LockedByAdminUserId";

DROP INDEX IF EXISTS public."IX_AspNetUsers_IX_AspNetUsers_ProvinceId";

DROP INDEX IF EXISTS public."IX_AspNetUsers_IX_AspNetUsers_SecurityQuestionId";

DROP INDEX IF EXISTS public."IX_AspNetUsers_UserNameIndex";

DROP INDEX IF EXISTS public."IX_DeletedJobs_IX_DeletedJobs_DeletedByAdminUserId";

DROP INDEX IF EXISTS public."IX_GeocodedLocationCache_IX_GeocodedLocationCache_Name";

DROP INDEX IF EXISTS public."IX_ImpersonationLog_IX_ImpersonationLog_AdminUserId";

DROP INDEX IF EXISTS public."IX_ImpersonationLog_IX_ImpersonationLog_AspNetUserId";

DROP INDEX IF EXISTS public."IX_ImportedJobsWanted_IX_ImportedJobsWanted_HashId";

DROP INDEX IF EXISTS public."IX_JobAlerts_IX_JobAlerts_AspNetUserId";

DROP INDEX IF EXISTS public."IX_JobAlerts_IX_JobAlerts_DateCreated";

DROP INDEX IF EXISTS public."IX_JobIds_IX_JobIds_JobSourceId";

DROP INDEX IF EXISTS public."IX_JobSeekerAdminComments_IX_JobSeekerAdminComments_AspNetUserId";

DROP INDEX IF EXISTS public."IX_JobSeekerAdminComments_IX_JobSeekerAdminComments_EnteredByAdminUserId";

DROP INDEX IF EXISTS public."IX_JobSeekerChangeLog_IX_JobSeekerChangeLog_AspNetUserId";

DROP INDEX IF EXISTS public."IX_JobSeekerChangeLog_IX_JobSeekerChangeLog_ModifiedByAdminUserId";

DROP INDEX IF EXISTS public."IX_JobSeekerEventLog_IX_JobSeekerEventLog_AspNetUserId";

DROP INDEX IF EXISTS public."IX_JobSeekerEventLog_IX_JobSeekerEventLog_DateLogged";

DROP INDEX IF EXISTS public."IX_JobSeekerFlags_IX_JobSeekerFlags_AspNetUserId";

DROP INDEX IF EXISTS public."IX_JobSeekerStats_IX_JobSeekerStats_LabelKey";

DROP INDEX IF EXISTS public."IX_JobSeekerStats_IX_JobSeekerStats_RegionId";

DROP INDEX IF EXISTS public."IX_JobSeekerVersions_IX_JobSeekerVersions_AspNetUserId_VersionNumber";

DROP INDEX IF EXISTS public."IX_JobSeekerVersions_IX_JobSeekerVersions_CountryId";

DROP INDEX IF EXISTS public."IX_JobSeekerVersions_IX_JobSeekerVersions_LocationId";

DROP INDEX IF EXISTS public."IX_JobSeekerVersions_IX_JobSeekerVersions_ProvinceId";

DROP INDEX IF EXISTS public."IX_JobStats_IX_JobStats_JobSourceId";

DROP INDEX IF EXISTS public."IX_JobStats_IX_JobStats_RegionId";

DROP INDEX IF EXISTS public."IX_JobVersions_IX_JobVersions_JobId_VersionNumber";

DROP INDEX IF EXISTS public."IX_JobVersions_IX_JobVersions_LocationId";

DROP INDEX IF EXISTS public."IX_JobVersions_IX_JobVersions_NaicsId";

DROP INDEX IF EXISTS public."IX_JobVersions_IX_JobVersions_NocCodeId";

DROP INDEX IF EXISTS public."IX_Jobs_IX_Jobs_JobSourceId";

DROP INDEX IF EXISTS public."IX_Jobs_IX_Jobs_LocationId";

DROP INDEX IF EXISTS public."IX_Jobs_IX_Jobs_NaicsId";

DROP INDEX IF EXISTS public."IX_Jobs_IX_Jobs_NocCodeId";

DROP INDEX IF EXISTS public."IX_Locations_IX_LocationLookups_RegionId";

DROP INDEX IF EXISTS public."IX_SavedCareerProfiles_IX_SavedCareerProfiles_AspNetUserId";

DROP INDEX IF EXISTS public."IX_SavedCareerProfiles_IX_SavedCareerProfiles_DateDeleted";

DROP INDEX IF EXISTS public."IX_SavedCareerProfiles_IX_SavedCareerProfiles_DateSaved";

DROP INDEX IF EXISTS public."IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_AspNetUserId";

DROP INDEX IF EXISTS public."IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_DateDeleted";

DROP INDEX IF EXISTS public."IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_DateSaved";

DROP INDEX IF EXISTS public."IX_SavedJobs_IX_SavedJobs_AspNetUserId";

DROP INDEX IF EXISTS public."IX_SavedJobs_IX_SavedJobs_JobId";

DROP INDEX IF EXISTS public."IX_SystemSettings_IX_SystemSettings_ModifiedByAdminUserId";

DROP INDEX IF EXISTS public."IX_WeeklyPeriods_IX_WeeklyPeriods_WeekEndDate";

-- ------------ Write DROP-TABLE-stage scripts -----------

DROP TABLE IF EXISTS public."AdminUsers";

DROP TABLE IF EXISTS public."AspNetRoleClaims";

DROP TABLE IF EXISTS public."AspNetRoles";

DROP TABLE IF EXISTS public."AspNetUserClaims";

DROP TABLE IF EXISTS public."AspNetUserLogins";

DROP TABLE IF EXISTS public."AspNetUserRoles";

DROP TABLE IF EXISTS public."AspNetUserTokens";

DROP TABLE IF EXISTS public."AspNetUsers";

DROP TABLE IF EXISTS public."Countries";

DROP TABLE IF EXISTS public."DataProtectionKeys";

DROP TABLE IF EXISTS public."DeletedJobs";

DROP TABLE IF EXISTS public."ExpiredJobs";

DROP TABLE IF EXISTS public."GeocodedLocationCache";

DROP TABLE IF EXISTS public."ImpersonationLog";

DROP TABLE IF EXISTS public."ImportedJobsFederal";

DROP TABLE IF EXISTS public."ImportedJobsWanted";

DROP TABLE IF EXISTS public."Industries";

DROP TABLE IF EXISTS public."JobAlerts";

DROP TABLE IF EXISTS public."JobIds";

DROP TABLE IF EXISTS public."JobBankUserImport";

DROP TABLE IF EXISTS public."JobSeekerAdminComments";

DROP TABLE IF EXISTS public."JobSeekerChangeLog";

DROP TABLE IF EXISTS public."JobSeekerEventLog";

DROP TABLE IF EXISTS public."JobSeekerFlags";

DROP TABLE IF EXISTS public."JobSeekerStatLabels";

DROP TABLE IF EXISTS public."JobSeekerStats";

DROP TABLE IF EXISTS public."JobSeekerVersions";

DROP TABLE IF EXISTS public."JobSources";

DROP TABLE IF EXISTS public."JobStats";

DROP TABLE IF EXISTS public."JobVersions";

DROP TABLE IF EXISTS public."JobViews";

DROP TABLE IF EXISTS public."Jobs";

DROP TABLE IF EXISTS public."Locations";

DROP TABLE IF EXISTS public."NocCategories";

DROP TABLE IF EXISTS public."NocCategories2021";

DROP TABLE IF EXISTS public."NocCodes";

DROP TABLE IF EXISTS public."NocCodes2021";

DROP TABLE IF EXISTS public."Provinces";

DROP TABLE IF EXISTS public."Regions";

DROP TABLE IF EXISTS public."ReportPersistenceControl";

DROP TABLE IF EXISTS public."SavedCareerProfiles";

DROP TABLE IF EXISTS public."SavedIndustryProfiles";

DROP TABLE IF EXISTS public."SavedJobs";

DROP TABLE IF EXISTS public."SecurityQuestions";

DROP TABLE IF EXISTS public."SystemSettings";

DROP TABLE IF EXISTS public."WeeklyPeriods";

DROP TABLE IF EXISTS public."__EFMigrationsHistory";

-- ------------ Write DROP-DATABASE-stage scripts -----------

-- ------------ Write CREATE-DATABASE-stage scripts -----------

CREATE SCHEMA IF NOT EXISTS public;

-- ------------ Write CREATE-TABLE-stage scripts -----------

CREATE TABLE public."AdminUsers"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "SamAccountName" VARCHAR(20),
    "DisplayName" VARCHAR(60) NOT NULL,
    "DateUpdated" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "AdminLevel" INTEGER NOT NULL,
    "DateCreated" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    "Deleted" NUMERIC(1,0) NOT NULL,
    "Guid" VARCHAR(40),
    "DateLocked" TIMESTAMP(6) WITHOUT TIME ZONE,
    "LockedByAdminUserId" INTEGER,
    "ModifiedByAdminUserId" INTEGER,
    "DateLastLogin" TIMESTAMP(6) WITHOUT TIME ZONE,
    "GivenName" VARCHAR(40),
    "Surname" VARCHAR(40)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."AspNetRoleClaims"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "RoleId" VARCHAR(450) NOT NULL,
    "ClaimType" TEXT,
    "ClaimValue" TEXT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."AspNetRoles"(
    "Id" VARCHAR(450) NOT NULL,
    "Name" VARCHAR(256),
    "NormalizedName" VARCHAR(256),
    "ConcurrencyStamp" TEXT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."AspNetUserClaims"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "UserId" VARCHAR(450) NOT NULL,
    "ClaimType" TEXT,
    "ClaimValue" TEXT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."AspNetUserLogins"(
    "LoginProvider" VARCHAR(128) NOT NULL,
    "ProviderKey" VARCHAR(128) NOT NULL,
    "ProviderDisplayName" TEXT,
    "UserId" VARCHAR(450) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."AspNetUserRoles"(
    "UserId" VARCHAR(450) NOT NULL,
    "RoleId" VARCHAR(450) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."AspNetUserTokens"(
    "UserId" VARCHAR(450) NOT NULL,
    "LoginProvider" VARCHAR(128) NOT NULL,
    "Name" VARCHAR(128) NOT NULL,
    "Value" TEXT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."AspNetUsers"(
    "Id" VARCHAR(450) NOT NULL,
    "UserName" VARCHAR(256),
    "NormalizedUserName" VARCHAR(256),
    "Email" VARCHAR(256),
    "NormalizedEmail" VARCHAR(256),
    "EmailConfirmed" NUMERIC(1,0) NOT NULL,
    "PasswordHash" TEXT,
    "SecurityStamp" TEXT,
    "ConcurrencyStamp" TEXT,
    "PhoneNumber" TEXT,
    "PhoneNumberConfirmed" NUMERIC(1,0) NOT NULL,
    "TwoFactorEnabled" NUMERIC(1,0) NOT NULL,
    "LockoutEnd" TIMESTAMP(6) WITH TIME ZONE,
    "LockoutEnabled" NUMERIC(1,0) NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL,
    "LocationId" INTEGER,
    "City" VARCHAR(50),
    "CountryId" INTEGER,
    "FirstName" VARCHAR(50),
    "LastName" VARCHAR(50),
    "LegacyWebUserId" INTEGER,
    "ProvinceId" INTEGER,
    "AccountStatus" SMALLINT NOT NULL,
    "DateRegistered" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    "LastLogon" TIMESTAMP(6) WITHOUT TIME ZONE,
    "LastModified" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    "VerificationGuid" UUID,
    "SecurityAnswer" VARCHAR(50),
    "SecurityQuestionId" INTEGER,
    "DateLocked" TIMESTAMP(6) WITHOUT TIME ZONE,
    "LockedByAdminUserId" INTEGER
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."Countries"(
    "Id" INTEGER NOT NULL,
    "Name" VARCHAR(50),
    "CountryTwoLetterCode" VARCHAR(2),
    "SortOrder" SMALLINT NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."DataProtectionKeys"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "FriendlyName" TEXT,
    "Xml" TEXT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."DeletedJobs"(
    "JobId" BIGINT NOT NULL,
    "DeletedByAdminUserId" INTEGER NOT NULL,
    "DateDeleted" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."ExpiredJobs"(
    "JobId" BIGINT NOT NULL,
    "RemovedFromElasticsearch" NUMERIC(1,0) NOT NULL,
    "DateRemoved" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."GeocodedLocationCache"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "Name" VARCHAR(120),
    "Latitude" VARCHAR(25),
    "Longitude" VARCHAR(25),
    "DateGeocoded" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "IsPermanent" NUMERIC(1,0) NOT NULL,
    "City" VARCHAR(80),
    "Province" VARCHAR(2),
    "FrenchCity" VARCHAR(80)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."ImpersonationLog"(
    "Token" VARCHAR(200) NOT NULL,
    "AspNetUserId" VARCHAR(450),
    "AdminUserId" INTEGER NOT NULL,
    "DateTokenCreated" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."ImportedJobsFederal"(
    "JobId" BIGINT NOT NULL,
    "ApiDate" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "DateFirstImported" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "JobPostEnglish" TEXT,
    "JobPostFrench" TEXT,
    "ReIndexNeeded" NUMERIC(1,0) NOT NULL,
    "DisplayUntil" TIMESTAMP(6) WITHOUT TIME ZONE,
    "DateLastImported" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000'
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."ImportedJobsWanted"(
    "JobId" BIGINT NOT NULL,
    "JobPostEnglish" TEXT,
    "DateFirstImported" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "ApiDate" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "ReIndexNeeded" NUMERIC(1,0) NOT NULL,
    "DateLastImported" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    "IsFederalOrWorkBc" NUMERIC(1,0) NOT NULL,
    "HashId" BIGINT NOT NULL,
    "DateLastSeen" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000'
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."Industries"(
    "Id" SMALLINT NOT NULL,
    "Title" VARCHAR(150),
    "TitleBC" VARCHAR(150)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobAlerts"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "Title" VARCHAR(50),
    "AlertFrequency" SMALLINT NOT NULL,
    "UrlParameters" VARCHAR(1000),
    "DateCreated" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "DateModified" TIMESTAMP(6) WITHOUT TIME ZONE,
    "DateDeleted" TIMESTAMP(6) WITHOUT TIME ZONE,
    "IsDeleted" NUMERIC(1,0) NOT NULL,
    "AspNetUserId" VARCHAR(450),
    "JobSearchFilters" TEXT,
    "JobSearchFiltersVersion" INTEGER NOT NULL DEFAULT (0)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobIds"(
    "Id" BIGINT NOT NULL,
    "DateFirstImported" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "JobSourceId" SMALLINT NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobBankUserImport"(
    "LegacyWebUserId" INTEGER NOT NULL,
    "Email" VARCHAR(150) NOT NULL,
    "Password" VARCHAR(150) NOT NULL,
    "AccountStatus" INTEGER NOT NULL,
    "VerificationGuid" UUID,
    "LastModified" TIMESTAMP WITHOUT TIME ZONE,
    "DateCreated" TIMESTAMP WITHOUT TIME ZONE,
    "FirstName" VARCHAR(50),
    "LastName" VARCHAR(50),
    "LastLogon" TIMESTAMP(6) WITHOUT TIME ZONE,
    "LocationId" INTEGER,
    "City" VARCHAR(100),
    "CountryId" INTEGER,
    "ProvinceId" INTEGER,
    "IsIndigenousPerson" NUMERIC(1,0),
    "IsPersonWithDisability" NUMERIC(1,0),
    "IsMatureWorker" NUMERIC(1,0),
    "IsReservist" NUMERIC(1,0),
    "IsApprentice" NUMERIC(1,0)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobSeekerAdminComments"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "AspNetUserId" VARCHAR(450),
    "Comment" TEXT,
    "IsPinned" NUMERIC(1,0) NOT NULL,
    "EnteredByAdminUserId" INTEGER NOT NULL,
    "DateEntered" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobSeekerChangeLog"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "AspNetUserId" VARCHAR(450),
    "Field" VARCHAR(100),
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ModifiedByAdminUserId" INTEGER,
    "DateUpdated" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobSeekerEventLog"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "AspNetUserId" VARCHAR(450),
    "EventTypeId" INTEGER NOT NULL,
    "DateLogged" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobSeekerFlags"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "AspNetUserId" VARCHAR(450),
    "IsApprentice" NUMERIC(1,0) NOT NULL,
    "IsIndigenousPerson" NUMERIC(1,0) NOT NULL,
    "IsMatureWorker" NUMERIC(1,0) NOT NULL,
    "IsNewImmigrant" NUMERIC(1,0) NOT NULL,
    "IsPersonWithDisability" NUMERIC(1,0) NOT NULL,
    "IsStudent" NUMERIC(1,0) NOT NULL,
    "IsVeteran" NUMERIC(1,0) NOT NULL,
    "IsVisibleMinority" NUMERIC(1,0) NOT NULL,
    "IsYouth" NUMERIC(1,0) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobSeekerStatLabels"(
    "Key" VARCHAR(4) NOT NULL,
    "Label" VARCHAR(100),
    "IsTotal" NUMERIC(1,0) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobSeekerStats"(
    "WeeklyPeriodId" INTEGER NOT NULL,
    "Value" INTEGER NOT NULL,
    "RegionId" INTEGER NOT NULL,
    "LabelKey" VARCHAR(4) NOT NULL DEFAULT ''
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobSeekerVersions"(
    "Id" BIGINT NOT NULL GENERATED ALWAYS AS IDENTITY,
    "AspNetUserId" VARCHAR(450),
    "CountryId" INTEGER,
    "ProvinceId" INTEGER,
    "LocationId" INTEGER,
    "DateRegistered" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "AccountStatus" SMALLINT NOT NULL,
    "EmailConfirmed" NUMERIC(1,0) NOT NULL,
    "IsApprentice" NUMERIC(1,0) NOT NULL,
    "IsIndigenousPerson" NUMERIC(1,0) NOT NULL,
    "IsMatureWorker" NUMERIC(1,0) NOT NULL,
    "IsNewImmigrant" NUMERIC(1,0) NOT NULL,
    "IsPersonWithDisability" NUMERIC(1,0) NOT NULL,
    "IsStudent" NUMERIC(1,0) NOT NULL,
    "IsVeteran" NUMERIC(1,0) NOT NULL,
    "IsVisibleMinority" NUMERIC(1,0) NOT NULL,
    "IsYouth" NUMERIC(1,0) NOT NULL,
    "DateVersionStart" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "DateVersionEnd" TIMESTAMP(6) WITHOUT TIME ZONE,
    "IsCurrentVersion" NUMERIC(1,0) NOT NULL,
    "VersionNumber" SMALLINT NOT NULL,
    "Email" VARCHAR(256)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobSources"(
    "Id" SMALLINT NOT NULL,
    "Name" VARCHAR(50),
    "GroupName" VARCHAR(50),
    "ListOrder" SMALLINT NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobStats"(
    "WeeklyPeriodId" INTEGER NOT NULL,
    "JobSourceId" SMALLINT NOT NULL,
    "RegionId" INTEGER NOT NULL,
    "JobPostings" INTEGER NOT NULL,
    "PositionsAvailable" INTEGER NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobVersions"(
    "Id" BIGINT NOT NULL GENERATED ALWAYS AS IDENTITY,
    "JobId" BIGINT NOT NULL,
    "LocationId" INTEGER NOT NULL,
    "NocCodeId" SMALLINT,
    "IndustryId" SMALLINT,
    "PositionsAvailable" SMALLINT NOT NULL,
    "DatePosted" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "IsActive" NUMERIC(1,0) NOT NULL,
    "DateVersionStart" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "DateVersionEnd" TIMESTAMP(6) WITHOUT TIME ZONE,
    "IsCurrentVersion" NUMERIC(1,0) NOT NULL,
    "VersionNumber" SMALLINT NOT NULL,
    "ActualDatePosted" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    "DateFirstImported" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    "JobSourceId" SMALLINT NOT NULL,
    "NocCodeId2021" INTEGER
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."JobViews"(
    "JobId" BIGINT NOT NULL,
    "Views" INTEGER,
    "DateLastViewed" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."Jobs"(
    "JobId" BIGINT NOT NULL,
    "Title" VARCHAR(300),
    "NocCodeId" SMALLINT,
    "PositionsAvailable" SMALLINT NOT NULL,
    "EmployerName" VARCHAR(100),
    "DatePosted" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "Casual" NUMERIC(1,0) NOT NULL,
    "City" VARCHAR(120),
    "ExpireDate" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    "FullTime" NUMERIC(1,0) NOT NULL,
    "LastUpdated" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    "LeadingToFullTime" NUMERIC(1,0) NOT NULL,
    "LocationId" INTEGER NOT NULL DEFAULT (0),
    "IndustryId" SMALLINT,
    "PartTime" NUMERIC(1,0) NOT NULL,
    "Permanent" NUMERIC(1,0) NOT NULL,
    "Salary" NUMERIC(18,2),
    "SalarySummary" VARCHAR(60),
    "Seasonal" NUMERIC(1,0) NOT NULL,
    "Temporary" NUMERIC(1,0) NOT NULL,
    "DateFirstImported" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    "IsActive" NUMERIC(1,0) NOT NULL,
    "DateLastImported" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    "JobSourceId" SMALLINT NOT NULL,
    "OriginalSource" VARCHAR(100),
    "ExternalSourceUrl" VARCHAR(800),
    "ActualDatePosted" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    "NocCodeId2021" INTEGER
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."Locations"(
    "LocationId" INTEGER NOT NULL,
    "EDM_Location_DistrictLocationId" INTEGER NOT NULL,
    "RegionId" INTEGER,
    "FederalCityId" INTEGER,
    "City" VARCHAR(50),
    "Label" VARCHAR(50),
    "IsDuplicate" NUMERIC(1,0) NOT NULL,
    "IsHidden" NUMERIC(1,0) NOT NULL,
    "Latitude" VARCHAR(25),
    "Longitude" VARCHAR(25),
    "BcStatsPlaceId" INTEGER
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."NocCategories"(
    "CategoryCode" VARCHAR(3) NOT NULL,
    "Level" SMALLINT NOT NULL,
    "Title" VARCHAR(150)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."NocCategories2021"(
    "CategoryCode" VARCHAR(4) NOT NULL,
    "Level" SMALLINT NOT NULL,
    "Title" VARCHAR(150)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."NocCodes"(
    "Code" VARCHAR(4),
    "Title" VARCHAR(150),
    "Id" SMALLINT NOT NULL,
    "FrenchTitle" VARCHAR(180)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."NocCodes2021"(
    "Id" INTEGER NOT NULL,
    "Code" VARCHAR(5),
    "Title" VARCHAR(150),
    "FrenchTitle" VARCHAR(250),
    "Code2016" VARCHAR(30)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."Provinces"(
    "ProvinceId" INTEGER NOT NULL,
    "Name" VARCHAR(50),
    "ShortName" VARCHAR(2)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."Regions"(
    "Id" INTEGER NOT NULL,
    "Name" VARCHAR(50),
    "ListOrder" SMALLINT NOT NULL,
    "IsHidden" NUMERIC(1,0) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."ReportPersistenceControl"(
    "WeeklyPeriodId" INTEGER NOT NULL,
    "DateCalculated" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "IsTotalToDate" NUMERIC(1,0) NOT NULL,
    "TableName" VARCHAR(25) NOT NULL DEFAULT ''
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."SavedCareerProfiles"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "EDM_CareerProfile_CareerProfileId" INTEGER,
    "AspNetUserId" VARCHAR(450),
    "DateSaved" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "DateDeleted" TIMESTAMP(6) WITHOUT TIME ZONE,
    "IsDeleted" NUMERIC(1,0) NOT NULL,
    "NocCodeId2021" INTEGER
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."SavedIndustryProfiles"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "AspNetUserId" VARCHAR(450),
    "DateSaved" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "DateDeleted" TIMESTAMP(6) WITHOUT TIME ZONE,
    "IsDeleted" NUMERIC(1,0) NOT NULL,
    "IndustryId" SMALLINT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."SavedJobs"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "JobId" BIGINT NOT NULL,
    "AspNetUserId" VARCHAR(450),
    "DateSaved" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "DateDeleted" TIMESTAMP(6) WITHOUT TIME ZONE,
    "IsDeleted" NUMERIC(1,0) NOT NULL,
    "Note" VARCHAR(800),
    "NoteUpdatedDate" TIMESTAMP(6) WITHOUT TIME ZONE
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."SecurityQuestions"(
    "Id" INTEGER NOT NULL,
    "QuestionText" VARCHAR(40)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."SystemSettings"(
    "Name" VARCHAR(400) NOT NULL,
    "Value" TEXT,
    "Description" TEXT,
    "FieldType" INTEGER NOT NULL,
    "ModifiedByAdminUserId" INTEGER NOT NULL,
    "DateUpdated" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "DefaultValue" TEXT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."WeeklyPeriods"(
    "Id" INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    "CalendarYear" SMALLINT NOT NULL,
    "CalendarMonth" SMALLINT NOT NULL,
    "FiscalYear" SMALLINT NOT NULL,
    "WeekOfMonth" SMALLINT NOT NULL,
    "WeekStartDate" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "WeekEndDate" TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    "IsEndOfFiscalYear" NUMERIC(1,0) NOT NULL,
    "IsEndOfMonth" NUMERIC(1,0) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public."__EFMigrationsHistory"(
    "MigrationId" VARCHAR(150) NOT NULL,
    "ProductVersion" VARCHAR(32) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

-- ------------ Write CREATE-INDEX-stage scripts -----------

CREATE INDEX "IX_AdminUsers_IX_AdminUsers_LockedByAdminUserId"
ON public."AdminUsers"
USING BTREE ("LockedByAdminUserId" ASC);

CREATE INDEX "IX_AdminUsers_IX_AdminUsers_ModifiedByAdminUserId"
ON public."AdminUsers"
USING BTREE ("ModifiedByAdminUserId" ASC);

CREATE INDEX "IX_AspNetRoleClaims_IX_AspNetRoleClaims_RoleId"
ON public."AspNetRoleClaims"
USING BTREE ("RoleId" ASC);

CREATE UNIQUE INDEX "IX_AspNetRoles_RoleNameIndex"
ON public."AspNetRoles"
USING BTREE ("NormalizedName" ASC)
WHERE
("NormalizedName" IS NOT NULL);

CREATE INDEX "IX_AspNetUserClaims_IX_AspNetUserClaims_UserId"
ON public."AspNetUserClaims"
USING BTREE ("UserId" ASC);

CREATE INDEX "IX_AspNetUserLogins_IX_AspNetUserLogins_UserId"
ON public."AspNetUserLogins"
USING BTREE ("UserId" ASC);

CREATE INDEX "IX_AspNetUserRoles_IX_AspNetUserRoles_RoleId"
ON public."AspNetUserRoles"
USING BTREE ("RoleId" ASC);

CREATE INDEX "IX_AspNetUsers_EmailIndex"
ON public."AspNetUsers"
USING BTREE ("NormalizedEmail" ASC);

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_AccountStatus_LastName_FirstName"
ON public."AspNetUsers"
USING BTREE ("AccountStatus" ASC, "LastName" ASC, "FirstName" ASC) INCLUDE("Email");

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_CountryId"
ON public."AspNetUsers"
USING BTREE ("CountryId" ASC);

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_DateRegistered"
ON public."AspNetUsers"
USING BTREE ("DateRegistered" ASC) INCLUDE("LastName", "FirstName", "Email", "AccountStatus");

CREATE UNIQUE INDEX "IX_AspNetUsers_IX_AspNetUsers_Email"
ON public."AspNetUsers"
USING BTREE ("Email" ASC) INCLUDE("LastName", "FirstName", "AccountStatus")
WHERE
("Email" IS NOT NULL);

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_FirstName_LastName"
ON public."AspNetUsers"
USING BTREE ("FirstName" ASC, "LastName" ASC) INCLUDE("Email", "AccountStatus");

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_LastModified"
ON public."AspNetUsers"
USING BTREE ("LastModified" ASC) INCLUDE("LastName", "FirstName", "Email", "AccountStatus");

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_LastName_FirstName"
ON public."AspNetUsers"
USING BTREE ("LastName" ASC, "FirstName" ASC) INCLUDE("Email", "AccountStatus");

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_LocationId"
ON public."AspNetUsers"
USING BTREE ("LocationId" ASC);

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_LockedByAdminUserId"
ON public."AspNetUsers"
USING BTREE ("LockedByAdminUserId" ASC);

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_ProvinceId"
ON public."AspNetUsers"
USING BTREE ("ProvinceId" ASC);

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_SecurityQuestionId"
ON public."AspNetUsers"
USING BTREE ("SecurityQuestionId" ASC);

CREATE UNIQUE INDEX "IX_AspNetUsers_UserNameIndex"
ON public."AspNetUsers"
USING BTREE ("NormalizedUserName" ASC)
WHERE
("NormalizedUserName" IS NOT NULL);

CREATE INDEX "IX_DeletedJobs_IX_DeletedJobs_DeletedByAdminUserId"
ON public."DeletedJobs"
USING BTREE ("DeletedByAdminUserId" ASC);

CREATE UNIQUE INDEX "IX_GeocodedLocationCache_IX_GeocodedLocationCache_Name"
ON public."GeocodedLocationCache"
USING BTREE ("Name" ASC)
WHERE
("Name" IS NOT NULL);

CREATE INDEX "IX_ImpersonationLog_IX_ImpersonationLog_AdminUserId"
ON public."ImpersonationLog"
USING BTREE ("AdminUserId" ASC);

CREATE INDEX "IX_ImpersonationLog_IX_ImpersonationLog_AspNetUserId"
ON public."ImpersonationLog"
USING BTREE ("AspNetUserId" ASC);

CREATE UNIQUE INDEX "IX_ImportedJobsWanted_IX_ImportedJobsWanted_HashId"
ON public."ImportedJobsWanted"
USING BTREE ("HashId" ASC);

CREATE INDEX "IX_JobAlerts_IX_JobAlerts_AspNetUserId"
ON public."JobAlerts"
USING BTREE ("AspNetUserId" ASC);

CREATE INDEX "IX_JobAlerts_IX_JobAlerts_DateCreated"
ON public."JobAlerts"
USING BTREE ("DateCreated" ASC);

CREATE INDEX "IX_JobIds_IX_JobIds_JobSourceId"
ON public."JobIds"
USING BTREE ("JobSourceId" ASC);

CREATE INDEX "IX_JobSeekerAdminComments_IX_JobSeekerAdminComments_AspNetUserId"
ON public."JobSeekerAdminComments"
USING BTREE ("AspNetUserId" ASC);

CREATE INDEX "IX_JobSeekerAdminComments_IX_JobSeekerAdminComments_EnteredByAdminUserId"
ON public."JobSeekerAdminComments"
USING BTREE ("EnteredByAdminUserId" ASC);

CREATE INDEX "IX_JobSeekerChangeLog_IX_JobSeekerChangeLog_AspNetUserId"
ON public."JobSeekerChangeLog"
USING BTREE ("AspNetUserId" ASC);

CREATE INDEX "IX_JobSeekerChangeLog_IX_JobSeekerChangeLog_ModifiedByAdminUserId"
ON public."JobSeekerChangeLog"
USING BTREE ("ModifiedByAdminUserId" ASC);

CREATE INDEX "IX_JobSeekerEventLog_IX_JobSeekerEventLog_AspNetUserId"
ON public."JobSeekerEventLog"
USING BTREE ("AspNetUserId" ASC);

CREATE INDEX "IX_JobSeekerEventLog_IX_JobSeekerEventLog_DateLogged"
ON public."JobSeekerEventLog"
USING BTREE ("DateLogged" ASC);

CREATE UNIQUE INDEX "IX_JobSeekerFlags_IX_JobSeekerFlags_AspNetUserId"
ON public."JobSeekerFlags"
USING BTREE ("AspNetUserId" ASC)
WHERE
("AspNetUserId" IS NOT NULL);

CREATE INDEX "IX_JobSeekerStats_IX_JobSeekerStats_LabelKey"
ON public."JobSeekerStats"
USING BTREE ("LabelKey" ASC);

CREATE INDEX "IX_JobSeekerStats_IX_JobSeekerStats_RegionId"
ON public."JobSeekerStats"
USING BTREE ("RegionId" ASC);

CREATE UNIQUE INDEX "IX_JobSeekerVersions_IX_JobSeekerVersions_AspNetUserId_VersionNumber"
ON public."JobSeekerVersions"
USING BTREE ("AspNetUserId" ASC, "VersionNumber" ASC)
WHERE
("AspNetUserId" IS NOT NULL);

CREATE INDEX "IX_JobSeekerVersions_IX_JobSeekerVersions_CountryId"
ON public."JobSeekerVersions"
USING BTREE ("CountryId" ASC);

CREATE INDEX "IX_JobSeekerVersions_IX_JobSeekerVersions_LocationId"
ON public."JobSeekerVersions"
USING BTREE ("LocationId" ASC);

CREATE INDEX "IX_JobSeekerVersions_IX_JobSeekerVersions_ProvinceId"
ON public."JobSeekerVersions"
USING BTREE ("ProvinceId" ASC);

CREATE INDEX "IX_JobStats_IX_JobStats_JobSourceId"
ON public."JobStats"
USING BTREE ("JobSourceId" ASC);

CREATE INDEX "IX_JobStats_IX_JobStats_RegionId"
ON public."JobStats"
USING BTREE ("RegionId" ASC);

CREATE UNIQUE INDEX "IX_JobVersions_IX_JobVersions_JobId_VersionNumber"
ON public."JobVersions"
USING BTREE ("JobId" ASC, "VersionNumber" ASC);

CREATE INDEX "IX_JobVersions_IX_JobVersions_LocationId"
ON public."JobVersions"
USING BTREE ("LocationId" ASC);

CREATE INDEX "IX_JobVersions_IX_JobVersions_NaicsId"
ON public."JobVersions"
USING BTREE ("IndustryId" ASC);

CREATE INDEX "IX_JobVersions_IX_JobVersions_NocCodeId"
ON public."JobVersions"
USING BTREE ("NocCodeId" ASC);

CREATE INDEX "IX_Jobs_IX_Jobs_JobSourceId"
ON public."Jobs"
USING BTREE ("JobSourceId" ASC);

CREATE INDEX "IX_Jobs_IX_Jobs_LocationId"
ON public."Jobs"
USING BTREE ("LocationId" ASC);

CREATE INDEX "IX_Jobs_IX_Jobs_NaicsId"
ON public."Jobs"
USING BTREE ("IndustryId" ASC);

CREATE INDEX "IX_Jobs_IX_Jobs_NocCodeId"
ON public."Jobs"
USING BTREE ("NocCodeId" ASC);

CREATE INDEX "IX_Locations_IX_LocationLookups_RegionId"
ON public."Locations"
USING BTREE ("RegionId" ASC);

CREATE INDEX "IX_SavedCareerProfiles_IX_SavedCareerProfiles_AspNetUserId"
ON public."SavedCareerProfiles"
USING BTREE ("AspNetUserId" ASC);

CREATE INDEX "IX_SavedCareerProfiles_IX_SavedCareerProfiles_DateDeleted"
ON public."SavedCareerProfiles"
USING BTREE ("DateDeleted" ASC);

CREATE INDEX "IX_SavedCareerProfiles_IX_SavedCareerProfiles_DateSaved"
ON public."SavedCareerProfiles"
USING BTREE ("DateSaved" ASC);

CREATE INDEX "IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_AspNetUserId"
ON public."SavedIndustryProfiles"
USING BTREE ("AspNetUserId" ASC);

CREATE INDEX "IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_DateDeleted"
ON public."SavedIndustryProfiles"
USING BTREE ("DateDeleted" ASC);

CREATE INDEX "IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_DateSaved"
ON public."SavedIndustryProfiles"
USING BTREE ("DateSaved" ASC);

CREATE INDEX "IX_SavedJobs_IX_SavedJobs_AspNetUserId"
ON public."SavedJobs"
USING BTREE ("AspNetUserId" ASC);

CREATE INDEX "IX_SavedJobs_IX_SavedJobs_JobId"
ON public."SavedJobs"
USING BTREE ("JobId" ASC);

CREATE INDEX "IX_SystemSettings_IX_SystemSettings_ModifiedByAdminUserId"
ON public."SystemSettings"
USING BTREE ("ModifiedByAdminUserId" ASC);

CREATE INDEX "IX_WeeklyPeriods_IX_WeeklyPeriods_WeekEndDate"
ON public."WeeklyPeriods"
USING BTREE ("WeekEndDate" ASC);

-- ------------ Write CREATE-CONSTRAINT-stage scripts -----------

ALTER TABLE public."AdminUsers"
ADD CONSTRAINT "PK_AdminUsers_322100188" PRIMARY KEY ("Id");

ALTER TABLE public."AspNetRoleClaims"
ADD CONSTRAINT "PK_AspNetRoleClaims_1813581499" PRIMARY KEY ("Id");

ALTER TABLE public."AspNetRoles"
ADD CONSTRAINT "PK_AspNetRoles_1749581271" PRIMARY KEY ("Id");

ALTER TABLE public."AspNetUserClaims"
ADD CONSTRAINT "PK_AspNetUserClaims_1861581670" PRIMARY KEY ("Id");

ALTER TABLE public."AspNetUserLogins"
ADD CONSTRAINT "PK_AspNetUserLogins_418100530" PRIMARY KEY ("ProviderKey");

ALTER TABLE public."AspNetUserRoles"
ADD CONSTRAINT "PK_AspNetUserRoles_1957582012" PRIMARY KEY ("UserId", "RoleId");

ALTER TABLE public."AspNetUserTokens"
ADD CONSTRAINT "PK_AspNetUserTokens_402100473" PRIMARY KEY ("Name");

ALTER TABLE public."AspNetUsers"
ADD CONSTRAINT "PK_AspNetUsers_1781581385" PRIMARY KEY ("Id");

ALTER TABLE public."Countries"
ADD CONSTRAINT "PK_Countries_398624463" PRIMARY KEY ("Id");

ALTER TABLE public."DataProtectionKeys"
ADD CONSTRAINT "PK_DataProtectionKeys_2078630448" PRIMARY KEY ("Id");

ALTER TABLE public."DeletedJobs"
ADD CONSTRAINT "PK_DeletedJobs_958626458" PRIMARY KEY ("JobId");

ALTER TABLE public."ExpiredJobs"
ADD CONSTRAINT "PK_ExpiredJobs_1710629137" PRIMARY KEY ("JobId");

ALTER TABLE public."GeocodedLocationCache"
ADD CONSTRAINT "PK_GeocodedLocationCache_1669580986" PRIMARY KEY ("Id");

ALTER TABLE public."ImpersonationLog"
ADD CONSTRAINT "PK_ImpersonationLog_1022626686" PRIMARY KEY ("Token");

ALTER TABLE public."ImportedJobsFederal"
ADD CONSTRAINT "PK_ImportedJobsFederal_290100074" PRIMARY KEY ("JobId");

ALTER TABLE public."ImportedJobsWanted"
ADD CONSTRAINT "PK_ImportedJobsWanted_1509580416" PRIMARY KEY ("JobId");

ALTER TABLE public."Industries"
ADD CONSTRAINT "PK_NaicsCodes_2146106686" PRIMARY KEY ("Id");

ALTER TABLE public."JobAlerts"
ADD CONSTRAINT "PK_JobAlerts_1106102981" PRIMARY KEY ("Id");

ALTER TABLE public."JobIds"
ADD CONSTRAINT "PK_JobIds_1538104520" PRIMARY KEY ("Id");

ALTER TABLE public."JobSeekerAdminComments"
ADD CONSTRAINT "PK_JobSeekerAdminComments_1874105717" PRIMARY KEY ("Id");

ALTER TABLE public."JobSeekerChangeLog"
ADD CONSTRAINT "PK_JobSeekerChangeLog_1646628909" PRIMARY KEY ("Id");

ALTER TABLE public."JobSeekerEventLog"
ADD CONSTRAINT "PK_JobSeekerEventLog_2002106173" PRIMARY KEY ("Id");

ALTER TABLE public."JobSeekerFlags"
ADD CONSTRAINT "PK_JobSeekerFlags_98099390" PRIMARY KEY ("Id");

ALTER TABLE public."JobSeekerStatLabels"
ADD CONSTRAINT "PK_JobSeekerStatLabels_1438628168" PRIMARY KEY ("Key");

ALTER TABLE public."JobSeekerStats"
ADD CONSTRAINT "PK_JobSeekerStats_1406628054" PRIMARY KEY ("WeeklyPeriodId", "LabelKey", "RegionId");

ALTER TABLE public."JobSeekerVersions"
ADD CONSTRAINT "PK_JobSeekerVersions_286624064" PRIMARY KEY ("Id");

ALTER TABLE public."JobSources"
ADD CONSTRAINT "PK_JobSources_734625660" PRIMARY KEY ("Id");

ALTER TABLE public."JobStats"
ADD CONSTRAINT "PK_JobStats_1342627826" PRIMARY KEY ("WeeklyPeriodId", "RegionId", "JobSourceId");

ALTER TABLE public."JobVersions"
ADD CONSTRAINT "PK_JobVersions_46623209" PRIMARY KEY ("Id");

ALTER TABLE public."JobViews"
ADD CONSTRAINT "PK_JobViews_786101841" PRIMARY KEY ("JobId");

ALTER TABLE public."Jobs"
ADD CONSTRAINT "PK_Jobs_802101898" PRIMARY KEY ("JobId");

ALTER TABLE public."Locations"
ADD CONSTRAINT "PK_LocationLookups_466100701" PRIMARY KEY ("LocationId");

ALTER TABLE public."NocCategories"
ADD CONSTRAINT "PK_NocCategories_2114106572" PRIMARY KEY ("CategoryCode");

ALTER TABLE public."NocCategories2021"
ADD CONSTRAINT "PK_NocCategories2021_955150448" PRIMARY KEY ("CategoryCode");

ALTER TABLE public."NocCodes"
ADD CONSTRAINT "PK_NocCodes_1634104862" PRIMARY KEY ("Id");

ALTER TABLE public."NocCodes2021"
ADD CONSTRAINT "PK_NocCodes2021_859150106" PRIMARY KEY ("Id");

ALTER TABLE public."Provinces"
ADD CONSTRAINT "PK_Provinces_142623551" PRIMARY KEY ("ProvinceId");

ALTER TABLE public."Regions"
ADD CONSTRAINT "PK_Regions_174623665" PRIMARY KEY ("Id");

ALTER TABLE public."ReportPersistenceControl"
ADD CONSTRAINT "PK_ReportPersistenceControl_1566628624" PRIMARY KEY ("WeeklyPeriodId", "TableName");

ALTER TABLE public."SavedCareerProfiles"
ADD CONSTRAINT "PK_SavedCareerProfiles_1250103494" PRIMARY KEY ("Id");

ALTER TABLE public."SavedIndustryProfiles"
ADD CONSTRAINT "PK_SavedIndustryProfiles_1282103608" PRIMARY KEY ("Id");

ALTER TABLE public."SavedJobs"
ADD CONSTRAINT "PK_SavedJobs_498100815" PRIMARY KEY ("Id");

ALTER TABLE public."SecurityQuestions"
ADD CONSTRAINT "PK_SecurityQuestions_770101784" PRIMARY KEY ("Id");

ALTER TABLE public."SystemSettings"
ADD CONSTRAINT "PK_SystemSettings_1678629023" PRIMARY KEY ("Name");

ALTER TABLE public."WeeklyPeriods"
ADD CONSTRAINT "PK_WeeklyPeriods_542624976" PRIMARY KEY ("Id");

ALTER TABLE public."__EFMigrationsHistory"
ADD CONSTRAINT "PK___EFMigrationsHistory_1221579390" PRIMARY KEY ("MigrationId");

-- ------------ Write CREATE-FOREIGN-KEY-CONSTRAINT-stage scripts -----------

ALTER TABLE public."AdminUsers"
ADD CONSTRAINT "FK_AdminUsers_AdminUsers_LockedByAdminUserId_1346103836" FOREIGN KEY ("LockedByAdminUserId")
REFERENCES public."AdminUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."AdminUsers"
ADD CONSTRAINT "FK_AdminUsers_AdminUsers_ModifiedByAdminUserId_1378103950" FOREIGN KEY ("ModifiedByAdminUserId")
REFERENCES public."AdminUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."AspNetRoleClaims"
ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId_1829581556" FOREIGN KEY ("RoleId")
REFERENCES public."AspNetRoles" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."AspNetUserClaims"
ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId_1877581727" FOREIGN KEY ("UserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."AspNetUserLogins"
ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId_1925581898" FOREIGN KEY ("UserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."AspNetUserRoles"
ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId_1973582069" FOREIGN KEY ("RoleId")
REFERENCES public."AspNetRoles" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."AspNetUserRoles"
ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId_1989582126" FOREIGN KEY ("UserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."AspNetUserTokens"
ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId_2037582297" FOREIGN KEY ("UserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."AspNetUsers"
ADD CONSTRAINT "FK_AspNetUsers_AdminUsers_LockedByAdminUserId_2050106344" FOREIGN KEY ("LockedByAdminUserId")
REFERENCES public."AdminUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."AspNetUsers"
ADD CONSTRAINT "FK_AspNetUsers_Countries_CountryId_414624520" FOREIGN KEY ("CountryId")
REFERENCES public."Countries" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."AspNetUsers"
ADD CONSTRAINT "FK_AspNetUsers_LocationLookups_LocationId_190623722" FOREIGN KEY ("LocationId")
REFERENCES public."Locations" ("LocationId")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."AspNetUsers"
ADD CONSTRAINT "FK_AspNetUsers_Provinces_ProvinceId_206623779" FOREIGN KEY ("ProvinceId")
REFERENCES public."Provinces" ("ProvinceId")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."AspNetUsers"
ADD CONSTRAINT "FK_AspNetUsers_SecurityQuestions_SecurityQuestionId_850102069" FOREIGN KEY ("SecurityQuestionId")
REFERENCES public."SecurityQuestions" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."DeletedJobs"
ADD CONSTRAINT "FK_DeletedJobs_AdminUsers_DeletedByAdminUserId_974626515" FOREIGN KEY ("DeletedByAdminUserId")
REFERENCES public."AdminUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."DeletedJobs"
ADD CONSTRAINT "FK_DeletedJobs_Jobs_JobId_990626572" FOREIGN KEY ("JobId")
REFERENCES public."Jobs" ("JobId")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."ExpiredJobs"
ADD CONSTRAINT "FK_ExpiredJobs_JobIds_JobId_1726629194" FOREIGN KEY ("JobId")
REFERENCES public."JobIds" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."ImpersonationLog"
ADD CONSTRAINT "FK_ImpersonationLog_AdminUsers_AdminUserId_1038626743" FOREIGN KEY ("AdminUserId")
REFERENCES public."AdminUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."ImpersonationLog"
ADD CONSTRAINT "FK_ImpersonationLog_AspNetUsers_AspNetUserId_1054626800" FOREIGN KEY ("AspNetUserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."ImportedJobsFederal"
ADD CONSTRAINT "FK_ImportedJobsFederal_JobIds_JobId_1570104634" FOREIGN KEY ("JobId")
REFERENCES public."JobIds" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."ImportedJobsWanted"
ADD CONSTRAINT "FK_ImportedJobsWanted_JobIds_JobId_1586104691" FOREIGN KEY ("JobId")
REFERENCES public."JobIds" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."JobAlerts"
ADD CONSTRAINT "FK_JobAlerts_AspNetUsers_AspNetUserId_1122103038" FOREIGN KEY ("AspNetUserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobIds"
ADD CONSTRAINT "FK_JobIds_JobSources_JobSourceId_766625774" FOREIGN KEY ("JobSourceId")
REFERENCES public."JobSources" ("Id") MATCH SIMPLE
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobSeekerAdminComments"
ADD CONSTRAINT "FK_JobSeekerAdminComments_AdminUsers_EnteredByAdminUserId_1906105831" FOREIGN KEY ("EnteredByAdminUserId")
REFERENCES public."AdminUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."JobSeekerAdminComments"
ADD CONSTRAINT "FK_JobSeekerAdminComments_AspNetUsers_AspNetUserId_1890105774" FOREIGN KEY ("AspNetUserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobSeekerChangeLog"
ADD CONSTRAINT "FK_JobSeekerAdminLog_AspNetUsers_AspNetUserId_1662628966" FOREIGN KEY ("AspNetUserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobSeekerChangeLog"
ADD CONSTRAINT "FK_JobSeekerChangeLog_AdminUsers_ModifiedByAdminUserId_1630628852" FOREIGN KEY ("ModifiedByAdminUserId")
REFERENCES public."AdminUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobSeekerEventLog"
ADD CONSTRAINT "FK_JobSeekerEventLog_AspNetUsers_AspNetUserId_2018106230" FOREIGN KEY ("AspNetUserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobSeekerFlags"
ADD CONSTRAINT "FK_JobSeekerFlags_AspNetUsers_AspNetUserId_178099675" FOREIGN KEY ("AspNetUserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobSeekerStats"
ADD CONSTRAINT "FK_JobSeekerStats_JobSeekerStatLabels_LabelKey_1454628225" FOREIGN KEY ("LabelKey")
REFERENCES public."JobSeekerStatLabels" ("Key")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."JobSeekerStats"
ADD CONSTRAINT "FK_JobSeekerStats_Regions_RegionId_1502628396" FOREIGN KEY ("RegionId")
REFERENCES public."Regions" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."JobSeekerStats"
ADD CONSTRAINT "FK_JobSeekerStats_WeeklyPeriods_WeeklyPeriodId_1262627541" FOREIGN KEY ("WeeklyPeriodId")
REFERENCES public."WeeklyPeriods" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."JobSeekerVersions"
ADD CONSTRAINT "FK_JobSeekerVersions_AspNetUsers_AspNetUserId_302624121" FOREIGN KEY ("AspNetUserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobSeekerVersions"
ADD CONSTRAINT "FK_JobSeekerVersions_Countries_CountryId_430624577" FOREIGN KEY ("CountryId")
REFERENCES public."Countries" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobSeekerVersions"
ADD CONSTRAINT "FK_JobSeekerVersions_Locations_LocationId_366624349" FOREIGN KEY ("LocationId")
REFERENCES public."Locations" ("LocationId")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobSeekerVersions"
ADD CONSTRAINT "FK_JobSeekerVersions_Provinces_ProvinceId_350624292" FOREIGN KEY ("ProvinceId")
REFERENCES public."Provinces" ("ProvinceId")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobStats"
ADD CONSTRAINT "FK_JobStats_JobSources_JobSourceId_1358627883" FOREIGN KEY ("JobSourceId")
REFERENCES public."JobSources" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."JobStats"
ADD CONSTRAINT "FK_JobStats_Regions_RegionId_1518628453" FOREIGN KEY ("RegionId")
REFERENCES public."Regions" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."JobStats"
ADD CONSTRAINT "FK_JobStats_WeeklyPeriods_WeeklyPeriodId_1374627940" FOREIGN KEY ("WeeklyPeriodId")
REFERENCES public."WeeklyPeriods" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."JobVersions"
ADD CONSTRAINT "FK_JobVersions_Jobs_JobId_62623266" FOREIGN KEY ("JobId")
REFERENCES public."Jobs" ("JobId")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobVersions"
ADD CONSTRAINT "FK_JobVersions_LocationLookups_LocationId_238623893" FOREIGN KEY ("LocationId")
REFERENCES public."Locations" ("LocationId")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."JobVersions"
ADD CONSTRAINT "FK_JobVersions_NaicsCodes_NaicsId_94623380" FOREIGN KEY ("IndustryId")
REFERENCES public."Industries" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobVersions"
ADD CONSTRAINT "FK_JobVersions_NocCodes2021_NocCodeId2021_891150220" FOREIGN KEY ("NocCodeId2021")
REFERENCES public."NocCodes2021" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobVersions"
ADD CONSTRAINT "FK_JobVersions_NocCodes_NocCodeId_110623437" FOREIGN KEY ("NocCodeId")
REFERENCES public."NocCodes" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."JobViews"
ADD CONSTRAINT "FK_JobViews_Jobs_JobId_1442104178" FOREIGN KEY ("JobId")
REFERENCES public."Jobs" ("JobId")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."Jobs"
ADD CONSTRAINT "FK_Jobs_JobIds_JobId_1602104748" FOREIGN KEY ("JobId")
REFERENCES public."JobIds" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."Jobs"
ADD CONSTRAINT "FK_Jobs_JobSources_JobSourceId_782625831" FOREIGN KEY ("JobSourceId")
REFERENCES public."JobSources" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."Jobs"
ADD CONSTRAINT "FK_Jobs_LocationLookups_LocationId_222623836" FOREIGN KEY ("LocationId")
REFERENCES public."Locations" ("LocationId")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."Jobs"
ADD CONSTRAINT "FK_Jobs_NaicsCodes_NaicsId_14623095" FOREIGN KEY ("IndustryId")
REFERENCES public."Industries" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."Jobs"
ADD CONSTRAINT "FK_Jobs_NocCodes2021_NocCodeId2021_875150163" FOREIGN KEY ("NocCodeId2021")
REFERENCES public."NocCodes2021" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."Jobs"
ADD CONSTRAINT "FK_Jobs_NocCodes_NocCodeId_1650104919" FOREIGN KEY ("NocCodeId")
REFERENCES public."NocCodes" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."Locations"
ADD CONSTRAINT "FK_LocationLookups_Regions_RegionId_254623950" FOREIGN KEY ("RegionId")
REFERENCES public."Regions" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."Locations"
ADD CONSTRAINT "FK_Locations_Regions_RegionId_446624634" FOREIGN KEY ("RegionId")
REFERENCES public."Regions" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."ReportPersistenceControl"
ADD CONSTRAINT "FK_ReportPersistenceControl_WeeklyPeriods_WeeklyPeriodId_1150627142" FOREIGN KEY ("WeeklyPeriodId")
REFERENCES public."WeeklyPeriods" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."SavedCareerProfiles"
ADD CONSTRAINT "FK_SavedCareerProfiles_AspNetUsers_AspNetUserId_1458104235" FOREIGN KEY ("AspNetUserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."SavedCareerProfiles"
ADD CONSTRAINT "FK_SavedCareerProfiles_NocCodes2021_Id_907150277" FOREIGN KEY ("NocCodeId2021")
REFERENCES public."NocCodes2021" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."SavedIndustryProfiles"
ADD CONSTRAINT "FK_SavedIndustryProfiles_AspNetUsers_AspNetUserId_1474104292" FOREIGN KEY ("AspNetUserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."SavedIndustryProfiles"
ADD CONSTRAINT "FK_SavedIndustryProfiles_Industries_Id_923150334" FOREIGN KEY ("IndustryId")
REFERENCES public."Industries" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."SavedJobs"
ADD CONSTRAINT "FK_SavedJobs_AspNetUsers_AspNetUserId_514100872" FOREIGN KEY ("AspNetUserId")
REFERENCES public."AspNetUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public."SavedJobs"
ADD CONSTRAINT "FK_SavedJobs_Jobs_JobId_1362103893" FOREIGN KEY ("JobId")
REFERENCES public."Jobs" ("JobId")
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public."SystemSettings"
ADD CONSTRAINT "FK_SystemSettings_AdminUsers_ModifiedByAdminUserId_1490104349" FOREIGN KEY ("ModifiedByAdminUserId")
REFERENCES public."AdminUsers" ("Id")
ON UPDATE NO ACTION
ON DELETE CASCADE;

-- ------------ Write CREATE-FUNCTION-stage scripts -----------

CREATE OR REPLACE FUNCTION public."tvf_GetJobSeekersForDate"(IN par_enddateplus1 TIMESTAMP WITHOUT TIME ZONE)
RETURNS TABLE ("AspNetUserId" VARCHAR, "Email" VARCHAR, "LocationId" INTEGER, "ProvinceId" INTEGER, "CountryId" INTEGER, "DateRegistered" TIMESTAMP WITHOUT TIME ZONE, "AccountStatus" SMALLINT, "EmailConfirmed" NUMERIC, "IsApprentice" NUMERIC, "IsIndigenousPerson" NUMERIC, "IsMatureWorker" NUMERIC, "IsNewImmigrant" NUMERIC, "IsPersonWithDisability" NUMERIC, "IsStudent" NUMERIC, "IsVeteran" NUMERIC, "IsVisibleMinority" NUMERIC, "IsYouth" NUMERIC)
AS
$BODY$
/*
Returns a snapshot of a subset jobseeker data for a specified date.
* Only data the is used by existing reports is included in the snapshot.
*
* Created by Mike Olund <mike@oxd.com>
* March 31, 2020
*
* NOTE:
* Stored procedures and functions are updated using code-first migrations.
* In order to keep localdev, dev, test and prod environments in sync,
* they should never be modified directly in the sql database (unless
* you don't mind having your changes wiped out by a future release).
* PLEASE INCLUDE THIS COMMENT IN THE ALTER STATEMENT OF YOUR MIGRATION!
*/
# variable_conflict use_column
BEGIN
    RETURN QUERY
    WITH periodversion (aspnetuserid, versionnumber)
    AS (SELECT
        aspnetuserid, MAX("VersionNumber") AS versionnumber
        FROM public."JobSeekerVersions"
        WHERE "DateVersionStart" < par_EndDatePlus1 AND ("DateVersionEnd" IS NULL OR "DateVersionEnd" >= par_EndDatePlus1)
        GROUP BY AspnetUserId)
    SELECT
        js."AspNetUserId", js."Email", js."LocationId", js."ProvinceId", js."CountryId", js."DateRegistered", js."AccountStatus", js."EmailConfirmed", js."IsApprentice", js."IsIndigenousPerson", js."IsMatureWorker", js."IsNewImmigrant", js."IsPersonWithDisability", js."IsStudent", js."IsVeteran", js."IsVisibleMinority", js."IsYouth"
        FROM public."JobSeekerVersions" AS js
        INNER JOIN periodversion AS pv
            ON pv.aspnetuserid = js."AspNetUserId"::VARCHAR AND pv.versionnumber = js."VersionNumber"
        WHERE par_EndDatePlus1 < clock_timestamp()
    UNION
    SELECT
        js."Id" AS aspnetuserid, js."Email", js."LocationId", js."ProvinceId", js."CountryId", js."DateRegistered", js."AccountStatus", js."EmailConfirmed", jsf."IsApprentice", jsf."IsIndigenousPerson", jsf."IsMatureWorker", jsf."IsNewImmigrant", jsf."IsPersonWithDisability", jsf."IsStudent", jsf."IsVeteran", jsf."IsVisibleMinority", jsf."IsYouth"
        FROM public."AspNetUsers" AS js
        LEFT OUTER JOIN public."JobSeekerFlags" AS jsf
            ON jsf.AspnetUserId = js."Id"::VARCHAR
        WHERE par_EndDatePlus1 >= clock_timestamp();
END;
$BODY$
LANGUAGE  plpgsql;

CREATE OR REPLACE FUNCTION public."tvf_GetJobsForDate"(IN par_enddateplus1 TIMESTAMP WITHOUT TIME ZONE)
RETURNS TABLE ("JobId" BIGINT, "JobSourceId" SMALLINT, "LocationId" INTEGER, "NocCodeId2021" INTEGER, "IndustryId" SMALLINT, "DateFirstImported" TIMESTAMP WITHOUT TIME ZONE, "PositionsAvailable" SMALLINT)
AS
$BODY$
/*
Returns a snapshot of a subset job data for a specified date.
* Only data the is used by existing reports is included in the snapshot.
*
* NOTE:
* Stored procedures and functions are updated using code-first migrations.
* In order to keep localdev, dev, test and prod environments in sync,
* they should never be modified directly in the sql database (unless
* you don't mind having your changes wiped out by a future release).
* PLEASE INCLUDE THIS COMMENT IN THE ALTER STATEMENT OF YOUR MIGRATION!
*/
# variable_conflict use_column
BEGIN
    RETURN QUERY
    WITH periodversion ("JobId", versionnumber)
    AS (SELECT
        "JobVersions"."JobId", MAX("JobVersions"."VersionNumber") AS versionnumber
        FROM public."JobVersions"
        WHERE "JobVersions"."DateVersionStart" < par_EndDatePlus1 AND ("JobVersions"."DateVersionEnd" IS NULL OR "JobVersions"."DateVersionEnd" >= par_EndDatePlus1)
        GROUP BY "JobVersions"."JobId")
    SELECT
        jv."JobId", jv."JobSourceId", jv."LocationId", jv."NocCodeId2021", jv."IndustryId", jv."DateFirstImported", jv."PositionsAvailable"
        FROM public."JobVersions" AS jv
        INNER JOIN periodversion AS pv
            ON pv."JobId" = jv."JobId" AND pv.versionnumber = jv."VersionNumber";
END;
$BODY$
LANGUAGE  plpgsql;

-- ------------ Write CREATE-PROCEDURE-stage scripts -----------

CREATE OR REPLACE PROCEDURE public."usp_GenerateJobSeekerStats"(IN par_weekenddate TIMESTAMP WITHOUT TIME ZONE, INOUT return_code int DEFAULT 0)
AS
$BODY$
/*
Generates data in the JobSeekerStats table for a 1-week period.
*
* Created by Mike Olund <mike@oxd.com>
* April 18, 2020
*
* NOTE:
* Stored procedures and functions are updated using code-first migrations.
* In order to keep localdev, dev, test and prod environments in sync,
* they should never be modified directly in the sql database (unless
* you don't mind having your changes wiped out by a future release).
* PLEASE INCLUDE THIS COMMENT IN THE ALTER STATEMENT OF YOUR MIGRATION!
*/
DECLARE
    var_StartDate TIMESTAMP WITHOUT TIME ZONE;
    var_EndDatePlus1 TIMESTAMP WITHOUT TIME ZONE;
    var_PeriodId INTEGER;
    var_TableName VARCHAR(25);
BEGIN
    var_TableName := 'JobSeekerStats';
    /* Get the WeeklyPeriod record */
    SELECT
        WeekStartDate, Id
        INTO var_StartDate, var_PeriodId
        FROM public."WeeklyPeriods"
        WHERE WeekEndDate = par_WeekEndDate;
    var_EndDatePlus1 := par_WeekEndDate + (1::NUMERIC || ' DAY')::INTERVAL;
    DELETE FROM public."ReportPersistenceControl"
        WHERE TableName = var_TableName::VARCHAR AND WeeklyPeriodId = var_PeriodId;
    /* also delete associated record from JobSeekerStats */
    DELETE FROM public."JobSeekerStats"
        WHERE WeeklyPeriodId = var_PeriodId;

    BEGIN
        IF NOT EXISTS (SELECT
            *
            FROM public."ReportPersistenceControl"
            WHERE WeeklyPeriodId = var_PeriodId AND TableName = var_TableName::VARCHAR) THEN
            /* Store UserRegions in a Table Variable */
            CREATE TEMPORARY TABLE jobseekerdata$usp_generatejobseekerstats
            (aspnetuserid VARCHAR(450) PRIMARY KEY,
                regionid INTEGER,
                dateregistered TIMESTAMP(6) WITHOUT TIME ZONE NULL,
                accountstatus SMALLINT NULL,
                emailconfirmed NUMERIC(1, 0) NULL,
                isapprentice NUMERIC(1, 0) NULL,
                isindigenousperson NUMERIC(1, 0) NULL,
                ismatureworker NUMERIC(1, 0) NULL,
                isnewimmigrant NUMERIC(1, 0) NULL,
                ispersonwithdisability NUMERIC(1, 0) NULL,
                isstudent NUMERIC(1, 0) NULL,
                isveteran NUMERIC(1, 0) NULL,
                isvisibleminority NUMERIC(1, 0) NULL,
                isyouth NUMERIC(1, 0) NULL);

            IF var_EndDatePlus1 < clock_timestamp() THEN
                INSERT INTO jobseekerdata$usp_generatejobseekerstats
                SELECT
                    aspnetuserid, (CASE
                        WHEN regionid IS NOT NULL THEN regionid
                        WHEN CountryId = 37 AND ProvinceId <> 2 THEN - 1
                        WHEN (CountryId IS NOT NULL AND CountryId <> 37) THEN - 2
                        ELSE 0
                    END) AS regionid, dateregistered, accountstatus, emailconfirmed, isapprentice, isindigenousperson, ismatureworker, isnewimmigrant, ispersonwithdisability, isstudent, isveteran, isvisibleminority, isyouth
                    FROM public."tvf_GetJobSeekersForDate"(var_EndDatePlus1)
                        AS js
                    LEFT OUTER JOIN public."Locations" AS l
                        ON l."LocationId" = js.LocationId;
            ELSE
                INSERT INTO jobseekerdata$usp_generatejobseekerstats
                SELECT
                    js."Id" AS aspnetuserid, (CASE
                        WHEN regionid IS NOT NULL THEN regionid
                        WHEN CountryId = 37 AND ProvinceId <> 2 THEN - 1
                        WHEN (CountryId IS NOT NULL AND CountryId <> 37) THEN - 2
                        ELSE 0
                    END) AS regionid, dateregistered, accountstatus, emailconfirmed, isapprentice, isindigenousperson, ismatureworker, isnewimmigrant, ispersonwithdisability, isstudent, isveteran, isvisibleminority, isyouth
                    FROM public."AspNetUsers" AS js
                    LEFT OUTER JOIN public."JobSeekerFlags" AS jf
                        ON jf."AspNetUserId" = js."Id"
                    LEFT OUTER JOIN public."Locations" AS l
                        ON l."LocationId" = js."LocationId";
            END IF;
            /* insert a record into ReportPersistenceControl */
            INSERT INTO public."ReportPersistenceControl" (weeklyperiodid, tablename, datecalculated, istotaltodate)
            SELECT
                var_PeriodId AS weeklyperiodid, var_TableName AS report, clock_timestamp() AS datecalculated, (CASE
                    WHEN var_EndDatePlus1 > clock_timestamp() THEN 1
                    ELSE 0
                END) AS istotaltodate;
            /* ACCOUNTS BY STATUS */
            /* New Registrations */
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'REGD', regionid, COUNT(*)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE dateregistered >= var_StartDate AND dateregistered < var_EndDatePlus1
                GROUP BY regionid;
            /* Awaiting Email Activation: This is the total at the end of the selected period. */
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'CFEM', regionid, COUNT(DISTINCT je."AspNetUserId")
                FROM public."JobSeekerEventLog" AS je
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd.aspnetuserid = je."AspNetUserId"
                WHERE EventTypeId = 3 AND DateLogged >= var_StartDate AND DateLogged < var_EndDatePlus1
                GROUP BY jd.regionid;
            /* Get the net number of new unactivated accounts for the period */
            /* by subtracting new email confirmations from new registrations. */

            /*
            [9996 - Severity CRITICAL - Transformer error occurred in statement. Please submit report to developers.]
            INSERT INTO dbo.JobSeekerStats
            			(WeeklyPeriodId,[LabelKey],[RegionId],[Value])
            		SELECT WeeklyPeriodId,'NOAC' AS [LabelKey],[RegionId], SUM([Value]) AS [Value]
            		FROM (
            			SELECT WeeklyPeriodId,[LabelKey],[RegionId],[Value]
            			FROM JobSeekerStats
            			WHERE WeeklyPeriodId = @PeriodId AND LabelKey = 'REGD'
            			UNION
            			SELECT WeeklyPeriodId,[LabelKey],[RegionId],-1 * [Value]
            			FROM JobSeekerStats
            			WHERE WeeklyPeriodId = @PeriodId AND LabelKey = 'CFEM'
            		) AS REGD_CFEM
            		GROUP BY WeeklyPeriodId,[RegionId]
            */
            /* Get statistics from the JobSeekerEventLog */
            /* Deactivated: This is accounts deactivated for this period. */
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'DEAC', regionid, COUNT(DISTINCT je."AspNetUserId")
                FROM public."JobSeekerEventLog" AS je
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd.aspnetuserid = je."AspNetUserId"
                WHERE EventTypeId = 4 AND DateLogged >= var_StartDate AND DateLogged < var_EndDatePlus1
                GROUP BY jd.regionid;
            /* Deleted: This is total account deleted for this period. */
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'DEL', regionid, COUNT(DISTINCT je."AspNetUserId")
                FROM public."JobSeekerEventLog" AS je
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd.aspnetuserid = je."AspNetUserId"
                WHERE EventTypeId = 6 AND DateLogged >= var_StartDate AND DateLogged < var_EndDatePlus1
                GROUP BY jd.regionid;
            /* JOB SEEKER EMPLOYMENT GROUPS */
            /* Employment Groups: Total number of accounts */
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'APPR', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isapprentice = 1
                GROUP BY regionid;
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'INDP', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isindigenousperson = 1
                GROUP BY regionid;
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'MAT', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE ismatureworker = 1
                GROUP BY regionid;
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'IMMG', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isnewimmigrant = 1
                GROUP BY regionid;
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'PWD', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE ispersonwithdisability = 1
                GROUP BY regionid;
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'STUD', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isstudent = 1
                GROUP BY regionid;
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'VET', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isveteran = 1
                GROUP BY regionid;
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'VMIN', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isvisibleminority = 1
                GROUP BY regionid;
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'YTH', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isyouth = 1
                GROUP BY regionid;
            /* ACCOUNT ACTIVITY */
            /* Get statistics from the JobSeekerEventLog */
            /* Logins: This is total number of times users successfully logged in for this period. */
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'LOGN', regionid, COUNT(je."AspNetUserId")
                FROM public."JobSeekerEventLog" AS je
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd.aspnetuserid = je."AspNetUserId"
                WHERE EventTypeId = 1 AND DateLogged >= var_StartDate AND DateLogged < var_EndDatePlus1
                GROUP BY jd.regionid;
            /* Job Seekers with Job Alerts, Job Seekers with Saved Career Profiles: */
            /* These are total number of accounts, not new registrations. */
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'ALRT', regionid, COUNT(DISTINCT ja."AspNetUserId")
                FROM public."JobAlerts" AS ja
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd.aspnetuserid = ja."AspNetUserId"
                WHERE DateCreated < var_EndDatePlus1 AND (IsDeleted = 0 OR DateDeleted >= var_EndDatePlus1)
                GROUP BY jd.regionid;
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'CAPR', jd.regionid, COUNT(DISTINCT sc."AspNetUserId")
                FROM public."SavedCareerProfiles" AS sc
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd.aspnetuserid = sc."AspNetUserId"
                WHERE DateSaved < var_EndDatePlus1 AND (IsDeleted = 0 OR DateDeleted >= var_EndDatePlus1)
                GROUP BY jd.regionid;
            INSERT INTO public."JobSeekerStats" (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'INPR', regionid, COUNT(DISTINCT si."AspNetUserId")
                FROM public."SavedIndustryProfiles" AS si
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd.aspnetuserid = si."AspNetUserId"
                WHERE DateSaved < var_EndDatePlus1 AND (IsDeleted = 0 OR DateDeleted >= var_EndDatePlus1)
                GROUP BY jd.regionid;
            DROP TABLE IF EXISTS jobseekerdata$usp_generatejobseekerstats;
        END IF;
        EXCEPTION
            WHEN OTHERS THEN
                DELETE FROM public."ReportPersistenceControl"
                    WHERE TableName = var_TableName::VARCHAR AND WeeklyPeriodId = var_PeriodId;
                /* also delete associated record from JobSeekerStats */
                DELETE FROM public."JobSeekerStats"
                    WHERE WeeklyPeriodId = var_PeriodId;
                return_code := - 1;
                RETURN;
    END;
    return_code := 0;
    RETURN;
END;
$BODY$
LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE public."usp_GenerateJobStats"(IN par_weekenddate TIMESTAMP WITHOUT TIME ZONE, INOUT return_code int DEFAULT 0)
AS
$BODY$
/*
Generates data in the JobStats table for a 1-week period.
*
* Created by Mike Olund <mike@oxd.com>
* April 5, 2020
*
* NOTE:
* Stored procedures and functions are updated using code-first migrations.
* In order to keep localdev, dev, test and prod environments in sync,
* they should never be modified directly in the sql database (unless
* you don't mind having your changes wiped out by a future release).
* PLEASE INCLUDE THIS COMMENT IN THE ALTER STATEMENT OF YOUR MIGRATION!
*/
DECLARE
    var_StartDate TIMESTAMP WITHOUT TIME ZONE;
    var_PeriodId INTEGER;
    var_EndDatePlus1 TIMESTAMP WITHOUT TIME ZONE;
    var_TableName VARCHAR(25);
BEGIN
    var_TableName := 'JobStats';
    /* Get the WeeklyPeriod record */
    SELECT
        WeekStartDate, Id
        INTO var_StartDate, var_PeriodId
        FROM public."WeeklyPeriods"
        WHERE WeekEndDate = par_WeekEndDate;
    var_EndDatePlus1 := par_WeekEndDate + (1::NUMERIC || ' DAY')::INTERVAL;
    /* - Check if a ReportPersistenceControl record exists.  Delete if it is a TotalToDate record */

    IF EXISTS (SELECT
        *
        FROM public."ReportPersistenceControl"
        WHERE TableName = var_TableName::VARCHAR AND WeeklyPeriodId = var_PeriodId AND DateCalculated < par_WeekEndDate + (48::NUMERIC || ' HOUR')::INTERVAL) THEN
        DELETE FROM public."ReportPersistenceControl"
            WHERE TableName = var_TableName::VARCHAR AND WeeklyPeriodId = var_PeriodId;
        /* also delete associated record from JobStats */
        DELETE FROM public."JobStats"
            WHERE WeeklyPeriodId = var_PeriodId;
    END IF;

    BEGIN
        IF NOT EXISTS (SELECT
            *
            FROM public."ReportPersistenceControl"
            WHERE WeeklyPeriodId = var_PeriodId AND TableName = var_TableName::VARCHAR) THEN
            /* insert a record into ReportPersistenceControl */
            INSERT INTO public."ReportPersistenceControl" (weeklyperiodid, tablename, datecalculated, istotaltodate)
            SELECT
                var_PeriodId AS weeklyperiodid, var_TableName AS tablename, clock_timestamp() AS datecalculated, (CASE
                    WHEN par_WeekEndDate + (48::NUMERIC || ' HOUR')::INTERVAL > clock_timestamp() THEN 1
                    ELSE 0
                END) AS istotaltodate;
            /* insert records into JobStats */
            INSERT INTO public."JobStats" (weeklyperiodid, jobsourceid, regionid, jobpostings, positionsavailable)
            SELECT
                var_PeriodId AS weeklyperiodid, j.JobSourceId, COALESCE(l."RegionId", 0) AS regionid, COUNT(*) AS jobpostings, SUM(positionsavailable) AS positionsavailable
                FROM public."tvf_GetJobsForDate"(var_EndDatePlus1)
                    AS j
                LEFT OUTER JOIN public."Locations" AS l
                    ON l."LocationId" = j.LocationId
                WHERE j.DateFirstImported >= var_StartDate AND j.DateFirstImported < var_EndDatePlus1
                GROUP BY j.JobSourceId, l."RegionId";
        END IF;
        EXCEPTION
            WHEN OTHERS THEN
                /* if an error occurs, undo any inserts */
                DELETE FROM public."ReportPersistenceControl"
                    WHERE TableName = var_TableName::VARCHAR AND WeeklyPeriodId = var_PeriodId;
                /* also delete associated record from JobStats */
                DELETE FROM public."JobStats"
                    WHERE WeeklyPeriodId = var_PeriodId;
                return_code := - 1;
                RETURN;
    END;
    return_code := 0;
    RETURN;
END;
$BODY$
LANGUAGE plpgsql;

