--
-- PostgreSQL database dump
--

-- Dumped from database version 15.5 (Debian 15.5-1.pgdg120+1)
-- Dumped by pg_dump version 15.5 (Debian 15.5-1.pgdg120+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

ALTER TABLE ONLY public."SystemSettings" DROP CONSTRAINT "FK_SystemSettings_AdminUsers_ModifiedByAdminUserId_1490104349";
ALTER TABLE ONLY public."SavedJobs" DROP CONSTRAINT "FK_SavedJobs_Jobs_JobId_1362103893";
ALTER TABLE ONLY public."SavedJobs" DROP CONSTRAINT "FK_SavedJobs_AspNetUsers_AspNetUserId_514100872";
ALTER TABLE ONLY public."SavedIndustryProfiles" DROP CONSTRAINT "FK_SavedIndustryProfiles_Industries_Id_923150334";
ALTER TABLE ONLY public."SavedIndustryProfiles" DROP CONSTRAINT "FK_SavedIndustryProfiles_AspNetUsers_AspNetUserId_1474104292";
ALTER TABLE ONLY public."SavedCareerProfiles" DROP CONSTRAINT "FK_SavedCareerProfiles_NocCodes2021_Id_907150277";
ALTER TABLE ONLY public."SavedCareerProfiles" DROP CONSTRAINT "FK_SavedCareerProfiles_AspNetUsers_AspNetUserId_1458104235";
ALTER TABLE ONLY public."ReportPersistenceControl" DROP CONSTRAINT "FK_ReportPersistenceControl_WeeklyPeriods_WeeklyPeriodId_115062";
ALTER TABLE ONLY public."Locations" DROP CONSTRAINT "FK_Locations_Regions_RegionId_446624634";
ALTER TABLE ONLY public."Locations" DROP CONSTRAINT "FK_LocationLookups_Regions_RegionId_254623950";
ALTER TABLE ONLY public."Jobs" DROP CONSTRAINT "FK_Jobs_NocCodes_NocCodeId_1650104919";
ALTER TABLE ONLY public."Jobs" DROP CONSTRAINT "FK_Jobs_NocCodes2021_NocCodeId2021_875150163";
ALTER TABLE ONLY public."Jobs" DROP CONSTRAINT "FK_Jobs_NaicsCodes_NaicsId_14623095";
ALTER TABLE ONLY public."Jobs" DROP CONSTRAINT "FK_Jobs_LocationLookups_LocationId_222623836";
ALTER TABLE ONLY public."Jobs" DROP CONSTRAINT "FK_Jobs_JobSources_JobSourceId_782625831";
ALTER TABLE ONLY public."Jobs" DROP CONSTRAINT "FK_Jobs_JobIds_JobId_1602104748";
ALTER TABLE ONLY public."JobViews" DROP CONSTRAINT "FK_JobViews_Jobs_JobId_1442104178";
ALTER TABLE ONLY public."JobVersions" DROP CONSTRAINT "FK_JobVersions_NocCodes_NocCodeId_110623437";
ALTER TABLE ONLY public."JobVersions" DROP CONSTRAINT "FK_JobVersions_NocCodes2021_NocCodeId2021_891150220";
ALTER TABLE ONLY public."JobVersions" DROP CONSTRAINT "FK_JobVersions_NaicsCodes_NaicsId_94623380";
ALTER TABLE ONLY public."JobVersions" DROP CONSTRAINT "FK_JobVersions_LocationLookups_LocationId_238623893";
ALTER TABLE ONLY public."JobVersions" DROP CONSTRAINT "FK_JobVersions_Jobs_JobId_62623266";
ALTER TABLE ONLY public."JobStats" DROP CONSTRAINT "FK_JobStats_WeeklyPeriods_WeeklyPeriodId_1374627940";
ALTER TABLE ONLY public."JobStats" DROP CONSTRAINT "FK_JobStats_Regions_RegionId_1518628453";
ALTER TABLE ONLY public."JobStats" DROP CONSTRAINT "FK_JobStats_JobSources_JobSourceId_1358627883";
ALTER TABLE ONLY public."JobSeekerVersions" DROP CONSTRAINT "FK_JobSeekerVersions_Provinces_ProvinceId_350624292";
ALTER TABLE ONLY public."JobSeekerVersions" DROP CONSTRAINT "FK_JobSeekerVersions_Locations_LocationId_366624349";
ALTER TABLE ONLY public."JobSeekerVersions" DROP CONSTRAINT "FK_JobSeekerVersions_Countries_CountryId_430624577";
ALTER TABLE ONLY public."JobSeekerVersions" DROP CONSTRAINT "FK_JobSeekerVersions_AspNetUsers_AspNetUserId_302624121";
ALTER TABLE ONLY public."JobSeekerStats" DROP CONSTRAINT "FK_JobSeekerStats_WeeklyPeriods_WeeklyPeriodId_1262627541";
ALTER TABLE ONLY public."JobSeekerStats" DROP CONSTRAINT "FK_JobSeekerStats_Regions_RegionId_1502628396";
ALTER TABLE ONLY public."JobSeekerStats" DROP CONSTRAINT "FK_JobSeekerStats_JobSeekerStatLabels_LabelKey_1454628225";
ALTER TABLE ONLY public."JobSeekerFlags" DROP CONSTRAINT "FK_JobSeekerFlags_AspNetUsers_AspNetUserId_178099675";
ALTER TABLE ONLY public."JobSeekerEventLog" DROP CONSTRAINT "FK_JobSeekerEventLog_AspNetUsers_AspNetUserId_2018106230";
ALTER TABLE ONLY public."JobSeekerChangeLog" DROP CONSTRAINT "FK_JobSeekerChangeLog_AdminUsers_ModifiedByAdminUserId_16306288";
ALTER TABLE ONLY public."JobSeekerChangeLog" DROP CONSTRAINT "FK_JobSeekerAdminLog_AspNetUsers_AspNetUserId_1662628966";
ALTER TABLE ONLY public."JobSeekerAdminComments" DROP CONSTRAINT "FK_JobSeekerAdminComments_AspNetUsers_AspNetUserId_1890105774";
ALTER TABLE ONLY public."JobSeekerAdminComments" DROP CONSTRAINT "FK_JobSeekerAdminComments_AdminUsers_EnteredByAdminUserId_19061";
ALTER TABLE ONLY public."JobIds" DROP CONSTRAINT "FK_JobIds_JobSources_JobSourceId_766625774";
ALTER TABLE ONLY public."JobAlerts" DROP CONSTRAINT "FK_JobAlerts_AspNetUsers_AspNetUserId_1122103038";
ALTER TABLE ONLY public."ImportedJobsWanted" DROP CONSTRAINT "FK_ImportedJobsWanted_JobIds_JobId_1586104691";
ALTER TABLE ONLY public."ImportedJobsFederal" DROP CONSTRAINT "FK_ImportedJobsFederal_JobIds_JobId_1570104634";
ALTER TABLE ONLY public."ImpersonationLog" DROP CONSTRAINT "FK_ImpersonationLog_AspNetUsers_AspNetUserId_1054626800";
ALTER TABLE ONLY public."ImpersonationLog" DROP CONSTRAINT "FK_ImpersonationLog_AdminUsers_AdminUserId_1038626743";
ALTER TABLE ONLY public."ExpiredJobs" DROP CONSTRAINT "FK_ExpiredJobs_JobIds_JobId_1726629194";
ALTER TABLE ONLY public."DeletedJobs" DROP CONSTRAINT "FK_DeletedJobs_Jobs_JobId_990626572";
ALTER TABLE ONLY public."DeletedJobs" DROP CONSTRAINT "FK_DeletedJobs_AdminUsers_DeletedByAdminUserId_974626515";
ALTER TABLE ONLY public."AspNetUsers" DROP CONSTRAINT "FK_AspNetUsers_SecurityQuestions_SecurityQuestionId_850102069";
ALTER TABLE ONLY public."AspNetUsers" DROP CONSTRAINT "FK_AspNetUsers_Provinces_ProvinceId_206623779";
ALTER TABLE ONLY public."AspNetUsers" DROP CONSTRAINT "FK_AspNetUsers_LocationLookups_LocationId_190623722";
ALTER TABLE ONLY public."AspNetUsers" DROP CONSTRAINT "FK_AspNetUsers_Countries_CountryId_414624520";
ALTER TABLE ONLY public."AspNetUsers" DROP CONSTRAINT "FK_AspNetUsers_AdminUsers_LockedByAdminUserId_2050106344";
ALTER TABLE ONLY public."AspNetUserTokens" DROP CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId_2037582297";
ALTER TABLE ONLY public."AspNetUserRoles" DROP CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId_1989582126";
ALTER TABLE ONLY public."AspNetUserRoles" DROP CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId_1973582069";
ALTER TABLE ONLY public."AspNetUserLogins" DROP CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId_1925581898";
ALTER TABLE ONLY public."AspNetUserClaims" DROP CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId_1877581727";
ALTER TABLE ONLY public."AspNetRoleClaims" DROP CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId_1829581556";
ALTER TABLE ONLY public."AdminUsers" DROP CONSTRAINT "FK_AdminUsers_AdminUsers_ModifiedByAdminUserId_1378103950";
ALTER TABLE ONLY public."AdminUsers" DROP CONSTRAINT "FK_AdminUsers_AdminUsers_LockedByAdminUserId_1346103836";
DROP INDEX public."IX_WeeklyPeriods_IX_WeeklyPeriods_WeekEndDate";
DROP INDEX public."IX_SystemSettings_IX_SystemSettings_ModifiedByAdminUserId";
DROP INDEX public."IX_SavedJobs_IX_SavedJobs_JobId";
DROP INDEX public."IX_SavedJobs_IX_SavedJobs_AspNetUserId";
DROP INDEX public."IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_DateSaved";
DROP INDEX public."IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_DateDeleted";
DROP INDEX public."IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_AspNetUserId";
DROP INDEX public."IX_SavedCareerProfiles_IX_SavedCareerProfiles_DateSaved";
DROP INDEX public."IX_SavedCareerProfiles_IX_SavedCareerProfiles_DateDeleted";
DROP INDEX public."IX_SavedCareerProfiles_IX_SavedCareerProfiles_AspNetUserId";
DROP INDEX public."IX_Locations_IX_LocationLookups_RegionId";
DROP INDEX public."IX_Jobs_IX_Jobs_NocCodeId";
DROP INDEX public."IX_Jobs_IX_Jobs_NaicsId";
DROP INDEX public."IX_Jobs_IX_Jobs_LocationId";
DROP INDEX public."IX_Jobs_IX_Jobs_JobSourceId";
DROP INDEX public."IX_JobVersions_IX_JobVersions_NocCodeId";
DROP INDEX public."IX_JobVersions_IX_JobVersions_NaicsId";
DROP INDEX public."IX_JobVersions_IX_JobVersions_LocationId";
DROP INDEX public."IX_JobVersions_IX_JobVersions_JobId_VersionNumber";
DROP INDEX public."IX_JobStats_IX_JobStats_RegionId";
DROP INDEX public."IX_JobStats_IX_JobStats_JobSourceId";
DROP INDEX public."IX_JobSeekerVersions_IX_JobSeekerVersions_ProvinceId";
DROP INDEX public."IX_JobSeekerVersions_IX_JobSeekerVersions_LocationId";
DROP INDEX public."IX_JobSeekerVersions_IX_JobSeekerVersions_CountryId";
DROP INDEX public."IX_JobSeekerVersions_IX_JobSeekerVersions_AspNetUserId_VersionN";
DROP INDEX public."IX_JobSeekerStats_IX_JobSeekerStats_RegionId";
DROP INDEX public."IX_JobSeekerStats_IX_JobSeekerStats_LabelKey";
DROP INDEX public."IX_JobSeekerFlags_IX_JobSeekerFlags_AspNetUserId";
DROP INDEX public."IX_JobSeekerEventLog_IX_JobSeekerEventLog_DateLogged";
DROP INDEX public."IX_JobSeekerEventLog_IX_JobSeekerEventLog_AspNetUserId";
DROP INDEX public."IX_JobSeekerChangeLog_IX_JobSeekerChangeLog_ModifiedByAdminUser";
DROP INDEX public."IX_JobSeekerChangeLog_IX_JobSeekerChangeLog_AspNetUserId";
DROP INDEX public."IX_JobSeekerAdminComments_IX_JobSeekerAdminComments_EnteredByAd";
DROP INDEX public."IX_JobSeekerAdminComments_IX_JobSeekerAdminComments_AspNetUserI";
DROP INDEX public."IX_JobIds_IX_JobIds_JobSourceId";
DROP INDEX public."IX_JobAlerts_IX_JobAlerts_DateCreated";
DROP INDEX public."IX_JobAlerts_IX_JobAlerts_AspNetUserId";
DROP INDEX public."IX_ImportedJobsWanted_IX_ImportedJobsWanted_HashId";
DROP INDEX public."IX_ImpersonationLog_IX_ImpersonationLog_AspNetUserId";
DROP INDEX public."IX_ImpersonationLog_IX_ImpersonationLog_AdminUserId";
DROP INDEX public."IX_GeocodedLocationCache_IX_GeocodedLocationCache_Name";
DROP INDEX public."IX_DeletedJobs_IX_DeletedJobs_DeletedByAdminUserId";
DROP INDEX public."IX_AspNetUsers_UserNameIndex";
DROP INDEX public."IX_AspNetUsers_IX_AspNetUsers_SecurityQuestionId";
DROP INDEX public."IX_AspNetUsers_IX_AspNetUsers_ProvinceId";
DROP INDEX public."IX_AspNetUsers_IX_AspNetUsers_LockedByAdminUserId";
DROP INDEX public."IX_AspNetUsers_IX_AspNetUsers_LocationId";
DROP INDEX public."IX_AspNetUsers_IX_AspNetUsers_LastName_FirstName";
DROP INDEX public."IX_AspNetUsers_IX_AspNetUsers_LastModified";
DROP INDEX public."IX_AspNetUsers_IX_AspNetUsers_FirstName_LastName";
DROP INDEX public."IX_AspNetUsers_IX_AspNetUsers_Email";
DROP INDEX public."IX_AspNetUsers_IX_AspNetUsers_DateRegistered";
DROP INDEX public."IX_AspNetUsers_IX_AspNetUsers_CountryId";
DROP INDEX public."IX_AspNetUsers_IX_AspNetUsers_AccountStatus_LastName_FirstName";
DROP INDEX public."IX_AspNetUsers_EmailIndex";
DROP INDEX public."IX_AspNetUserRoles_IX_AspNetUserRoles_RoleId";
DROP INDEX public."IX_AspNetUserLogins_IX_AspNetUserLogins_UserId";
DROP INDEX public."IX_AspNetUserClaims_IX_AspNetUserClaims_UserId";
DROP INDEX public."IX_AspNetRoles_RoleNameIndex";
DROP INDEX public."IX_AspNetRoleClaims_IX_AspNetRoleClaims_RoleId";
DROP INDEX public."IX_AdminUsers_IX_AdminUsers_ModifiedByAdminUserId";
DROP INDEX public."IX_AdminUsers_IX_AdminUsers_LockedByAdminUserId";
ALTER TABLE ONLY public."__EFMigrationsHistory" DROP CONSTRAINT "PK___EFMigrationsHistory_1221579390";
ALTER TABLE ONLY public."WeeklyPeriods" DROP CONSTRAINT "PK_WeeklyPeriods_542624976";
ALTER TABLE ONLY public."SystemSettings" DROP CONSTRAINT "PK_SystemSettings_1678629023";
ALTER TABLE ONLY public."SecurityQuestions" DROP CONSTRAINT "PK_SecurityQuestions_770101784";
ALTER TABLE ONLY public."SavedJobs" DROP CONSTRAINT "PK_SavedJobs_498100815";
ALTER TABLE ONLY public."SavedIndustryProfiles" DROP CONSTRAINT "PK_SavedIndustryProfiles_1282103608";
ALTER TABLE ONLY public."SavedCareerProfiles" DROP CONSTRAINT "PK_SavedCareerProfiles_1250103494";
ALTER TABLE ONLY public."ReportPersistenceControl" DROP CONSTRAINT "PK_ReportPersistenceControl_1566628624";
ALTER TABLE ONLY public."Regions" DROP CONSTRAINT "PK_Regions_174623665";
ALTER TABLE ONLY public."Provinces" DROP CONSTRAINT "PK_Provinces_142623551";
ALTER TABLE ONLY public."NocCodes" DROP CONSTRAINT "PK_NocCodes_1634104862";
ALTER TABLE ONLY public."NocCodes2021" DROP CONSTRAINT "PK_NocCodes2021_859150106";
ALTER TABLE ONLY public."NocCategories" DROP CONSTRAINT "PK_NocCategories_2114106572";
ALTER TABLE ONLY public."NocCategories2021" DROP CONSTRAINT "PK_NocCategories2021_955150448";
ALTER TABLE ONLY public."Industries" DROP CONSTRAINT "PK_NaicsCodes_2146106686";
ALTER TABLE ONLY public."Locations" DROP CONSTRAINT "PK_LocationLookups_466100701";
ALTER TABLE ONLY public."Jobs" DROP CONSTRAINT "PK_Jobs_802101898";
ALTER TABLE ONLY public."JobViews" DROP CONSTRAINT "PK_JobViews_786101841";
ALTER TABLE ONLY public."JobVersions" DROP CONSTRAINT "PK_JobVersions_46623209";
ALTER TABLE ONLY public."JobStats" DROP CONSTRAINT "PK_JobStats_1342627826";
ALTER TABLE ONLY public."JobSources" DROP CONSTRAINT "PK_JobSources_734625660";
ALTER TABLE ONLY public."JobSeekerVersions" DROP CONSTRAINT "PK_JobSeekerVersions_286624064";
ALTER TABLE ONLY public."JobSeekerStats" DROP CONSTRAINT "PK_JobSeekerStats_1406628054";
ALTER TABLE ONLY public."JobSeekerStatLabels" DROP CONSTRAINT "PK_JobSeekerStatLabels_1438628168";
ALTER TABLE ONLY public."JobSeekerFlags" DROP CONSTRAINT "PK_JobSeekerFlags_98099390";
ALTER TABLE ONLY public."JobSeekerEventLog" DROP CONSTRAINT "PK_JobSeekerEventLog_2002106173";
ALTER TABLE ONLY public."JobSeekerChangeLog" DROP CONSTRAINT "PK_JobSeekerChangeLog_1646628909";
ALTER TABLE ONLY public."JobSeekerAdminComments" DROP CONSTRAINT "PK_JobSeekerAdminComments_1874105717";
ALTER TABLE ONLY public."JobIds" DROP CONSTRAINT "PK_JobIds_1538104520";
ALTER TABLE ONLY public."JobAlerts" DROP CONSTRAINT "PK_JobAlerts_1106102981";
ALTER TABLE ONLY public."ImportedJobsWanted" DROP CONSTRAINT "PK_ImportedJobsWanted_1509580416";
ALTER TABLE ONLY public."ImportedJobsFederal" DROP CONSTRAINT "PK_ImportedJobsFederal_290100074";
ALTER TABLE ONLY public."ImpersonationLog" DROP CONSTRAINT "PK_ImpersonationLog_1022626686";
ALTER TABLE ONLY public."GeocodedLocationCache" DROP CONSTRAINT "PK_GeocodedLocationCache_1669580986";
ALTER TABLE ONLY public."ExpiredJobs" DROP CONSTRAINT "PK_ExpiredJobs_1710629137";
ALTER TABLE ONLY public."DeletedJobs" DROP CONSTRAINT "PK_DeletedJobs_958626458";
ALTER TABLE ONLY public."DataProtectionKeys" DROP CONSTRAINT "PK_DataProtectionKeys_2078630448";
ALTER TABLE ONLY public."Countries" DROP CONSTRAINT "PK_Countries_398624463";
ALTER TABLE ONLY public."AspNetUsers" DROP CONSTRAINT "PK_AspNetUsers_1781581385";
ALTER TABLE ONLY public."AspNetUserTokens" DROP CONSTRAINT "PK_AspNetUserTokens_402100473";
ALTER TABLE ONLY public."AspNetUserRoles" DROP CONSTRAINT "PK_AspNetUserRoles_1957582012";
ALTER TABLE ONLY public."AspNetUserLogins" DROP CONSTRAINT "PK_AspNetUserLogins_418100530";
ALTER TABLE ONLY public."AspNetUserClaims" DROP CONSTRAINT "PK_AspNetUserClaims_1861581670";
ALTER TABLE ONLY public."AspNetRoles" DROP CONSTRAINT "PK_AspNetRoles_1749581271";
ALTER TABLE ONLY public."AspNetRoleClaims" DROP CONSTRAINT "PK_AspNetRoleClaims_1813581499";
ALTER TABLE ONLY public."AdminUsers" DROP CONSTRAINT "PK_AdminUsers_322100188";
DROP TABLE public."__EFMigrationsHistory";
DROP TABLE public."WeeklyPeriods";
DROP TABLE public."SystemSettings";
DROP TABLE public."SecurityQuestions";
DROP TABLE public."SavedJobs";
DROP TABLE public."SavedIndustryProfiles";
DROP TABLE public."SavedCareerProfiles";
DROP TABLE public."ReportPersistenceControl";
DROP TABLE public."Regions";
DROP TABLE public."Provinces";
DROP TABLE public."NocCodes2021";
DROP TABLE public."NocCodes";
DROP TABLE public."NocCategories2021";
DROP TABLE public."NocCategories";
DROP TABLE public."Locations";
DROP TABLE public."Jobs";
DROP TABLE public."JobViews";
DROP TABLE public."JobVersions";
DROP TABLE public."JobStats";
DROP TABLE public."JobSources";
DROP TABLE public."JobSeekerVersions";
DROP TABLE public."JobSeekerStats";
DROP TABLE public."JobSeekerStatLabels";
DROP TABLE public."JobSeekerFlags";
DROP TABLE public."JobSeekerEventLog";
DROP TABLE public."JobSeekerChangeLog";
DROP TABLE public."JobSeekerAdminComments";
DROP TABLE public."JobIds";
DROP TABLE public."JobAlerts";
DROP TABLE public."Industries";
DROP TABLE public."ImportedJobsWanted";
DROP TABLE public."ImportedJobsFederal";
DROP TABLE public."ImpersonationLog";
DROP TABLE public."GeocodedLocationCache";
DROP TABLE public."ExpiredJobs";
DROP TABLE public."DeletedJobs";
DROP TABLE public."DataProtectionKeys";
DROP TABLE public."Countries";
DROP TABLE public."AspNetUsers";
DROP TABLE public."AspNetUserTokens";
DROP TABLE public."AspNetUserRoles";
DROP TABLE public."AspNetUserLogins";
DROP TABLE public."AspNetUserClaims";
DROP TABLE public."AspNetRoles";
DROP TABLE public."AspNetRoleClaims";
DROP TABLE public."AdminUsers";
DROP PROCEDURE public."usp_GenerateJobStats"(IN par_weekenddate timestamp without time zone, INOUT return_code integer);
DROP PROCEDURE public."usp_GenerateJobSeekerStats"(IN par_weekenddate timestamp without time zone, INOUT return_code integer);
DROP FUNCTION public."tvf_GetJobsForDate"(par_enddateplus1 timestamp without time zone);
DROP FUNCTION public."tvf_GetJobSeekersForDate"(par_enddateplus1 timestamp without time zone);
-- *not* dropping schema, since initdb creates it
--
-- Name: public; Type: SCHEMA; Schema: -; Owner: workbc
--

-- *not* creating schema, since initdb creates it


ALTER SCHEMA public OWNER TO workbc;

--
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: workbc
--

COMMENT ON SCHEMA public IS '';


--
-- Name: tvf_GetJobSeekersForDate(timestamp without time zone); Type: FUNCTION; Schema: public; Owner: workbc
--

CREATE FUNCTION public."tvf_GetJobSeekersForDate"(par_enddateplus1 timestamp without time zone) RETURNS TABLE("AspNetUserId" character varying, "Email" character varying, "LocationId" integer, "ProvinceId" integer, "CountryId" integer, "DateRegistered" timestamp without time zone, "AccountStatus" smallint, "EmailConfirmed" boolean, "IsApprentice" boolean, "IsIndigenousPerson" boolean, "IsMatureWorker" boolean, "IsNewImmigrant" boolean, "IsPersonWithDisability" boolean, "IsStudent" boolean, "IsVeteran" boolean, "IsVisibleMinority" boolean, "IsYouth" boolean)
    LANGUAGE plpgsql
    AS $$
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
BEGIN
    RETURN QUERY
    WITH periodversion ("AspNetUserId", "VersionNumber")
    AS (SELECT
        jsv."AspNetUserId", MAX("VersionNumber") AS "VersionNumber"
        FROM public."JobSeekerVersions" AS jsv
        WHERE "DateVersionStart" < par_EndDatePlus1 AND ("DateVersionEnd" IS NULL OR "DateVersionEnd" >= par_EndDatePlus1)
        GROUP BY jsv."AspNetUserId")
    SELECT
        js."AspNetUserId", js."Email", js."LocationId", js."ProvinceId", js."CountryId", js."DateRegistered", js."AccountStatus", js."EmailConfirmed", js."IsApprentice", js."IsIndigenousPerson", js."IsMatureWorker", js."IsNewImmigrant", js."IsPersonWithDisability", js."IsStudent", js."IsVeteran", js."IsVisibleMinority", js."IsYouth"
        FROM public."JobSeekerVersions" AS js
        INNER JOIN periodversion AS pv
            ON pv."AspNetUserId" = js."AspNetUserId"::VARCHAR AND pv."VersionNumber" = js."VersionNumber"
        WHERE par_EndDatePlus1 < clock_timestamp()
    UNION
    SELECT
        js."Id" AS "AspNetUserId", js."Email", js."LocationId", js."ProvinceId", js."CountryId", js."DateRegistered", js."AccountStatus", js."EmailConfirmed", jsf."IsApprentice", jsf."IsIndigenousPerson", jsf."IsMatureWorker", jsf."IsNewImmigrant", jsf."IsPersonWithDisability", jsf."IsStudent", jsf."IsVeteran", jsf."IsVisibleMinority", jsf."IsYouth"
        FROM public."AspNetUsers" AS js
        LEFT OUTER JOIN public."JobSeekerFlags" AS jsf
            ON jsf."AspNetUserId" = js."Id"::VARCHAR
        WHERE par_EndDatePlus1 >= clock_timestamp();
END;
$$;


ALTER FUNCTION public."tvf_GetJobSeekersForDate"(par_enddateplus1 timestamp without time zone) OWNER TO workbc;

--
-- Name: tvf_GetJobsForDate(timestamp without time zone); Type: FUNCTION; Schema: public; Owner: workbc
--

CREATE FUNCTION public."tvf_GetJobsForDate"(par_enddateplus1 timestamp without time zone) RETURNS TABLE("JobId" bigint, "JobSourceId" smallint, "LocationId" integer, "NocCodeId2021" integer, "IndustryId" smallint, "DateFirstImported" timestamp without time zone, "PositionsAvailable" smallint)
    LANGUAGE plpgsql
    AS $$
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
$$;


ALTER FUNCTION public."tvf_GetJobsForDate"(par_enddateplus1 timestamp without time zone) OWNER TO workbc;

--
-- Name: usp_GenerateJobSeekerStats(timestamp without time zone, integer); Type: PROCEDURE; Schema: public; Owner: workbc
--

CREATE PROCEDURE public."usp_GenerateJobSeekerStats"(IN par_weekenddate timestamp without time zone, INOUT return_code integer DEFAULT 0)
    LANGUAGE plpgsql
    AS $_$
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
        "WeekStartDate", "Id"
        INTO var_StartDate, var_PeriodId
        FROM public."WeeklyPeriods"
        WHERE "WeekEndDate" = par_WeekEndDate;
    var_EndDatePlus1 := par_WeekEndDate + (1::NUMERIC || ' DAY')::INTERVAL;
    DELETE FROM public."ReportPersistenceControl"
        WHERE "TableName" = var_TableName::VARCHAR AND "WeeklyPeriodId" = var_PeriodId;
    /* also delete associated record from JobSeekerStats */
    DELETE FROM public."JobSeekerStats"
        WHERE "WeeklyPeriodId" = var_PeriodId;

    BEGIN
        IF NOT EXISTS (SELECT
            *
            FROM public."ReportPersistenceControl"
            WHERE "WeeklyPeriodId" = var_PeriodId AND "TableName" = var_TableName::VARCHAR) THEN
            /* Store UserRegions in a Table Variable */
            CREATE TEMPORARY TABLE jobseekerdata$usp_generatejobseekerstats
            ("AspNetUserId" VARCHAR(450) PRIMARY KEY,
                "RegionId" INTEGER,
                "DateRegistered" TIMESTAMP(6) WITHOUT TIME ZONE NULL,
                "AccountStatus" SMALLINT NULL,
                "EmailConfirmed" BOOLEAN NULL,
                "IsApprentice" BOOLEAN NULL,
                "IsIndigenousPerson" BOOLEAN NULL,
                "IsMatureWorker" BOOLEAN NULL,
                "IsNewImmigrant" BOOLEAN NULL,
                "IsPersonWithDisability" BOOLEAN NULL,
                "IsStudent" BOOLEAN NULL,
                "IsVeteran" BOOLEAN NULL,
                "IsVisibleMinority" BOOLEAN NULL,
                "IsYouth" BOOLEAN NULL);

            IF var_EndDatePlus1 < clock_timestamp() THEN
                INSERT INTO jobseekerdata$usp_generatejobseekerstats
                SELECT
                    "AspNetUserId", (CASE
                        WHEN "RegionId" IS NOT NULL THEN "RegionId"
                        WHEN "CountryId" = 37 AND "ProvinceId" <> 2 THEN - 1
                        WHEN ("CountryId" IS NOT NULL AND "CountryId" <> 37) THEN - 2
                        ELSE 0
                    END) AS "RegionId", "DateRegistered", "AccountStatus", "EmailConfirmed", "IsApprentice", "IsIndigenousPerson", "IsMatureWorker", "IsNewImmigrant", "IsPersonWithDisability", "IsStudent", "IsVeteran", "IsVisibleMinority", "IsYouth"
                    FROM public."tvf_GetJobSeekersForDate"(var_EndDatePlus1)
                        AS js
                    LEFT OUTER JOIN public."Locations" AS l
                        ON l."LocationId" = js."LocationId";
            ELSE
                INSERT INTO jobseekerdata$usp_generatejobseekerstats
                SELECT
                    js."Id" AS "AspNetUserId", (CASE
                        WHEN "RegionId" IS NOT NULL THEN "RegionId"
                        WHEN "CountryId" = 37 AND "ProvinceId" <> 2 THEN - 1
                        WHEN ("CountryId" IS NOT NULL AND "CountryId" <> 37) THEN - 2
                        ELSE 0
                    END) AS "RegionId", "DateRegistered", "AccountStatus", "EmailConfirmed", "IsApprentice", "IsIndigenousPerson", "IsMatureWorker", "IsNewImmigrant", "IsPersonWithDisability", "IsStudent", "IsVeteran", "IsVisibleMinority", "IsYouth"
                    FROM public."AspNetUsers" AS js
                    LEFT OUTER JOIN public."JobSeekerFlags" AS jf
                        ON jf."AspNetUserId" = js."Id"
                    LEFT OUTER JOIN public."Locations" AS l
                        ON l."LocationId" = js."LocationId";
            END IF;
            /* insert a record into ReportPersistenceControl */
            INSERT INTO public."ReportPersistenceControl" ("WeeklyPeriodId", "TableName", "DateCalculated", "IsTotalToDate")
            SELECT
                var_PeriodId AS "WeeklyPeriodId", var_TableName AS report, clock_timestamp() AS "DateCalculated", (CASE
                    WHEN var_EndDatePlus1 > clock_timestamp() THEN true
                    ELSE false
                END) AS "IsTotalToDate";
            /* ACCOUNTS BY STATUS */
            /* New Registrations */
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'REGD', "RegionId", COUNT(*)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE "DateRegistered" >= var_StartDate AND "DateRegistered" < var_EndDatePlus1
                GROUP BY "RegionId";
            /* Awaiting Email Activation: This is the total at the end of the selected period. */
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'CFEM', "RegionId", COUNT(DISTINCT je."AspNetUserId")
                FROM public."JobSeekerEventLog" AS je
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd."AspNetUserId" = je."AspNetUserId"
                WHERE "EventTypeId" = 3 AND "DateLogged" >= var_StartDate AND "DateLogged" < var_EndDatePlus1
                GROUP BY jd."RegionId";
            /* Get the net number of new unactivated accounts for the period */
            /* by subtracting new email confirmations from new registrations. */
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT "WeeklyPeriodId", 'NOAC' AS "LabelKey", "RegionId", SUM("Value") AS "Value"
            FROM (
                SELECT "WeeklyPeriodId","LabelKey","RegionId","Value"
                FROM "JobSeekerStats"
                WHERE "WeeklyPeriodId" = var_PeriodId AND "LabelKey" = 'REGD'
                UNION
                SELECT "WeeklyPeriodId","LabelKey","RegionId",-1 * "Value"
                FROM "JobSeekerStats"
                WHERE "WeeklyPeriodId" = var_PeriodId AND "LabelKey" = 'CFEM'
            ) AS REGD_CFEM
            GROUP BY "WeeklyPeriodId","RegionId";
            /* Get statistics from the JobSeekerEventLog */
            /* Deactivated: This is accounts deactivated for this period. */
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'DEAC', "RegionId", COUNT(DISTINCT je."AspNetUserId")
                FROM public."JobSeekerEventLog" AS je
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd."AspNetUserId" = je."AspNetUserId"
                WHERE "EventTypeId" = 4 AND "DateLogged" >= var_StartDate AND "DateLogged" < var_EndDatePlus1
                GROUP BY jd."RegionId";
            /* Deleted: This is total account deleted for this period. */
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'DEL', "RegionId", COUNT(DISTINCT je."AspNetUserId")
                FROM public."JobSeekerEventLog" AS je
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd."AspNetUserId" = je."AspNetUserId"
                WHERE "EventTypeId" = 6 AND "DateLogged" >= var_StartDate AND "DateLogged" < var_EndDatePlus1
                GROUP BY jd."RegionId";
            /* JOB SEEKER EMPLOYMENT GROUPS */
            /* Employment Groups: Total number of accounts */
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'APPR', "RegionId", COUNT(DISTINCT "AspNetUserId")
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE "IsApprentice" = true
                GROUP BY "RegionId";
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'INDP', "RegionId", COUNT(DISTINCT "AspNetUserId")
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE "IsIndigenousPerson" = true
                GROUP BY "RegionId";
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'MAT', "RegionId", COUNT(DISTINCT "AspNetUserId")
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE "IsMatureWorker" = true
                GROUP BY "RegionId";
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'IMMG', "RegionId", COUNT(DISTINCT "AspNetUserId")
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE "IsNewImmigrant" = true
                GROUP BY "RegionId";
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'PWD', "RegionId", COUNT(DISTINCT "AspNetUserId")
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE "IsPersonWithDisability" = true
                GROUP BY "RegionId";
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'STUD', "RegionId", COUNT(DISTINCT "AspNetUserId")
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE "IsStudent" = true
                GROUP BY "RegionId";
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'VET', "RegionId", COUNT(DISTINCT "AspNetUserId")
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE "IsVeteran" = true
                GROUP BY "RegionId";
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'VMIN', "RegionId", COUNT(DISTINCT "AspNetUserId")
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE "IsVisibleMinority" = true
                GROUP BY "RegionId";
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'YTH', "RegionId", COUNT(DISTINCT "AspNetUserId")
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE "IsYouth" = true
                GROUP BY "RegionId";
            /* ACCOUNT ACTIVITY */
            /* Get statistics from the JobSeekerEventLog */
            /* Logins: This is total number of times users successfully logged in for this period. */
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'LOGN', "RegionId", COUNT(je."AspNetUserId")
                FROM public."JobSeekerEventLog" AS je
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd."AspNetUserId" = je."AspNetUserId"
                WHERE "EventTypeId" = 1 AND "DateLogged" >= var_StartDate AND "DateLogged" < var_EndDatePlus1
                GROUP BY jd."RegionId";
            /* Job Seekers with Job Alerts, Job Seekers with Saved Career Profiles: */
            /* These are total number of accounts, not new registrations. */
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'ALRT', "RegionId", COUNT(DISTINCT ja."AspNetUserId")
                FROM public."JobAlerts" AS ja
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd."AspNetUserId" = ja."AspNetUserId"
                WHERE "DateCreated" < var_EndDatePlus1 AND ("IsDeleted" = false OR "DateDeleted" >= var_EndDatePlus1)
                GROUP BY jd."RegionId";
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'CAPR', jd."RegionId", COUNT(DISTINCT sc."AspNetUserId")
                FROM public."SavedCareerProfiles" AS sc
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd."AspNetUserId" = sc."AspNetUserId"
                WHERE "DateSaved" < var_EndDatePlus1 AND ("IsDeleted" = false OR "DateDeleted" >= var_EndDatePlus1)
                GROUP BY jd."RegionId";
            INSERT INTO public."JobSeekerStats" ("WeeklyPeriodId", "LabelKey", "RegionId", "Value")
            SELECT
                var_PeriodId, 'INPR', "RegionId", COUNT(DISTINCT si."AspNetUserId")
                FROM public."SavedIndustryProfiles" AS si
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON jd."AspNetUserId" = si."AspNetUserId"
                WHERE "DateSaved" < var_EndDatePlus1 AND ("IsDeleted" = false OR "DateDeleted" >= var_EndDatePlus1)
                GROUP BY jd."RegionId";
            DROP TABLE IF EXISTS jobseekerdata$usp_generatejobseekerstats;
        END IF;
        EXCEPTION   
            WHEN OTHERS THEN
                DELETE FROM public."ReportPersistenceControl"
                    WHERE "TableName" = var_TableName::VARCHAR AND "WeeklyPeriodId" = var_PeriodId;
                /* also delete associated record from JobSeekerStats */
                DELETE FROM public."JobSeekerStats"
                    WHERE "WeeklyPeriodId" = var_PeriodId;
                return_code := - 1;
                RETURN;
    END;
    return_code := 0;
    RETURN;
END;
$_$;


ALTER PROCEDURE public."usp_GenerateJobSeekerStats"(IN par_weekenddate timestamp without time zone, INOUT return_code integer) OWNER TO workbc;

--
-- Name: usp_GenerateJobStats(timestamp without time zone, integer); Type: PROCEDURE; Schema: public; Owner: workbc
--

CREATE PROCEDURE public."usp_GenerateJobStats"(IN par_weekenddate timestamp without time zone, INOUT return_code integer DEFAULT 0)
    LANGUAGE plpgsql
    AS $$
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
        "WeekStartDate", "Id"
        INTO var_StartDate, var_PeriodId
        FROM public."WeeklyPeriods"
        WHERE "WeekEndDate" = par_WeekEndDate;
    var_EndDatePlus1 := par_WeekEndDate + (1::NUMERIC || ' DAY')::INTERVAL;
    /* - Check if a ReportPersistenceControl record exists.  Delete if it is a TotalToDate record */

    IF EXISTS (SELECT
        *
        FROM public."ReportPersistenceControl"
        WHERE "TableName" = var_TableName::VARCHAR AND "WeeklyPeriodId" = var_PeriodId AND "DateCalculated" < par_WeekEndDate + (48::NUMERIC || ' HOUR')::INTERVAL) THEN
        DELETE FROM public."ReportPersistenceControl"
            WHERE "TableName" = var_TableName::VARCHAR AND "WeeklyPeriodId" = var_PeriodId;
        /* also delete associated record from JobStats */
        DELETE FROM public."JobStats"
            WHERE "WeeklyPeriodId" = var_PeriodId;
    END IF;

    BEGIN
        IF NOT EXISTS (SELECT
            *
            FROM public."ReportPersistenceControl"
            WHERE "WeeklyPeriodId" = var_PeriodId AND "TableName" = var_TableName::VARCHAR) THEN
            /* insert a record into ReportPersistenceControl */
            INSERT INTO public."ReportPersistenceControl" ("WeeklyPeriodId", "TableName", "DateCalculated", "IsTotalToDate")
            SELECT
                var_PeriodId AS "WeeklyPeriodId", var_TableName AS "TableName", clock_timestamp() AS "DateCalculated", (CASE
                    WHEN par_WeekEndDate + (48::NUMERIC || ' HOUR')::INTERVAL > clock_timestamp() THEN true
                    ELSE false
                END) AS "IsTotalToDate";
            /* insert records into JobStats */
            INSERT INTO public."JobStats" ("WeeklyPeriodId", "JobSourceId", "RegionId", "JobPostings", "PositionsAvailable")
            SELECT
                var_PeriodId AS "WeeklyPeriodId", j."JobSourceId", COALESCE(l."RegionId", 0) AS "RegionId", COUNT(*) AS "JobPostings", SUM("PositionsAvailable") AS "PositionsAvailable"
                FROM public."tvf_GetJobsForDate"(var_EndDatePlus1)
                    AS j
                LEFT OUTER JOIN public."Locations" AS l
                    ON l."LocationId" = j."LocationId"
                WHERE j."DateFirstImported" >= var_StartDate AND j."DateFirstImported" < var_EndDatePlus1
                GROUP BY j."JobSourceId", l."RegionId";
        END IF;
        EXCEPTION
            WHEN OTHERS THEN
                /* if an error occurs, undo any inserts */
                DELETE FROM public."ReportPersistenceControl"
                    WHERE "TableName" = var_TableName::VARCHAR AND "WeeklyPeriodId" = var_PeriodId;
                /* also delete associated record from JobStats */
                DELETE FROM public."JobStats"
                    WHERE "WeeklyPeriodId" = var_PeriodId;
                return_code := - 1;
                RETURN;
    END;
    return_code := 0;
    RETURN;
END;
$$;


ALTER PROCEDURE public."usp_GenerateJobStats"(IN par_weekenddate timestamp without time zone, INOUT return_code integer) OWNER TO workbc;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: AdminUsers; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."AdminUsers" (
    "Id" integer NOT NULL,
    "SamAccountName" character varying(20),
    "DisplayName" character varying(60) NOT NULL,
    "DateUpdated" timestamp(6) without time zone NOT NULL,
    "AdminLevel" integer NOT NULL,
    "DateCreated" timestamp(6) without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "Deleted" boolean NOT NULL,
    "Guid" character varying(40),
    "DateLocked" timestamp(6) without time zone,
    "LockedByAdminUserId" integer,
    "ModifiedByAdminUserId" integer,
    "DateLastLogin" timestamp(6) without time zone,
    "GivenName" character varying(40),
    "Surname" character varying(40)
);


ALTER TABLE public."AdminUsers" OWNER TO workbc;

--
-- Name: AdminUsers_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."AdminUsers" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."AdminUsers_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetRoleClaims; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."AspNetRoleClaims" (
    "Id" integer NOT NULL,
    "RoleId" character varying(450) NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetRoleClaims" OWNER TO workbc;

--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."AspNetRoleClaims" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."AspNetRoleClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetRoles; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."AspNetRoles" (
    "Id" character varying(450) NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "ConcurrencyStamp" text
);


ALTER TABLE public."AspNetRoles" OWNER TO workbc;

--
-- Name: AspNetUserClaims; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."AspNetUserClaims" (
    "Id" integer NOT NULL,
    "UserId" character varying(450) NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetUserClaims" OWNER TO workbc;

--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."AspNetUserClaims" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."AspNetUserClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetUserLogins; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."AspNetUserLogins" (
    "LoginProvider" character varying(128) NOT NULL,
    "ProviderKey" character varying(128) NOT NULL,
    "ProviderDisplayName" text,
    "UserId" character varying(450) NOT NULL
);


ALTER TABLE public."AspNetUserLogins" OWNER TO workbc;

--
-- Name: AspNetUserRoles; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."AspNetUserRoles" (
    "UserId" character varying(450) NOT NULL,
    "RoleId" character varying(450) NOT NULL
);


ALTER TABLE public."AspNetUserRoles" OWNER TO workbc;

--
-- Name: AspNetUserTokens; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."AspNetUserTokens" (
    "UserId" character varying(450) NOT NULL,
    "LoginProvider" character varying(128) NOT NULL,
    "Name" character varying(128) NOT NULL,
    "Value" text
);


ALTER TABLE public."AspNetUserTokens" OWNER TO workbc;

--
-- Name: AspNetUsers; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."AspNetUsers" (
    "Id" character varying(450) NOT NULL,
    "UserName" character varying(256),
    "NormalizedUserName" character varying(256),
    "Email" character varying(256),
    "NormalizedEmail" character varying(256),
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "PhoneNumber" text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp(6) with time zone,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL,
    "LocationId" integer,
    "City" character varying(50),
    "CountryId" integer,
    "FirstName" character varying(50),
    "LastName" character varying(50),
    "LegacyWebUserId" integer,
    "ProvinceId" integer,
    "AccountStatus" smallint NOT NULL,
    "DateRegistered" timestamp(6) without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "LastLogon" timestamp(6) without time zone,
    "LastModified" timestamp(6) without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "VerificationGuid" uuid,
    "SecurityAnswer" character varying(50),
    "SecurityQuestionId" integer,
    "DateLocked" timestamp(6) without time zone,
    "LockedByAdminUserId" integer
);


ALTER TABLE public."AspNetUsers" OWNER TO workbc;

--
-- Name: Countries; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."Countries" (
    "Id" integer NOT NULL,
    "Name" character varying(50),
    "CountryTwoLetterCode" character varying(2),
    "SortOrder" smallint NOT NULL
);


ALTER TABLE public."Countries" OWNER TO workbc;

--
-- Name: DataProtectionKeys; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."DataProtectionKeys" (
    "Id" integer NOT NULL,
    "FriendlyName" text,
    "Xml" text
);


ALTER TABLE public."DataProtectionKeys" OWNER TO workbc;

--
-- Name: DataProtectionKeys_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."DataProtectionKeys" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."DataProtectionKeys_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: DeletedJobs; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."DeletedJobs" (
    "JobId" bigint NOT NULL,
    "DeletedByAdminUserId" integer NOT NULL,
    "DateDeleted" timestamp(6) without time zone NOT NULL
);


ALTER TABLE public."DeletedJobs" OWNER TO workbc;

--
-- Name: ExpiredJobs; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."ExpiredJobs" (
    "JobId" bigint NOT NULL,
    "RemovedFromElasticsearch" boolean NOT NULL,
    "DateRemoved" timestamp(6) without time zone NOT NULL
);


ALTER TABLE public."ExpiredJobs" OWNER TO workbc;

--
-- Name: GeocodedLocationCache; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."GeocodedLocationCache" (
    "Id" integer NOT NULL,
    "Name" character varying(120),
    "Latitude" character varying(25),
    "Longitude" character varying(25),
    "DateGeocoded" timestamp(6) without time zone NOT NULL,
    "IsPermanent" boolean NOT NULL,
    "City" character varying(80),
    "Province" character varying(2),
    "FrenchCity" character varying(80)
);


ALTER TABLE public."GeocodedLocationCache" OWNER TO workbc;

--
-- Name: GeocodedLocationCache_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."GeocodedLocationCache" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."GeocodedLocationCache_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ImpersonationLog; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."ImpersonationLog" (
    "Token" character varying(200) NOT NULL,
    "AspNetUserId" character varying(450),
    "AdminUserId" integer NOT NULL,
    "DateTokenCreated" timestamp(6) without time zone NOT NULL
);


ALTER TABLE public."ImpersonationLog" OWNER TO workbc;

--
-- Name: ImportedJobsFederal; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."ImportedJobsFederal" (
    "JobId" bigint NOT NULL,
    "ApiDate" timestamp(6) without time zone NOT NULL,
    "DateFirstImported" timestamp(6) without time zone NOT NULL,
    "JobPostEnglish" text,
    "JobPostFrench" text,
    "ReIndexNeeded" boolean NOT NULL,
    "DisplayUntil" timestamp(6) without time zone,
    "DateLastImported" timestamp(6) without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL
);


ALTER TABLE public."ImportedJobsFederal" OWNER TO workbc;

--
-- Name: ImportedJobsWanted; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."ImportedJobsWanted" (
    "JobId" bigint NOT NULL,
    "JobPostEnglish" text,
    "DateFirstImported" timestamp(6) without time zone NOT NULL,
    "ApiDate" timestamp(6) without time zone NOT NULL,
    "ReIndexNeeded" boolean NOT NULL,
    "DateLastImported" timestamp(6) without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "IsFederalOrWorkBc" boolean NOT NULL,
    "HashId" bigint NOT NULL,
    "DateLastSeen" timestamp(6) without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL
);


ALTER TABLE public."ImportedJobsWanted" OWNER TO workbc;

--
-- Name: Industries; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."Industries" (
    "Id" smallint NOT NULL,
    "Title" character varying(150),
    "TitleBC" character varying(150)
);


ALTER TABLE public."Industries" OWNER TO workbc;

--
-- Name: JobAlerts; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."JobAlerts" (
    "Id" integer NOT NULL,
    "Title" character varying(50),
    "AlertFrequency" smallint NOT NULL,
    "UrlParameters" character varying(1000),
    "DateCreated" timestamp(6) without time zone NOT NULL,
    "DateModified" timestamp(6) without time zone,
    "DateDeleted" timestamp(6) without time zone,
    "IsDeleted" boolean NOT NULL,
    "AspNetUserId" character varying(450),
    "JobSearchFilters" text,
    "JobSearchFiltersVersion" integer DEFAULT 0 NOT NULL
);


ALTER TABLE public."JobAlerts" OWNER TO workbc;

--
-- Name: JobAlerts_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."JobAlerts" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."JobAlerts_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: JobIds; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."JobIds" (
    "Id" bigint NOT NULL,
    "DateFirstImported" timestamp(6) without time zone NOT NULL,
    "JobSourceId" smallint NOT NULL
);


ALTER TABLE public."JobIds" OWNER TO workbc;

--
-- Name: JobSeekerAdminComments; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."JobSeekerAdminComments" (
    "Id" integer NOT NULL,
    "AspNetUserId" character varying(450),
    "Comment" text,
    "IsPinned" boolean NOT NULL,
    "EnteredByAdminUserId" integer NOT NULL,
    "DateEntered" timestamp(6) without time zone NOT NULL
);


ALTER TABLE public."JobSeekerAdminComments" OWNER TO workbc;

--
-- Name: JobSeekerAdminComments_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."JobSeekerAdminComments" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."JobSeekerAdminComments_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: JobSeekerChangeLog; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."JobSeekerChangeLog" (
    "Id" integer NOT NULL,
    "AspNetUserId" character varying(450),
    "Field" character varying(100),
    "OldValue" text,
    "NewValue" text,
    "ModifiedByAdminUserId" integer,
    "DateUpdated" timestamp(6) without time zone NOT NULL
);


ALTER TABLE public."JobSeekerChangeLog" OWNER TO workbc;

--
-- Name: JobSeekerChangeLog_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."JobSeekerChangeLog" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."JobSeekerChangeLog_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: JobSeekerEventLog; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."JobSeekerEventLog" (
    "Id" integer NOT NULL,
    "AspNetUserId" character varying(450),
    "EventTypeId" integer NOT NULL,
    "DateLogged" timestamp(6) without time zone NOT NULL
);


ALTER TABLE public."JobSeekerEventLog" OWNER TO workbc;

--
-- Name: JobSeekerEventLog_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."JobSeekerEventLog" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."JobSeekerEventLog_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: JobSeekerFlags; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."JobSeekerFlags" (
    "Id" integer NOT NULL,
    "AspNetUserId" character varying(450),
    "IsApprentice" boolean NOT NULL,
    "IsIndigenousPerson" boolean NOT NULL,
    "IsMatureWorker" boolean NOT NULL,
    "IsNewImmigrant" boolean NOT NULL,
    "IsPersonWithDisability" boolean NOT NULL,
    "IsStudent" boolean NOT NULL,
    "IsVeteran" boolean NOT NULL,
    "IsVisibleMinority" boolean NOT NULL,
    "IsYouth" boolean NOT NULL
);


ALTER TABLE public."JobSeekerFlags" OWNER TO workbc;

--
-- Name: JobSeekerFlags_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."JobSeekerFlags" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."JobSeekerFlags_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: JobSeekerStatLabels; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."JobSeekerStatLabels" (
    "Key" character varying(4) NOT NULL,
    "Label" character varying(100),
    "IsTotal" boolean NOT NULL
);


ALTER TABLE public."JobSeekerStatLabels" OWNER TO workbc;

--
-- Name: JobSeekerStats; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."JobSeekerStats" (
    "WeeklyPeriodId" integer NOT NULL,
    "Value" integer NOT NULL,
    "RegionId" integer NOT NULL,
    "LabelKey" character varying(4) DEFAULT ''::character varying NOT NULL
);


ALTER TABLE public."JobSeekerStats" OWNER TO workbc;

--
-- Name: JobSeekerVersions; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."JobSeekerVersions" (
    "Id" bigint NOT NULL,
    "AspNetUserId" character varying(450),
    "CountryId" integer,
    "ProvinceId" integer,
    "LocationId" integer,
    "DateRegistered" timestamp(6) without time zone NOT NULL,
    "AccountStatus" smallint NOT NULL,
    "EmailConfirmed" boolean NOT NULL,
    "IsApprentice" boolean NOT NULL,
    "IsIndigenousPerson" boolean NOT NULL,
    "IsMatureWorker" boolean NOT NULL,
    "IsNewImmigrant" boolean NOT NULL,
    "IsPersonWithDisability" boolean NOT NULL,
    "IsStudent" boolean NOT NULL,
    "IsVeteran" boolean NOT NULL,
    "IsVisibleMinority" boolean NOT NULL,
    "IsYouth" boolean NOT NULL,
    "DateVersionStart" timestamp(6) without time zone NOT NULL,
    "DateVersionEnd" timestamp(6) without time zone,
    "IsCurrentVersion" boolean NOT NULL,
    "VersionNumber" smallint NOT NULL,
    "Email" character varying(256)
);


ALTER TABLE public."JobSeekerVersions" OWNER TO workbc;

--
-- Name: JobSeekerVersions_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."JobSeekerVersions" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."JobSeekerVersions_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: JobSources; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."JobSources" (
    "Id" smallint NOT NULL,
    "Name" character varying(50),
    "GroupName" character varying(50),
    "ListOrder" smallint NOT NULL
);


ALTER TABLE public."JobSources" OWNER TO workbc;

--
-- Name: JobStats; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."JobStats" (
    "WeeklyPeriodId" integer NOT NULL,
    "JobSourceId" smallint NOT NULL,
    "RegionId" integer NOT NULL,
    "JobPostings" integer NOT NULL,
    "PositionsAvailable" integer NOT NULL
);


ALTER TABLE public."JobStats" OWNER TO workbc;

--
-- Name: JobVersions; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."JobVersions" (
    "Id" bigint NOT NULL,
    "JobId" bigint NOT NULL,
    "LocationId" integer NOT NULL,
    "NocCodeId" smallint,
    "IndustryId" smallint,
    "PositionsAvailable" smallint NOT NULL,
    "DatePosted" timestamp(6) without time zone NOT NULL,
    "IsActive" boolean NOT NULL,
    "DateVersionStart" timestamp(6) without time zone NOT NULL,
    "DateVersionEnd" timestamp(6) without time zone,
    "IsCurrentVersion" boolean NOT NULL,
    "VersionNumber" smallint NOT NULL,
    "ActualDatePosted" timestamp(6) without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "DateFirstImported" timestamp(6) without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "JobSourceId" smallint NOT NULL,
    "NocCodeId2021" integer
);


ALTER TABLE public."JobVersions" OWNER TO workbc;

--
-- Name: JobVersions_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."JobVersions" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."JobVersions_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: JobViews; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."JobViews" (
    "JobId" bigint NOT NULL,
    "Views" integer,
    "DateLastViewed" timestamp(6) without time zone NOT NULL
);


ALTER TABLE public."JobViews" OWNER TO workbc;

--
-- Name: Jobs; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."Jobs" (
    "JobId" bigint NOT NULL,
    "Title" character varying(300),
    "NocCodeId" smallint,
    "PositionsAvailable" smallint NOT NULL,
    "EmployerName" character varying(100),
    "DatePosted" timestamp(6) without time zone NOT NULL,
    "Casual" boolean NOT NULL,
    "City" character varying(120),
    "ExpireDate" timestamp(6) without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "FullTime" boolean NOT NULL,
    "LastUpdated" timestamp(6) without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "LeadingToFullTime" boolean NOT NULL,
    "LocationId" integer DEFAULT 0 NOT NULL,
    "IndustryId" smallint,
    "PartTime" boolean NOT NULL,
    "Permanent" boolean NOT NULL,
    "Salary" numeric(18,2),
    "SalarySummary" character varying(60),
    "Seasonal" boolean NOT NULL,
    "Temporary" boolean NOT NULL,
    "DateFirstImported" timestamp(6) without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "IsActive" boolean NOT NULL,
    "DateLastImported" timestamp(6) without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "JobSourceId" smallint NOT NULL,
    "OriginalSource" character varying(100),
    "ExternalSourceUrl" character varying(800),
    "ActualDatePosted" timestamp(6) without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "NocCodeId2021" integer
);


ALTER TABLE public."Jobs" OWNER TO workbc;

--
-- Name: Locations; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."Locations" (
    "LocationId" integer NOT NULL,
    "EDM_Location_DistrictLocationId" integer NOT NULL,
    "RegionId" integer,
    "FederalCityId" integer,
    "City" character varying(50),
    "Label" character varying(50),
    "IsDuplicate" boolean NOT NULL,
    "IsHidden" boolean NOT NULL,
    "Latitude" character varying(25),
    "Longitude" character varying(25),
    "BcStatsPlaceId" integer
);


ALTER TABLE public."Locations" OWNER TO workbc;

--
-- Name: NocCategories; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."NocCategories" (
    "CategoryCode" character varying(3) NOT NULL,
    "Level" smallint NOT NULL,
    "Title" character varying(150)
);


ALTER TABLE public."NocCategories" OWNER TO workbc;

--
-- Name: NocCategories2021; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."NocCategories2021" (
    "CategoryCode" character varying(4) NOT NULL,
    "Level" smallint NOT NULL,
    "Title" character varying(150)
);


ALTER TABLE public."NocCategories2021" OWNER TO workbc;

--
-- Name: NocCodes; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."NocCodes" (
    "Code" character varying(4),
    "Title" character varying(150),
    "Id" smallint NOT NULL,
    "FrenchTitle" character varying(180)
);


ALTER TABLE public."NocCodes" OWNER TO workbc;

--
-- Name: NocCodes2021; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."NocCodes2021" (
    "Id" integer NOT NULL,
    "Code" character varying(5),
    "Title" character varying(150),
    "FrenchTitle" character varying(250),
    "Code2016" character varying(30)
);


ALTER TABLE public."NocCodes2021" OWNER TO workbc;

--
-- Name: Provinces; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."Provinces" (
    "ProvinceId" integer NOT NULL,
    "Name" character varying(50),
    "ShortName" character varying(2)
);


ALTER TABLE public."Provinces" OWNER TO workbc;

--
-- Name: Regions; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."Regions" (
    "Id" integer NOT NULL,
    "Name" character varying(50),
    "ListOrder" smallint NOT NULL,
    "IsHidden" boolean NOT NULL
);


ALTER TABLE public."Regions" OWNER TO workbc;

--
-- Name: ReportPersistenceControl; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."ReportPersistenceControl" (
    "WeeklyPeriodId" integer NOT NULL,
    "DateCalculated" timestamp(6) without time zone NOT NULL,
    "IsTotalToDate" boolean NOT NULL,
    "TableName" character varying(25) DEFAULT ''::character varying NOT NULL
);


ALTER TABLE public."ReportPersistenceControl" OWNER TO workbc;

--
-- Name: SavedCareerProfiles; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."SavedCareerProfiles" (
    "Id" integer NOT NULL,
    "EDM_CareerProfile_CareerProfileId" integer,
    "AspNetUserId" character varying(450),
    "DateSaved" timestamp(6) without time zone NOT NULL,
    "DateDeleted" timestamp(6) without time zone,
    "IsDeleted" boolean NOT NULL,
    "NocCodeId2021" integer
);


ALTER TABLE public."SavedCareerProfiles" OWNER TO workbc;

--
-- Name: SavedCareerProfiles_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."SavedCareerProfiles" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."SavedCareerProfiles_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: SavedIndustryProfiles; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."SavedIndustryProfiles" (
    "Id" integer NOT NULL,
    "AspNetUserId" character varying(450),
    "DateSaved" timestamp(6) without time zone NOT NULL,
    "DateDeleted" timestamp(6) without time zone,
    "IsDeleted" boolean NOT NULL,
    "IndustryId" smallint
);


ALTER TABLE public."SavedIndustryProfiles" OWNER TO workbc;

--
-- Name: SavedIndustryProfiles_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."SavedIndustryProfiles" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."SavedIndustryProfiles_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: SavedJobs; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."SavedJobs" (
    "Id" integer NOT NULL,
    "JobId" bigint NOT NULL,
    "AspNetUserId" character varying(450),
    "DateSaved" timestamp(6) without time zone NOT NULL,
    "DateDeleted" timestamp(6) without time zone,
    "IsDeleted" boolean NOT NULL,
    "Note" character varying(800),
    "NoteUpdatedDate" timestamp(6) without time zone
);


ALTER TABLE public."SavedJobs" OWNER TO workbc;

--
-- Name: SavedJobs_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."SavedJobs" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."SavedJobs_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: SecurityQuestions; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."SecurityQuestions" (
    "Id" integer NOT NULL,
    "QuestionText" character varying(40)
);


ALTER TABLE public."SecurityQuestions" OWNER TO workbc;

--
-- Name: SystemSettings; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."SystemSettings" (
    "Name" character varying(400) NOT NULL,
    "Value" text,
    "Description" text,
    "FieldType" integer NOT NULL,
    "ModifiedByAdminUserId" integer NOT NULL,
    "DateUpdated" timestamp(6) without time zone NOT NULL,
    "DefaultValue" text
);


ALTER TABLE public."SystemSettings" OWNER TO workbc;

--
-- Name: WeeklyPeriods; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."WeeklyPeriods" (
    "Id" integer NOT NULL,
    "CalendarYear" smallint NOT NULL,
    "CalendarMonth" smallint NOT NULL,
    "FiscalYear" smallint NOT NULL,
    "WeekOfMonth" smallint NOT NULL,
    "WeekStartDate" timestamp(6) without time zone NOT NULL,
    "WeekEndDate" timestamp(6) without time zone NOT NULL,
    "IsEndOfFiscalYear" boolean NOT NULL,
    "IsEndOfMonth" boolean NOT NULL
);


ALTER TABLE public."WeeklyPeriods" OWNER TO workbc;

--
-- Name: WeeklyPeriods_Id_seq; Type: SEQUENCE; Schema: public; Owner: workbc
--

ALTER TABLE public."WeeklyPeriods" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."WeeklyPeriods_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: workbc
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO workbc;

--
-- Name: AdminUsers PK_AdminUsers_322100188; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AdminUsers"
    ADD CONSTRAINT "PK_AdminUsers_322100188" PRIMARY KEY ("Id");


--
-- Name: AspNetRoleClaims PK_AspNetRoleClaims_1813581499; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "PK_AspNetRoleClaims_1813581499" PRIMARY KEY ("Id");


--
-- Name: AspNetRoles PK_AspNetRoles_1749581271; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetRoles"
    ADD CONSTRAINT "PK_AspNetRoles_1749581271" PRIMARY KEY ("Id");


--
-- Name: AspNetUserClaims PK_AspNetUserClaims_1861581670; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "PK_AspNetUserClaims_1861581670" PRIMARY KEY ("Id");


--
-- Name: AspNetUserLogins PK_AspNetUserLogins_418100530; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "PK_AspNetUserLogins_418100530" PRIMARY KEY ("ProviderKey");


--
-- Name: AspNetUserRoles PK_AspNetUserRoles_1957582012; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "PK_AspNetUserRoles_1957582012" PRIMARY KEY ("UserId", "RoleId");


--
-- Name: AspNetUserTokens PK_AspNetUserTokens_402100473; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "PK_AspNetUserTokens_402100473" PRIMARY KEY ("Name");


--
-- Name: AspNetUsers PK_AspNetUsers_1781581385; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "PK_AspNetUsers_1781581385" PRIMARY KEY ("Id");


--
-- Name: Countries PK_Countries_398624463; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Countries"
    ADD CONSTRAINT "PK_Countries_398624463" PRIMARY KEY ("Id");


--
-- Name: DataProtectionKeys PK_DataProtectionKeys_2078630448; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."DataProtectionKeys"
    ADD CONSTRAINT "PK_DataProtectionKeys_2078630448" PRIMARY KEY ("Id");


--
-- Name: DeletedJobs PK_DeletedJobs_958626458; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."DeletedJobs"
    ADD CONSTRAINT "PK_DeletedJobs_958626458" PRIMARY KEY ("JobId");


--
-- Name: ExpiredJobs PK_ExpiredJobs_1710629137; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."ExpiredJobs"
    ADD CONSTRAINT "PK_ExpiredJobs_1710629137" PRIMARY KEY ("JobId");


--
-- Name: GeocodedLocationCache PK_GeocodedLocationCache_1669580986; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."GeocodedLocationCache"
    ADD CONSTRAINT "PK_GeocodedLocationCache_1669580986" PRIMARY KEY ("Id");


--
-- Name: ImpersonationLog PK_ImpersonationLog_1022626686; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."ImpersonationLog"
    ADD CONSTRAINT "PK_ImpersonationLog_1022626686" PRIMARY KEY ("Token");


--
-- Name: ImportedJobsFederal PK_ImportedJobsFederal_290100074; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."ImportedJobsFederal"
    ADD CONSTRAINT "PK_ImportedJobsFederal_290100074" PRIMARY KEY ("JobId");


--
-- Name: ImportedJobsWanted PK_ImportedJobsWanted_1509580416; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."ImportedJobsWanted"
    ADD CONSTRAINT "PK_ImportedJobsWanted_1509580416" PRIMARY KEY ("JobId");


--
-- Name: JobAlerts PK_JobAlerts_1106102981; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobAlerts"
    ADD CONSTRAINT "PK_JobAlerts_1106102981" PRIMARY KEY ("Id");


--
-- Name: JobIds PK_JobIds_1538104520; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobIds"
    ADD CONSTRAINT "PK_JobIds_1538104520" PRIMARY KEY ("Id");


--
-- Name: JobSeekerAdminComments PK_JobSeekerAdminComments_1874105717; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerAdminComments"
    ADD CONSTRAINT "PK_JobSeekerAdminComments_1874105717" PRIMARY KEY ("Id");


--
-- Name: JobSeekerChangeLog PK_JobSeekerChangeLog_1646628909; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerChangeLog"
    ADD CONSTRAINT "PK_JobSeekerChangeLog_1646628909" PRIMARY KEY ("Id");


--
-- Name: JobSeekerEventLog PK_JobSeekerEventLog_2002106173; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerEventLog"
    ADD CONSTRAINT "PK_JobSeekerEventLog_2002106173" PRIMARY KEY ("Id");


--
-- Name: JobSeekerFlags PK_JobSeekerFlags_98099390; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerFlags"
    ADD CONSTRAINT "PK_JobSeekerFlags_98099390" PRIMARY KEY ("Id");


--
-- Name: JobSeekerStatLabels PK_JobSeekerStatLabels_1438628168; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerStatLabels"
    ADD CONSTRAINT "PK_JobSeekerStatLabels_1438628168" PRIMARY KEY ("Key");


--
-- Name: JobSeekerStats PK_JobSeekerStats_1406628054; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerStats"
    ADD CONSTRAINT "PK_JobSeekerStats_1406628054" PRIMARY KEY ("WeeklyPeriodId", "LabelKey", "RegionId");


--
-- Name: JobSeekerVersions PK_JobSeekerVersions_286624064; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerVersions"
    ADD CONSTRAINT "PK_JobSeekerVersions_286624064" PRIMARY KEY ("Id");


--
-- Name: JobSources PK_JobSources_734625660; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSources"
    ADD CONSTRAINT "PK_JobSources_734625660" PRIMARY KEY ("Id");


--
-- Name: JobStats PK_JobStats_1342627826; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobStats"
    ADD CONSTRAINT "PK_JobStats_1342627826" PRIMARY KEY ("WeeklyPeriodId", "RegionId", "JobSourceId");


--
-- Name: JobVersions PK_JobVersions_46623209; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobVersions"
    ADD CONSTRAINT "PK_JobVersions_46623209" PRIMARY KEY ("Id");


--
-- Name: JobViews PK_JobViews_786101841; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobViews"
    ADD CONSTRAINT "PK_JobViews_786101841" PRIMARY KEY ("JobId");


--
-- Name: Jobs PK_Jobs_802101898; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Jobs"
    ADD CONSTRAINT "PK_Jobs_802101898" PRIMARY KEY ("JobId");


--
-- Name: Locations PK_LocationLookups_466100701; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Locations"
    ADD CONSTRAINT "PK_LocationLookups_466100701" PRIMARY KEY ("LocationId");


--
-- Name: Industries PK_NaicsCodes_2146106686; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Industries"
    ADD CONSTRAINT "PK_NaicsCodes_2146106686" PRIMARY KEY ("Id");


--
-- Name: NocCategories2021 PK_NocCategories2021_955150448; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."NocCategories2021"
    ADD CONSTRAINT "PK_NocCategories2021_955150448" PRIMARY KEY ("CategoryCode");


--
-- Name: NocCategories PK_NocCategories_2114106572; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."NocCategories"
    ADD CONSTRAINT "PK_NocCategories_2114106572" PRIMARY KEY ("CategoryCode");


--
-- Name: NocCodes2021 PK_NocCodes2021_859150106; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."NocCodes2021"
    ADD CONSTRAINT "PK_NocCodes2021_859150106" PRIMARY KEY ("Id");


--
-- Name: NocCodes PK_NocCodes_1634104862; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."NocCodes"
    ADD CONSTRAINT "PK_NocCodes_1634104862" PRIMARY KEY ("Id");


--
-- Name: Provinces PK_Provinces_142623551; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Provinces"
    ADD CONSTRAINT "PK_Provinces_142623551" PRIMARY KEY ("ProvinceId");


--
-- Name: Regions PK_Regions_174623665; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Regions"
    ADD CONSTRAINT "PK_Regions_174623665" PRIMARY KEY ("Id");


--
-- Name: ReportPersistenceControl PK_ReportPersistenceControl_1566628624; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."ReportPersistenceControl"
    ADD CONSTRAINT "PK_ReportPersistenceControl_1566628624" PRIMARY KEY ("WeeklyPeriodId", "TableName");


--
-- Name: SavedCareerProfiles PK_SavedCareerProfiles_1250103494; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."SavedCareerProfiles"
    ADD CONSTRAINT "PK_SavedCareerProfiles_1250103494" PRIMARY KEY ("Id");


--
-- Name: SavedIndustryProfiles PK_SavedIndustryProfiles_1282103608; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."SavedIndustryProfiles"
    ADD CONSTRAINT "PK_SavedIndustryProfiles_1282103608" PRIMARY KEY ("Id");


--
-- Name: SavedJobs PK_SavedJobs_498100815; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."SavedJobs"
    ADD CONSTRAINT "PK_SavedJobs_498100815" PRIMARY KEY ("Id");


--
-- Name: SecurityQuestions PK_SecurityQuestions_770101784; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."SecurityQuestions"
    ADD CONSTRAINT "PK_SecurityQuestions_770101784" PRIMARY KEY ("Id");


--
-- Name: SystemSettings PK_SystemSettings_1678629023; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."SystemSettings"
    ADD CONSTRAINT "PK_SystemSettings_1678629023" PRIMARY KEY ("Name");


--
-- Name: WeeklyPeriods PK_WeeklyPeriods_542624976; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."WeeklyPeriods"
    ADD CONSTRAINT "PK_WeeklyPeriods_542624976" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory_1221579390; Type: CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory_1221579390" PRIMARY KEY ("MigrationId");


--
-- Name: IX_AdminUsers_IX_AdminUsers_LockedByAdminUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AdminUsers_IX_AdminUsers_LockedByAdminUserId" ON public."AdminUsers" USING btree ("LockedByAdminUserId");


--
-- Name: IX_AdminUsers_IX_AdminUsers_ModifiedByAdminUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AdminUsers_IX_AdminUsers_ModifiedByAdminUserId" ON public."AdminUsers" USING btree ("ModifiedByAdminUserId");


--
-- Name: IX_AspNetRoleClaims_IX_AspNetRoleClaims_RoleId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetRoleClaims_IX_AspNetRoleClaims_RoleId" ON public."AspNetRoleClaims" USING btree ("RoleId");


--
-- Name: IX_AspNetRoles_RoleNameIndex; Type: INDEX; Schema: public; Owner: workbc
--

CREATE UNIQUE INDEX "IX_AspNetRoles_RoleNameIndex" ON public."AspNetRoles" USING btree ("NormalizedName") WHERE ("NormalizedName" IS NOT NULL);


--
-- Name: IX_AspNetUserClaims_IX_AspNetUserClaims_UserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUserClaims_IX_AspNetUserClaims_UserId" ON public."AspNetUserClaims" USING btree ("UserId");


--
-- Name: IX_AspNetUserLogins_IX_AspNetUserLogins_UserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUserLogins_IX_AspNetUserLogins_UserId" ON public."AspNetUserLogins" USING btree ("UserId");


--
-- Name: IX_AspNetUserRoles_IX_AspNetUserRoles_RoleId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUserRoles_IX_AspNetUserRoles_RoleId" ON public."AspNetUserRoles" USING btree ("RoleId");


--
-- Name: IX_AspNetUsers_EmailIndex; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUsers_EmailIndex" ON public."AspNetUsers" USING btree ("NormalizedEmail");


--
-- Name: IX_AspNetUsers_IX_AspNetUsers_AccountStatus_LastName_FirstName; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_AccountStatus_LastName_FirstName" ON public."AspNetUsers" USING btree ("AccountStatus", "LastName", "FirstName") INCLUDE ("Email");


--
-- Name: IX_AspNetUsers_IX_AspNetUsers_CountryId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_CountryId" ON public."AspNetUsers" USING btree ("CountryId");


--
-- Name: IX_AspNetUsers_IX_AspNetUsers_DateRegistered; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_DateRegistered" ON public."AspNetUsers" USING btree ("DateRegistered") INCLUDE ("LastName", "FirstName", "Email", "AccountStatus");


--
-- Name: IX_AspNetUsers_IX_AspNetUsers_Email; Type: INDEX; Schema: public; Owner: workbc
--

CREATE UNIQUE INDEX "IX_AspNetUsers_IX_AspNetUsers_Email" ON public."AspNetUsers" USING btree ("Email") INCLUDE ("LastName", "FirstName", "AccountStatus") WHERE ("Email" IS NOT NULL);


--
-- Name: IX_AspNetUsers_IX_AspNetUsers_FirstName_LastName; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_FirstName_LastName" ON public."AspNetUsers" USING btree ("FirstName", "LastName") INCLUDE ("Email", "AccountStatus");


--
-- Name: IX_AspNetUsers_IX_AspNetUsers_LastModified; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_LastModified" ON public."AspNetUsers" USING btree ("LastModified") INCLUDE ("LastName", "FirstName", "Email", "AccountStatus");


--
-- Name: IX_AspNetUsers_IX_AspNetUsers_LastName_FirstName; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_LastName_FirstName" ON public."AspNetUsers" USING btree ("LastName", "FirstName") INCLUDE ("Email", "AccountStatus");


--
-- Name: IX_AspNetUsers_IX_AspNetUsers_LocationId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_LocationId" ON public."AspNetUsers" USING btree ("LocationId");


--
-- Name: IX_AspNetUsers_IX_AspNetUsers_LockedByAdminUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_LockedByAdminUserId" ON public."AspNetUsers" USING btree ("LockedByAdminUserId");


--
-- Name: IX_AspNetUsers_IX_AspNetUsers_ProvinceId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_ProvinceId" ON public."AspNetUsers" USING btree ("ProvinceId");


--
-- Name: IX_AspNetUsers_IX_AspNetUsers_SecurityQuestionId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_AspNetUsers_IX_AspNetUsers_SecurityQuestionId" ON public."AspNetUsers" USING btree ("SecurityQuestionId");


--
-- Name: IX_AspNetUsers_UserNameIndex; Type: INDEX; Schema: public; Owner: workbc
--

CREATE UNIQUE INDEX "IX_AspNetUsers_UserNameIndex" ON public."AspNetUsers" USING btree ("NormalizedUserName") WHERE ("NormalizedUserName" IS NOT NULL);


--
-- Name: IX_DeletedJobs_IX_DeletedJobs_DeletedByAdminUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_DeletedJobs_IX_DeletedJobs_DeletedByAdminUserId" ON public."DeletedJobs" USING btree ("DeletedByAdminUserId");


--
-- Name: IX_GeocodedLocationCache_IX_GeocodedLocationCache_Name; Type: INDEX; Schema: public; Owner: workbc
--

CREATE UNIQUE INDEX "IX_GeocodedLocationCache_IX_GeocodedLocationCache_Name" ON public."GeocodedLocationCache" USING btree ("Name") WHERE ("Name" IS NOT NULL);


--
-- Name: IX_ImpersonationLog_IX_ImpersonationLog_AdminUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_ImpersonationLog_IX_ImpersonationLog_AdminUserId" ON public."ImpersonationLog" USING btree ("AdminUserId");


--
-- Name: IX_ImpersonationLog_IX_ImpersonationLog_AspNetUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_ImpersonationLog_IX_ImpersonationLog_AspNetUserId" ON public."ImpersonationLog" USING btree ("AspNetUserId");


--
-- Name: IX_ImportedJobsWanted_IX_ImportedJobsWanted_HashId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE UNIQUE INDEX "IX_ImportedJobsWanted_IX_ImportedJobsWanted_HashId" ON public."ImportedJobsWanted" USING btree ("HashId");


--
-- Name: IX_JobAlerts_IX_JobAlerts_AspNetUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobAlerts_IX_JobAlerts_AspNetUserId" ON public."JobAlerts" USING btree ("AspNetUserId");


--
-- Name: IX_JobAlerts_IX_JobAlerts_DateCreated; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobAlerts_IX_JobAlerts_DateCreated" ON public."JobAlerts" USING btree ("DateCreated");


--
-- Name: IX_JobIds_IX_JobIds_JobSourceId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobIds_IX_JobIds_JobSourceId" ON public."JobIds" USING btree ("JobSourceId");


--
-- Name: IX_JobSeekerAdminComments_IX_JobSeekerAdminComments_AspNetUserI; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobSeekerAdminComments_IX_JobSeekerAdminComments_AspNetUserI" ON public."JobSeekerAdminComments" USING btree ("AspNetUserId");


--
-- Name: IX_JobSeekerAdminComments_IX_JobSeekerAdminComments_EnteredByAd; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobSeekerAdminComments_IX_JobSeekerAdminComments_EnteredByAd" ON public."JobSeekerAdminComments" USING btree ("EnteredByAdminUserId");


--
-- Name: IX_JobSeekerChangeLog_IX_JobSeekerChangeLog_AspNetUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobSeekerChangeLog_IX_JobSeekerChangeLog_AspNetUserId" ON public."JobSeekerChangeLog" USING btree ("AspNetUserId");


--
-- Name: IX_JobSeekerChangeLog_IX_JobSeekerChangeLog_ModifiedByAdminUser; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobSeekerChangeLog_IX_JobSeekerChangeLog_ModifiedByAdminUser" ON public."JobSeekerChangeLog" USING btree ("ModifiedByAdminUserId");


--
-- Name: IX_JobSeekerEventLog_IX_JobSeekerEventLog_AspNetUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobSeekerEventLog_IX_JobSeekerEventLog_AspNetUserId" ON public."JobSeekerEventLog" USING btree ("AspNetUserId");


--
-- Name: IX_JobSeekerEventLog_IX_JobSeekerEventLog_DateLogged; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobSeekerEventLog_IX_JobSeekerEventLog_DateLogged" ON public."JobSeekerEventLog" USING btree ("DateLogged");


--
-- Name: IX_JobSeekerFlags_IX_JobSeekerFlags_AspNetUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE UNIQUE INDEX "IX_JobSeekerFlags_IX_JobSeekerFlags_AspNetUserId" ON public."JobSeekerFlags" USING btree ("AspNetUserId") WHERE ("AspNetUserId" IS NOT NULL);


--
-- Name: IX_JobSeekerStats_IX_JobSeekerStats_LabelKey; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobSeekerStats_IX_JobSeekerStats_LabelKey" ON public."JobSeekerStats" USING btree ("LabelKey");


--
-- Name: IX_JobSeekerStats_IX_JobSeekerStats_RegionId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobSeekerStats_IX_JobSeekerStats_RegionId" ON public."JobSeekerStats" USING btree ("RegionId");


--
-- Name: IX_JobSeekerVersions_IX_JobSeekerVersions_AspNetUserId_VersionN; Type: INDEX; Schema: public; Owner: workbc
--

CREATE UNIQUE INDEX "IX_JobSeekerVersions_IX_JobSeekerVersions_AspNetUserId_VersionN" ON public."JobSeekerVersions" USING btree ("AspNetUserId", "VersionNumber") WHERE ("AspNetUserId" IS NOT NULL);


--
-- Name: IX_JobSeekerVersions_IX_JobSeekerVersions_CountryId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobSeekerVersions_IX_JobSeekerVersions_CountryId" ON public."JobSeekerVersions" USING btree ("CountryId");


--
-- Name: IX_JobSeekerVersions_IX_JobSeekerVersions_LocationId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobSeekerVersions_IX_JobSeekerVersions_LocationId" ON public."JobSeekerVersions" USING btree ("LocationId");


--
-- Name: IX_JobSeekerVersions_IX_JobSeekerVersions_ProvinceId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobSeekerVersions_IX_JobSeekerVersions_ProvinceId" ON public."JobSeekerVersions" USING btree ("ProvinceId");


--
-- Name: IX_JobStats_IX_JobStats_JobSourceId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobStats_IX_JobStats_JobSourceId" ON public."JobStats" USING btree ("JobSourceId");


--
-- Name: IX_JobStats_IX_JobStats_RegionId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobStats_IX_JobStats_RegionId" ON public."JobStats" USING btree ("RegionId");


--
-- Name: IX_JobVersions_IX_JobVersions_JobId_VersionNumber; Type: INDEX; Schema: public; Owner: workbc
--

CREATE UNIQUE INDEX "IX_JobVersions_IX_JobVersions_JobId_VersionNumber" ON public."JobVersions" USING btree ("JobId", "VersionNumber");


--
-- Name: IX_JobVersions_IX_JobVersions_LocationId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobVersions_IX_JobVersions_LocationId" ON public."JobVersions" USING btree ("LocationId");


--
-- Name: IX_JobVersions_IX_JobVersions_NaicsId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobVersions_IX_JobVersions_NaicsId" ON public."JobVersions" USING btree ("IndustryId");


--
-- Name: IX_JobVersions_IX_JobVersions_NocCodeId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_JobVersions_IX_JobVersions_NocCodeId" ON public."JobVersions" USING btree ("NocCodeId");


--
-- Name: IX_Jobs_IX_Jobs_JobSourceId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_Jobs_IX_Jobs_JobSourceId" ON public."Jobs" USING btree ("JobSourceId");


--
-- Name: IX_Jobs_IX_Jobs_LocationId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_Jobs_IX_Jobs_LocationId" ON public."Jobs" USING btree ("LocationId");


--
-- Name: IX_Jobs_IX_Jobs_NaicsId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_Jobs_IX_Jobs_NaicsId" ON public."Jobs" USING btree ("IndustryId");


--
-- Name: IX_Jobs_IX_Jobs_NocCodeId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_Jobs_IX_Jobs_NocCodeId" ON public."Jobs" USING btree ("NocCodeId");


--
-- Name: IX_Locations_IX_LocationLookups_RegionId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_Locations_IX_LocationLookups_RegionId" ON public."Locations" USING btree ("RegionId");


--
-- Name: IX_SavedCareerProfiles_IX_SavedCareerProfiles_AspNetUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_SavedCareerProfiles_IX_SavedCareerProfiles_AspNetUserId" ON public."SavedCareerProfiles" USING btree ("AspNetUserId");


--
-- Name: IX_SavedCareerProfiles_IX_SavedCareerProfiles_DateDeleted; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_SavedCareerProfiles_IX_SavedCareerProfiles_DateDeleted" ON public."SavedCareerProfiles" USING btree ("DateDeleted");


--
-- Name: IX_SavedCareerProfiles_IX_SavedCareerProfiles_DateSaved; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_SavedCareerProfiles_IX_SavedCareerProfiles_DateSaved" ON public."SavedCareerProfiles" USING btree ("DateSaved");


--
-- Name: IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_AspNetUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_AspNetUserId" ON public."SavedIndustryProfiles" USING btree ("AspNetUserId");


--
-- Name: IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_DateDeleted; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_DateDeleted" ON public."SavedIndustryProfiles" USING btree ("DateDeleted");


--
-- Name: IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_DateSaved; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_SavedIndustryProfiles_IX_SavedIndustryProfiles_DateSaved" ON public."SavedIndustryProfiles" USING btree ("DateSaved");


--
-- Name: IX_SavedJobs_IX_SavedJobs_AspNetUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_SavedJobs_IX_SavedJobs_AspNetUserId" ON public."SavedJobs" USING btree ("AspNetUserId");


--
-- Name: IX_SavedJobs_IX_SavedJobs_JobId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_SavedJobs_IX_SavedJobs_JobId" ON public."SavedJobs" USING btree ("JobId");


--
-- Name: IX_SystemSettings_IX_SystemSettings_ModifiedByAdminUserId; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_SystemSettings_IX_SystemSettings_ModifiedByAdminUserId" ON public."SystemSettings" USING btree ("ModifiedByAdminUserId");


--
-- Name: IX_WeeklyPeriods_IX_WeeklyPeriods_WeekEndDate; Type: INDEX; Schema: public; Owner: workbc
--

CREATE INDEX "IX_WeeklyPeriods_IX_WeeklyPeriods_WeekEndDate" ON public."WeeklyPeriods" USING btree ("WeekEndDate");


--
-- Name: AdminUsers FK_AdminUsers_AdminUsers_LockedByAdminUserId_1346103836; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AdminUsers"
    ADD CONSTRAINT "FK_AdminUsers_AdminUsers_LockedByAdminUserId_1346103836" FOREIGN KEY ("LockedByAdminUserId") REFERENCES public."AdminUsers"("Id");


--
-- Name: AdminUsers FK_AdminUsers_AdminUsers_ModifiedByAdminUserId_1378103950; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AdminUsers"
    ADD CONSTRAINT "FK_AdminUsers_AdminUsers_ModifiedByAdminUserId_1378103950" FOREIGN KEY ("ModifiedByAdminUserId") REFERENCES public."AdminUsers"("Id");


--
-- Name: AspNetRoleClaims FK_AspNetRoleClaims_AspNetRoles_RoleId_1829581556; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId_1829581556" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserClaims FK_AspNetUserClaims_AspNetUsers_UserId_1877581727; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId_1877581727" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserLogins FK_AspNetUserLogins_AspNetUsers_UserId_1925581898; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId_1925581898" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetRoles_RoleId_1973582069; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId_1973582069" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetUsers_UserId_1989582126; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId_1989582126" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserTokens FK_AspNetUserTokens_AspNetUsers_UserId_2037582297; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId_2037582297" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUsers FK_AspNetUsers_AdminUsers_LockedByAdminUserId_2050106344; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "FK_AspNetUsers_AdminUsers_LockedByAdminUserId_2050106344" FOREIGN KEY ("LockedByAdminUserId") REFERENCES public."AdminUsers"("Id");


--
-- Name: AspNetUsers FK_AspNetUsers_Countries_CountryId_414624520; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "FK_AspNetUsers_Countries_CountryId_414624520" FOREIGN KEY ("CountryId") REFERENCES public."Countries"("Id");


--
-- Name: AspNetUsers FK_AspNetUsers_LocationLookups_LocationId_190623722; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "FK_AspNetUsers_LocationLookups_LocationId_190623722" FOREIGN KEY ("LocationId") REFERENCES public."Locations"("LocationId");


--
-- Name: AspNetUsers FK_AspNetUsers_Provinces_ProvinceId_206623779; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "FK_AspNetUsers_Provinces_ProvinceId_206623779" FOREIGN KEY ("ProvinceId") REFERENCES public."Provinces"("ProvinceId");


--
-- Name: AspNetUsers FK_AspNetUsers_SecurityQuestions_SecurityQuestionId_850102069; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "FK_AspNetUsers_SecurityQuestions_SecurityQuestionId_850102069" FOREIGN KEY ("SecurityQuestionId") REFERENCES public."SecurityQuestions"("Id");


--
-- Name: DeletedJobs FK_DeletedJobs_AdminUsers_DeletedByAdminUserId_974626515; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."DeletedJobs"
    ADD CONSTRAINT "FK_DeletedJobs_AdminUsers_DeletedByAdminUserId_974626515" FOREIGN KEY ("DeletedByAdminUserId") REFERENCES public."AdminUsers"("Id") ON DELETE CASCADE;


--
-- Name: DeletedJobs FK_DeletedJobs_Jobs_JobId_990626572; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."DeletedJobs"
    ADD CONSTRAINT "FK_DeletedJobs_Jobs_JobId_990626572" FOREIGN KEY ("JobId") REFERENCES public."Jobs"("JobId") ON DELETE CASCADE;


--
-- Name: ExpiredJobs FK_ExpiredJobs_JobIds_JobId_1726629194; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."ExpiredJobs"
    ADD CONSTRAINT "FK_ExpiredJobs_JobIds_JobId_1726629194" FOREIGN KEY ("JobId") REFERENCES public."JobIds"("Id") ON DELETE CASCADE;


--
-- Name: ImpersonationLog FK_ImpersonationLog_AdminUsers_AdminUserId_1038626743; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."ImpersonationLog"
    ADD CONSTRAINT "FK_ImpersonationLog_AdminUsers_AdminUserId_1038626743" FOREIGN KEY ("AdminUserId") REFERENCES public."AdminUsers"("Id") ON DELETE CASCADE;


--
-- Name: ImpersonationLog FK_ImpersonationLog_AspNetUsers_AspNetUserId_1054626800; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."ImpersonationLog"
    ADD CONSTRAINT "FK_ImpersonationLog_AspNetUsers_AspNetUserId_1054626800" FOREIGN KEY ("AspNetUserId") REFERENCES public."AspNetUsers"("Id");


--
-- Name: ImportedJobsFederal FK_ImportedJobsFederal_JobIds_JobId_1570104634; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."ImportedJobsFederal"
    ADD CONSTRAINT "FK_ImportedJobsFederal_JobIds_JobId_1570104634" FOREIGN KEY ("JobId") REFERENCES public."JobIds"("Id") ON DELETE CASCADE;


--
-- Name: ImportedJobsWanted FK_ImportedJobsWanted_JobIds_JobId_1586104691; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."ImportedJobsWanted"
    ADD CONSTRAINT "FK_ImportedJobsWanted_JobIds_JobId_1586104691" FOREIGN KEY ("JobId") REFERENCES public."JobIds"("Id") ON DELETE CASCADE;


--
-- Name: JobAlerts FK_JobAlerts_AspNetUsers_AspNetUserId_1122103038; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobAlerts"
    ADD CONSTRAINT "FK_JobAlerts_AspNetUsers_AspNetUserId_1122103038" FOREIGN KEY ("AspNetUserId") REFERENCES public."AspNetUsers"("Id");


--
-- Name: JobIds FK_JobIds_JobSources_JobSourceId_766625774; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobIds"
    ADD CONSTRAINT "FK_JobIds_JobSources_JobSourceId_766625774" FOREIGN KEY ("JobSourceId") REFERENCES public."JobSources"("Id");


--
-- Name: JobSeekerAdminComments FK_JobSeekerAdminComments_AdminUsers_EnteredByAdminUserId_19061; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerAdminComments"
    ADD CONSTRAINT "FK_JobSeekerAdminComments_AdminUsers_EnteredByAdminUserId_19061" FOREIGN KEY ("EnteredByAdminUserId") REFERENCES public."AdminUsers"("Id") ON DELETE CASCADE;


--
-- Name: JobSeekerAdminComments FK_JobSeekerAdminComments_AspNetUsers_AspNetUserId_1890105774; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerAdminComments"
    ADD CONSTRAINT "FK_JobSeekerAdminComments_AspNetUsers_AspNetUserId_1890105774" FOREIGN KEY ("AspNetUserId") REFERENCES public."AspNetUsers"("Id");


--
-- Name: JobSeekerChangeLog FK_JobSeekerAdminLog_AspNetUsers_AspNetUserId_1662628966; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerChangeLog"
    ADD CONSTRAINT "FK_JobSeekerAdminLog_AspNetUsers_AspNetUserId_1662628966" FOREIGN KEY ("AspNetUserId") REFERENCES public."AspNetUsers"("Id");


--
-- Name: JobSeekerChangeLog FK_JobSeekerChangeLog_AdminUsers_ModifiedByAdminUserId_16306288; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerChangeLog"
    ADD CONSTRAINT "FK_JobSeekerChangeLog_AdminUsers_ModifiedByAdminUserId_16306288" FOREIGN KEY ("ModifiedByAdminUserId") REFERENCES public."AdminUsers"("Id");


--
-- Name: JobSeekerEventLog FK_JobSeekerEventLog_AspNetUsers_AspNetUserId_2018106230; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerEventLog"
    ADD CONSTRAINT "FK_JobSeekerEventLog_AspNetUsers_AspNetUserId_2018106230" FOREIGN KEY ("AspNetUserId") REFERENCES public."AspNetUsers"("Id");


--
-- Name: JobSeekerFlags FK_JobSeekerFlags_AspNetUsers_AspNetUserId_178099675; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerFlags"
    ADD CONSTRAINT "FK_JobSeekerFlags_AspNetUsers_AspNetUserId_178099675" FOREIGN KEY ("AspNetUserId") REFERENCES public."AspNetUsers"("Id");


--
-- Name: JobSeekerStats FK_JobSeekerStats_JobSeekerStatLabels_LabelKey_1454628225; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerStats"
    ADD CONSTRAINT "FK_JobSeekerStats_JobSeekerStatLabels_LabelKey_1454628225" FOREIGN KEY ("LabelKey") REFERENCES public."JobSeekerStatLabels"("Key") ON DELETE CASCADE;


--
-- Name: JobSeekerStats FK_JobSeekerStats_Regions_RegionId_1502628396; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerStats"
    ADD CONSTRAINT "FK_JobSeekerStats_Regions_RegionId_1502628396" FOREIGN KEY ("RegionId") REFERENCES public."Regions"("Id") ON DELETE CASCADE;


--
-- Name: JobSeekerStats FK_JobSeekerStats_WeeklyPeriods_WeeklyPeriodId_1262627541; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerStats"
    ADD CONSTRAINT "FK_JobSeekerStats_WeeklyPeriods_WeeklyPeriodId_1262627541" FOREIGN KEY ("WeeklyPeriodId") REFERENCES public."WeeklyPeriods"("Id") ON DELETE CASCADE;


--
-- Name: JobSeekerVersions FK_JobSeekerVersions_AspNetUsers_AspNetUserId_302624121; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerVersions"
    ADD CONSTRAINT "FK_JobSeekerVersions_AspNetUsers_AspNetUserId_302624121" FOREIGN KEY ("AspNetUserId") REFERENCES public."AspNetUsers"("Id");


--
-- Name: JobSeekerVersions FK_JobSeekerVersions_Countries_CountryId_430624577; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerVersions"
    ADD CONSTRAINT "FK_JobSeekerVersions_Countries_CountryId_430624577" FOREIGN KEY ("CountryId") REFERENCES public."Countries"("Id");


--
-- Name: JobSeekerVersions FK_JobSeekerVersions_Locations_LocationId_366624349; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerVersions"
    ADD CONSTRAINT "FK_JobSeekerVersions_Locations_LocationId_366624349" FOREIGN KEY ("LocationId") REFERENCES public."Locations"("LocationId");


--
-- Name: JobSeekerVersions FK_JobSeekerVersions_Provinces_ProvinceId_350624292; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobSeekerVersions"
    ADD CONSTRAINT "FK_JobSeekerVersions_Provinces_ProvinceId_350624292" FOREIGN KEY ("ProvinceId") REFERENCES public."Provinces"("ProvinceId");


--
-- Name: JobStats FK_JobStats_JobSources_JobSourceId_1358627883; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobStats"
    ADD CONSTRAINT "FK_JobStats_JobSources_JobSourceId_1358627883" FOREIGN KEY ("JobSourceId") REFERENCES public."JobSources"("Id") ON DELETE CASCADE;


--
-- Name: JobStats FK_JobStats_Regions_RegionId_1518628453; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobStats"
    ADD CONSTRAINT "FK_JobStats_Regions_RegionId_1518628453" FOREIGN KEY ("RegionId") REFERENCES public."Regions"("Id") ON DELETE CASCADE;


--
-- Name: JobStats FK_JobStats_WeeklyPeriods_WeeklyPeriodId_1374627940; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobStats"
    ADD CONSTRAINT "FK_JobStats_WeeklyPeriods_WeeklyPeriodId_1374627940" FOREIGN KEY ("WeeklyPeriodId") REFERENCES public."WeeklyPeriods"("Id") ON DELETE CASCADE;


--
-- Name: JobVersions FK_JobVersions_Jobs_JobId_62623266; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobVersions"
    ADD CONSTRAINT "FK_JobVersions_Jobs_JobId_62623266" FOREIGN KEY ("JobId") REFERENCES public."Jobs"("JobId");


--
-- Name: JobVersions FK_JobVersions_LocationLookups_LocationId_238623893; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobVersions"
    ADD CONSTRAINT "FK_JobVersions_LocationLookups_LocationId_238623893" FOREIGN KEY ("LocationId") REFERENCES public."Locations"("LocationId") ON DELETE CASCADE;


--
-- Name: JobVersions FK_JobVersions_NaicsCodes_NaicsId_94623380; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobVersions"
    ADD CONSTRAINT "FK_JobVersions_NaicsCodes_NaicsId_94623380" FOREIGN KEY ("IndustryId") REFERENCES public."Industries"("Id");


--
-- Name: JobVersions FK_JobVersions_NocCodes2021_NocCodeId2021_891150220; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobVersions"
    ADD CONSTRAINT "FK_JobVersions_NocCodes2021_NocCodeId2021_891150220" FOREIGN KEY ("NocCodeId2021") REFERENCES public."NocCodes2021"("Id");


--
-- Name: JobVersions FK_JobVersions_NocCodes_NocCodeId_110623437; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobVersions"
    ADD CONSTRAINT "FK_JobVersions_NocCodes_NocCodeId_110623437" FOREIGN KEY ("NocCodeId") REFERENCES public."NocCodes"("Id");


--
-- Name: JobViews FK_JobViews_Jobs_JobId_1442104178; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."JobViews"
    ADD CONSTRAINT "FK_JobViews_Jobs_JobId_1442104178" FOREIGN KEY ("JobId") REFERENCES public."Jobs"("JobId") ON DELETE CASCADE;


--
-- Name: Jobs FK_Jobs_JobIds_JobId_1602104748; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Jobs"
    ADD CONSTRAINT "FK_Jobs_JobIds_JobId_1602104748" FOREIGN KEY ("JobId") REFERENCES public."JobIds"("Id") ON DELETE CASCADE;


--
-- Name: Jobs FK_Jobs_JobSources_JobSourceId_782625831; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Jobs"
    ADD CONSTRAINT "FK_Jobs_JobSources_JobSourceId_782625831" FOREIGN KEY ("JobSourceId") REFERENCES public."JobSources"("Id");


--
-- Name: Jobs FK_Jobs_LocationLookups_LocationId_222623836; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Jobs"
    ADD CONSTRAINT "FK_Jobs_LocationLookups_LocationId_222623836" FOREIGN KEY ("LocationId") REFERENCES public."Locations"("LocationId") ON DELETE CASCADE;


--
-- Name: Jobs FK_Jobs_NaicsCodes_NaicsId_14623095; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Jobs"
    ADD CONSTRAINT "FK_Jobs_NaicsCodes_NaicsId_14623095" FOREIGN KEY ("IndustryId") REFERENCES public."Industries"("Id");


--
-- Name: Jobs FK_Jobs_NocCodes2021_NocCodeId2021_875150163; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Jobs"
    ADD CONSTRAINT "FK_Jobs_NocCodes2021_NocCodeId2021_875150163" FOREIGN KEY ("NocCodeId2021") REFERENCES public."NocCodes2021"("Id");


--
-- Name: Jobs FK_Jobs_NocCodes_NocCodeId_1650104919; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Jobs"
    ADD CONSTRAINT "FK_Jobs_NocCodes_NocCodeId_1650104919" FOREIGN KEY ("NocCodeId") REFERENCES public."NocCodes"("Id");


--
-- Name: Locations FK_LocationLookups_Regions_RegionId_254623950; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Locations"
    ADD CONSTRAINT "FK_LocationLookups_Regions_RegionId_254623950" FOREIGN KEY ("RegionId") REFERENCES public."Regions"("Id") ON DELETE CASCADE;


--
-- Name: Locations FK_Locations_Regions_RegionId_446624634; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."Locations"
    ADD CONSTRAINT "FK_Locations_Regions_RegionId_446624634" FOREIGN KEY ("RegionId") REFERENCES public."Regions"("Id");


--
-- Name: ReportPersistenceControl FK_ReportPersistenceControl_WeeklyPeriods_WeeklyPeriodId_115062; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."ReportPersistenceControl"
    ADD CONSTRAINT "FK_ReportPersistenceControl_WeeklyPeriods_WeeklyPeriodId_115062" FOREIGN KEY ("WeeklyPeriodId") REFERENCES public."WeeklyPeriods"("Id") ON DELETE CASCADE;


--
-- Name: SavedCareerProfiles FK_SavedCareerProfiles_AspNetUsers_AspNetUserId_1458104235; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."SavedCareerProfiles"
    ADD CONSTRAINT "FK_SavedCareerProfiles_AspNetUsers_AspNetUserId_1458104235" FOREIGN KEY ("AspNetUserId") REFERENCES public."AspNetUsers"("Id");


--
-- Name: SavedCareerProfiles FK_SavedCareerProfiles_NocCodes2021_Id_907150277; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."SavedCareerProfiles"
    ADD CONSTRAINT "FK_SavedCareerProfiles_NocCodes2021_Id_907150277" FOREIGN KEY ("NocCodeId2021") REFERENCES public."NocCodes2021"("Id") ON DELETE CASCADE;


--
-- Name: SavedIndustryProfiles FK_SavedIndustryProfiles_AspNetUsers_AspNetUserId_1474104292; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."SavedIndustryProfiles"
    ADD CONSTRAINT "FK_SavedIndustryProfiles_AspNetUsers_AspNetUserId_1474104292" FOREIGN KEY ("AspNetUserId") REFERENCES public."AspNetUsers"("Id");


--
-- Name: SavedIndustryProfiles FK_SavedIndustryProfiles_Industries_Id_923150334; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."SavedIndustryProfiles"
    ADD CONSTRAINT "FK_SavedIndustryProfiles_Industries_Id_923150334" FOREIGN KEY ("IndustryId") REFERENCES public."Industries"("Id") ON DELETE CASCADE;


--
-- Name: SavedJobs FK_SavedJobs_AspNetUsers_AspNetUserId_514100872; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."SavedJobs"
    ADD CONSTRAINT "FK_SavedJobs_AspNetUsers_AspNetUserId_514100872" FOREIGN KEY ("AspNetUserId") REFERENCES public."AspNetUsers"("Id");


--
-- Name: SavedJobs FK_SavedJobs_Jobs_JobId_1362103893; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."SavedJobs"
    ADD CONSTRAINT "FK_SavedJobs_Jobs_JobId_1362103893" FOREIGN KEY ("JobId") REFERENCES public."Jobs"("JobId") ON DELETE CASCADE;


--
-- Name: SystemSettings FK_SystemSettings_AdminUsers_ModifiedByAdminUserId_1490104349; Type: FK CONSTRAINT; Schema: public; Owner: workbc
--

ALTER TABLE ONLY public."SystemSettings"
    ADD CONSTRAINT "FK_SystemSettings_AdminUsers_ModifiedByAdminUserId_1490104349" FOREIGN KEY ("ModifiedByAdminUserId") REFERENCES public."AdminUsers"("Id") ON DELETE CASCADE;


--
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: workbc
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;


--
-- PostgreSQL database dump complete
--

