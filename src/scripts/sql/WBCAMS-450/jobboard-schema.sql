-- ------------ Write DROP-FUNCTION-stage scripts -----------

DROP ROUTINE IF EXISTS public.tvf_getjobseekersfordate(IN TIMESTAMP WITHOUT TIME ZONE, OUT VARCHAR, OUT VARCHAR, OUT INTEGER, OUT INTEGER, OUT INTEGER, OUT TIMESTAMP WITHOUT TIME ZONE, OUT SMALLINT, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC, OUT NUMERIC);

DROP ROUTINE IF EXISTS public.tvf_getjobsfordate(IN TIMESTAMP WITHOUT TIME ZONE, OUT BIGINT, OUT SMALLINT, OUT INTEGER, OUT INTEGER, OUT SMALLINT, OUT TIMESTAMP WITHOUT TIME ZONE, OUT SMALLINT);

-- ------------ Write DROP-PROCEDURE-stage scripts -----------

DROP ROUTINE IF EXISTS public.usp_generatejobseekerstats(IN TIMESTAMP WITHOUT TIME ZONE, INOUT int);

DROP ROUTINE IF EXISTS public.usp_generatejobstats(IN TIMESTAMP WITHOUT TIME ZONE, INOUT int);

-- ------------ Write DROP-FOREIGN-KEY-CONSTRAINT-stage scripts -----------

ALTER TABLE public.adminusers DROP CONSTRAINT fk_adminusers_adminusers_lockedbyadminuserid_1346103836;

ALTER TABLE public.adminusers DROP CONSTRAINT fk_adminusers_adminusers_modifiedbyadminuserid_1378103950;

ALTER TABLE public.aspnetroleclaims DROP CONSTRAINT fk_aspnetroleclaims_aspnetroles_roleid_1829581556;

ALTER TABLE public.aspnetuserclaims DROP CONSTRAINT fk_aspnetuserclaims_aspnetusers_userid_1877581727;

ALTER TABLE public.aspnetuserlogins DROP CONSTRAINT fk_aspnetuserlogins_aspnetusers_userid_1925581898;

ALTER TABLE public.aspnetuserroles DROP CONSTRAINT fk_aspnetuserroles_aspnetroles_roleid_1973582069;

ALTER TABLE public.aspnetuserroles DROP CONSTRAINT fk_aspnetuserroles_aspnetusers_userid_1989582126;

ALTER TABLE public.aspnetusers DROP CONSTRAINT fk_aspnetusers_adminusers_lockedbyadminuserid_2050106344;

ALTER TABLE public.aspnetusers DROP CONSTRAINT fk_aspnetusers_countries_countryid_414624520;

ALTER TABLE public.aspnetusers DROP CONSTRAINT fk_aspnetusers_locationlookups_locationid_190623722;

ALTER TABLE public.aspnetusers DROP CONSTRAINT fk_aspnetusers_provinces_provinceid_206623779;

ALTER TABLE public.aspnetusers DROP CONSTRAINT fk_aspnetusers_securityquestions_securityquestionid_850102069;

ALTER TABLE public.aspnetusertokens DROP CONSTRAINT fk_aspnetusertokens_aspnetusers_userid_2037582297;

ALTER TABLE public.deletedjobs DROP CONSTRAINT fk_deletedjobs_adminusers_deletedbyadminuserid_974626515;

ALTER TABLE public.deletedjobs DROP CONSTRAINT fk_deletedjobs_jobs_jobid_990626572;

ALTER TABLE public.expiredjobs DROP CONSTRAINT fk_expiredjobs_jobids_jobid_1726629194;

ALTER TABLE public.impersonationlog DROP CONSTRAINT fk_impersonationlog_adminusers_adminuserid_1038626743;

ALTER TABLE public.impersonationlog DROP CONSTRAINT fk_impersonationlog_aspnetusers_aspnetuserid_1054626800;

ALTER TABLE public.importedjobsfederal DROP CONSTRAINT fk_importedjobsfederal_jobids_jobid_1570104634;

ALTER TABLE public.importedjobswanted DROP CONSTRAINT fk_importedjobswanted_jobids_jobid_1586104691;

ALTER TABLE public.jobalerts DROP CONSTRAINT fk_jobalerts_aspnetusers_aspnetuserid_1122103038;

ALTER TABLE public.jobids DROP CONSTRAINT fk_jobids_jobsources_jobsourceid_766625774;

ALTER TABLE public.jobs DROP CONSTRAINT fk_jobs_jobids_jobid_1602104748;

ALTER TABLE public.jobs DROP CONSTRAINT fk_jobs_jobsources_jobsourceid_782625831;

ALTER TABLE public.jobs DROP CONSTRAINT fk_jobs_locationlookups_locationid_222623836;

ALTER TABLE public.jobs DROP CONSTRAINT fk_jobs_naicscodes_naicsid_14623095;

ALTER TABLE public.jobs DROP CONSTRAINT fk_jobs_noccodes2021_noccodeid2021_1051150790;

ALTER TABLE public.jobs DROP CONSTRAINT fk_jobs_noccodes_noccodeid_1650104919;

ALTER TABLE public.jobseekeradmincomments DROP CONSTRAINT fk_jobseekeradmincomments_adminusers_enteredbyadminuserid_1906105831;

ALTER TABLE public.jobseekeradmincomments DROP CONSTRAINT fk_jobseekeradmincomments_aspnetusers_aspnetuserid_1890105774;

ALTER TABLE public.jobseekerchangelog DROP CONSTRAINT fk_jobseekeradminlog_aspnetusers_aspnetuserid_1662628966;

ALTER TABLE public.jobseekerchangelog DROP CONSTRAINT fk_jobseekerchangelog_adminusers_modifiedbyadminuserid_1630628852;

ALTER TABLE public.jobseekereventlog DROP CONSTRAINT fk_jobseekereventlog_aspnetusers_aspnetuserid_2018106230;

ALTER TABLE public.jobseekerflags DROP CONSTRAINT fk_jobseekerflags_aspnetusers_aspnetuserid_178099675;

ALTER TABLE public.jobseekerstats DROP CONSTRAINT fk_jobseekerstats_jobseekerstatlabels_labelkey_1454628225;

ALTER TABLE public.jobseekerstats DROP CONSTRAINT fk_jobseekerstats_regions_regionid_1502628396;

ALTER TABLE public.jobseekerstats DROP CONSTRAINT fk_jobseekerstats_weeklyperiods_weeklyperiodid_1262627541;

ALTER TABLE public.jobseekerversions DROP CONSTRAINT fk_jobseekerversions_aspnetusers_aspnetuserid_302624121;

ALTER TABLE public.jobseekerversions DROP CONSTRAINT fk_jobseekerversions_countries_countryid_430624577;

ALTER TABLE public.jobseekerversions DROP CONSTRAINT fk_jobseekerversions_locations_locationid_366624349;

ALTER TABLE public.jobseekerversions DROP CONSTRAINT fk_jobseekerversions_provinces_provinceid_350624292;

ALTER TABLE public.jobstats DROP CONSTRAINT fk_jobstats_jobsources_jobsourceid_1358627883;

ALTER TABLE public.jobstats DROP CONSTRAINT fk_jobstats_regions_regionid_1518628453;

ALTER TABLE public.jobstats DROP CONSTRAINT fk_jobstats_weeklyperiods_weeklyperiodid_1374627940;

ALTER TABLE public.jobversions DROP CONSTRAINT fk_jobversions_jobs_jobid_62623266;

ALTER TABLE public.jobversions DROP CONSTRAINT fk_jobversions_locationlookups_locationid_238623893;

ALTER TABLE public.jobversions DROP CONSTRAINT fk_jobversions_naicscodes_naicsid_94623380;

ALTER TABLE public.jobversions DROP CONSTRAINT fk_jobversions_noccodes2021_noccodeid2021_1067150847;

ALTER TABLE public.jobversions DROP CONSTRAINT fk_jobversions_noccodes_noccodeid_110623437;

ALTER TABLE public.jobviews DROP CONSTRAINT fk_jobviews_jobs_jobid_1442104178;

ALTER TABLE public.locations DROP CONSTRAINT fk_locationlookups_regions_regionid_254623950;

ALTER TABLE public.locations DROP CONSTRAINT fk_locations_regions_regionid_446624634;

ALTER TABLE public.reportpersistencecontrol DROP CONSTRAINT fk_reportpersistencecontrol_weeklyperiods_weeklyperiodid_1150627142;

ALTER TABLE public.savedcareerprofiles DROP CONSTRAINT fk_savedcareerprofiles_aspnetusers_aspnetuserid_1458104235;

ALTER TABLE public.savedcareerprofiles DROP CONSTRAINT fk_savedcareerprofiles_noccodes2021_id_1083150904;

ALTER TABLE public.savedindustryprofiles DROP CONSTRAINT fk_savedindustryprofiles_aspnetusers_aspnetuserid_1474104292;

ALTER TABLE public.savedindustryprofiles DROP CONSTRAINT fk_savedindustryprofiles_industries_id_1099150961;

ALTER TABLE public.savedjobs DROP CONSTRAINT fk_savedjobs_aspnetusers_aspnetuserid_514100872;

ALTER TABLE public.savedjobs DROP CONSTRAINT fk_savedjobs_jobs_jobid_1362103893;

ALTER TABLE public.systemsettings DROP CONSTRAINT fk_systemsettings_adminusers_modifiedbyadminuserid_1490104349;

-- ------------ Write DROP-CONSTRAINT-stage scripts -----------

ALTER TABLE public.__efmigrationshistory DROP CONSTRAINT pk___efmigrationshistory_1221579390;

ALTER TABLE public.adminusers DROP CONSTRAINT pk_adminusers_322100188;

ALTER TABLE public.aspnetroleclaims DROP CONSTRAINT pk_aspnetroleclaims_1813581499;

ALTER TABLE public.aspnetroles DROP CONSTRAINT pk_aspnetroles_1749581271;

ALTER TABLE public.aspnetuserclaims DROP CONSTRAINT pk_aspnetuserclaims_1861581670;

ALTER TABLE public.aspnetuserlogins DROP CONSTRAINT pk_aspnetuserlogins_418100530;

ALTER TABLE public.aspnetuserroles DROP CONSTRAINT pk_aspnetuserroles_1957582012;

ALTER TABLE public.aspnetusers DROP CONSTRAINT pk_aspnetusers_1781581385;

ALTER TABLE public.aspnetusertokens DROP CONSTRAINT pk_aspnetusertokens_402100473;

ALTER TABLE public.countries DROP CONSTRAINT pk_countries_398624463;

ALTER TABLE public.dataprotectionkeys DROP CONSTRAINT pk_dataprotectionkeys_2078630448;

ALTER TABLE public.deletedjobs DROP CONSTRAINT pk_deletedjobs_958626458;

ALTER TABLE public.expiredjobs DROP CONSTRAINT pk_expiredjobs_1710629137;

ALTER TABLE public.geocodedlocationcache DROP CONSTRAINT pk_geocodedlocationcache_1669580986;

ALTER TABLE public.impersonationlog DROP CONSTRAINT pk_impersonationlog_1022626686;

ALTER TABLE public.importedjobsfederal DROP CONSTRAINT pk_importedjobsfederal_290100074;

ALTER TABLE public.importedjobswanted DROP CONSTRAINT pk_importedjobswanted_1509580416;

ALTER TABLE public.industries DROP CONSTRAINT pk_naicscodes_2146106686;

ALTER TABLE public.jobalerts DROP CONSTRAINT pk_jobalerts_1106102981;

ALTER TABLE public.jobids DROP CONSTRAINT pk_jobids_1538104520;

ALTER TABLE public.jobs DROP CONSTRAINT pk_jobs_802101898;

ALTER TABLE public.jobseekeradmincomments DROP CONSTRAINT pk_jobseekeradmincomments_1874105717;

ALTER TABLE public.jobseekerchangelog DROP CONSTRAINT pk_jobseekerchangelog_1646628909;

ALTER TABLE public.jobseekereventlog DROP CONSTRAINT pk_jobseekereventlog_2002106173;

ALTER TABLE public.jobseekerflags DROP CONSTRAINT pk_jobseekerflags_98099390;

ALTER TABLE public.jobseekerstatlabels DROP CONSTRAINT pk_jobseekerstatlabels_1438628168;

ALTER TABLE public.jobseekerstats DROP CONSTRAINT pk_jobseekerstats_1406628054;

ALTER TABLE public.jobseekerversions DROP CONSTRAINT pk_jobseekerversions_286624064;

ALTER TABLE public.jobsources DROP CONSTRAINT pk_jobsources_734625660;

ALTER TABLE public.jobstats DROP CONSTRAINT pk_jobstats_1342627826;

ALTER TABLE public.jobversions DROP CONSTRAINT pk_jobversions_46623209;

ALTER TABLE public.jobviews DROP CONSTRAINT pk_jobviews_786101841;

ALTER TABLE public.locations DROP CONSTRAINT pk_locationlookups_466100701;

ALTER TABLE public.noccategories DROP CONSTRAINT pk_noccategories_2114106572;

ALTER TABLE public.noccategories2021 DROP CONSTRAINT pk_noccategories2021_1131151075;

ALTER TABLE public.noccodes DROP CONSTRAINT pk_noccodes_1634104862;

ALTER TABLE public.noccodes2021 DROP CONSTRAINT pk_noccodes2021_1035150733;

ALTER TABLE public.provinces DROP CONSTRAINT pk_provinces_142623551;

ALTER TABLE public.regions DROP CONSTRAINT pk_regions_174623665;

ALTER TABLE public.reportpersistencecontrol DROP CONSTRAINT pk_reportpersistencecontrol_1566628624;

ALTER TABLE public.savedcareerprofiles DROP CONSTRAINT pk_savedcareerprofiles_1250103494;

ALTER TABLE public.savedindustryprofiles DROP CONSTRAINT pk_savedindustryprofiles_1282103608;

ALTER TABLE public.savedjobs DROP CONSTRAINT pk_savedjobs_498100815;

ALTER TABLE public.securityquestions DROP CONSTRAINT pk_securityquestions_770101784;

ALTER TABLE public.sysdiagrams DROP CONSTRAINT pk__sysdiagr__c2b05b619312df6e;

ALTER TABLE public.sysdiagrams DROP CONSTRAINT uk_principal_name_891150220;

ALTER TABLE public.systemsettings DROP CONSTRAINT pk_systemsettings_1678629023;

ALTER TABLE public.weeklyperiods DROP CONSTRAINT pk_weeklyperiods_542624976;

-- ------------ Write DROP-INDEX-stage scripts -----------

DROP INDEX IF EXISTS public.ix_adminusers_ix_adminusers_lockedbyadminuserid;

DROP INDEX IF EXISTS public.ix_adminusers_ix_adminusers_modifiedbyadminuserid;

DROP INDEX IF EXISTS public.ix_aspnetroleclaims_ix_aspnetroleclaims_roleid;

DROP INDEX IF EXISTS public.ix_aspnetroles_rolenameindex;

DROP INDEX IF EXISTS public.ix_aspnetuserclaims_ix_aspnetuserclaims_userid;

DROP INDEX IF EXISTS public.ix_aspnetuserlogins_ix_aspnetuserlogins_userid;

DROP INDEX IF EXISTS public.ix_aspnetuserroles_ix_aspnetuserroles_roleid;

DROP INDEX IF EXISTS public.ix_aspnetusers_emailindex;

DROP INDEX IF EXISTS public.ix_aspnetusers_ix_aspnetusers_accountstatus_lastname_firstname;

DROP INDEX IF EXISTS public.ix_aspnetusers_ix_aspnetusers_countryid;

DROP INDEX IF EXISTS public.ix_aspnetusers_ix_aspnetusers_dateregistered;

DROP INDEX IF EXISTS public.ix_aspnetusers_ix_aspnetusers_email;

DROP INDEX IF EXISTS public.ix_aspnetusers_ix_aspnetusers_firstname_lastname;

DROP INDEX IF EXISTS public.ix_aspnetusers_ix_aspnetusers_lastmodified;

DROP INDEX IF EXISTS public.ix_aspnetusers_ix_aspnetusers_lastname_firstname;

DROP INDEX IF EXISTS public.ix_aspnetusers_ix_aspnetusers_locationid;

DROP INDEX IF EXISTS public.ix_aspnetusers_ix_aspnetusers_lockedbyadminuserid;

DROP INDEX IF EXISTS public.ix_aspnetusers_ix_aspnetusers_provinceid;

DROP INDEX IF EXISTS public.ix_aspnetusers_ix_aspnetusers_securityquestionid;

DROP INDEX IF EXISTS public.ix_aspnetusers_usernameindex;

DROP INDEX IF EXISTS public.ix_deletedjobs_ix_deletedjobs_deletedbyadminuserid;

DROP INDEX IF EXISTS public.ix_geocodedlocationcache_ix_geocodedlocationcache_name;

DROP INDEX IF EXISTS public.ix_impersonationlog_ix_impersonationlog_adminuserid;

DROP INDEX IF EXISTS public.ix_impersonationlog_ix_impersonationlog_aspnetuserid;

DROP INDEX IF EXISTS public.ix_importedjobswanted_ix_importedjobswanted_hashid;

DROP INDEX IF EXISTS public.ix_jobalerts_ix_jobalerts_aspnetuserid;

DROP INDEX IF EXISTS public.ix_jobalerts_ix_jobalerts_datecreated;

DROP INDEX IF EXISTS public.ix_jobids_ix_jobids_jobsourceid;

DROP INDEX IF EXISTS public.ix_jobs_ix_jobs_jobsourceid;

DROP INDEX IF EXISTS public.ix_jobs_ix_jobs_locationid;

DROP INDEX IF EXISTS public.ix_jobs_ix_jobs_naicsid;

DROP INDEX IF EXISTS public.ix_jobs_ix_jobs_noccodeid;

DROP INDEX IF EXISTS public.ix_jobseekeradmincomments_ix_jobseekeradmincomments_aspnetuserid;

DROP INDEX IF EXISTS public.ix_jobseekeradmincomments_ix_jobseekeradmincomments_enteredbyadminuserid;

DROP INDEX IF EXISTS public.ix_jobseekerchangelog_ix_jobseekerchangelog_aspnetuserid;

DROP INDEX IF EXISTS public.ix_jobseekerchangelog_ix_jobseekerchangelog_modifiedbyadminuserid;

DROP INDEX IF EXISTS public.ix_jobseekereventlog_ix_jobseekereventlog_aspnetuserid;

DROP INDEX IF EXISTS public.ix_jobseekereventlog_ix_jobseekereventlog_datelogged;

DROP INDEX IF EXISTS public.ix_jobseekerflags_ix_jobseekerflags_aspnetuserid;

DROP INDEX IF EXISTS public.ix_jobseekerstats_ix_jobseekerstats_labelkey;

DROP INDEX IF EXISTS public.ix_jobseekerstats_ix_jobseekerstats_regionid;

DROP INDEX IF EXISTS public.ix_jobseekerversions_ix_jobseekerversions_aspnetuserid_versionnumber;

DROP INDEX IF EXISTS public.ix_jobseekerversions_ix_jobseekerversions_countryid;

DROP INDEX IF EXISTS public.ix_jobseekerversions_ix_jobseekerversions_locationid;

DROP INDEX IF EXISTS public.ix_jobseekerversions_ix_jobseekerversions_provinceid;

DROP INDEX IF EXISTS public.ix_jobstats_ix_jobstats_jobsourceid;

DROP INDEX IF EXISTS public.ix_jobstats_ix_jobstats_regionid;

DROP INDEX IF EXISTS public.ix_jobversions_ix_jobversions_jobid_versionnumber;

DROP INDEX IF EXISTS public.ix_jobversions_ix_jobversions_locationid;

DROP INDEX IF EXISTS public.ix_jobversions_ix_jobversions_naicsid;

DROP INDEX IF EXISTS public.ix_jobversions_ix_jobversions_noccodeid;

DROP INDEX IF EXISTS public.ix_locations_ix_locationlookups_regionid;

DROP INDEX IF EXISTS public.ix_savedcareerprofiles_ix_savedcareerprofiles_aspnetuserid;

DROP INDEX IF EXISTS public.ix_savedcareerprofiles_ix_savedcareerprofiles_datedeleted;

DROP INDEX IF EXISTS public.ix_savedcareerprofiles_ix_savedcareerprofiles_datesaved;

DROP INDEX IF EXISTS public.ix_savedindustryprofiles_ix_savedindustryprofiles_aspnetuserid;

DROP INDEX IF EXISTS public.ix_savedindustryprofiles_ix_savedindustryprofiles_datedeleted;

DROP INDEX IF EXISTS public.ix_savedindustryprofiles_ix_savedindustryprofiles_datesaved;

DROP INDEX IF EXISTS public.ix_savedjobs_ix_savedjobs_aspnetuserid;

DROP INDEX IF EXISTS public.ix_savedjobs_ix_savedjobs_jobid;

DROP INDEX IF EXISTS public.ix_systemsettings_ix_systemsettings_modifiedbyadminuserid;

DROP INDEX IF EXISTS public.ix_weeklyperiods_ix_weeklyperiods_weekenddate;

-- ------------ Write DROP-TABLE-stage scripts -----------

DROP TABLE IF EXISTS public.__efmigrationshistory;

DROP TABLE IF EXISTS public.adminusers;

DROP TABLE IF EXISTS public.aspnetroleclaims;

DROP TABLE IF EXISTS public.aspnetroles;

DROP TABLE IF EXISTS public.aspnetuserclaims;

DROP TABLE IF EXISTS public.aspnetuserlogins;

DROP TABLE IF EXISTS public.aspnetuserroles;

DROP TABLE IF EXISTS public.aspnetusers;

DROP TABLE IF EXISTS public.aspnetusertokens;

DROP TABLE IF EXISTS public.countries;

DROP TABLE IF EXISTS public.dataprotectionkeys;

DROP TABLE IF EXISTS public.deletedjobs;

DROP TABLE IF EXISTS public.expiredjobs;

DROP TABLE IF EXISTS public.geocodedlocationcache;

DROP TABLE IF EXISTS public.impersonationlog;

DROP TABLE IF EXISTS public.importedjobsfederal;

DROP TABLE IF EXISTS public.importedjobswanted;

DROP TABLE IF EXISTS public.industries;

DROP TABLE IF EXISTS public.jobalerts;

DROP TABLE IF EXISTS public.jobids;

DROP TABLE IF EXISTS public.jobs;

DROP TABLE IF EXISTS public.jobseekeradmincomments;

DROP TABLE IF EXISTS public.jobseekerchangelog;

DROP TABLE IF EXISTS public.jobseekereventlog;

DROP TABLE IF EXISTS public.jobseekerflags;

DROP TABLE IF EXISTS public.jobseekerstatlabels;

DROP TABLE IF EXISTS public.jobseekerstats;

DROP TABLE IF EXISTS public.jobseekerversions;

DROP TABLE IF EXISTS public.jobsources;

DROP TABLE IF EXISTS public.jobstats;

DROP TABLE IF EXISTS public.jobversions;

DROP TABLE IF EXISTS public.jobviews;

DROP TABLE IF EXISTS public.locations;

DROP TABLE IF EXISTS public.noccategories;

DROP TABLE IF EXISTS public.noccategories2021;

DROP TABLE IF EXISTS public.noccodes;

DROP TABLE IF EXISTS public.noccodes2021;

DROP TABLE IF EXISTS public.provinces;

DROP TABLE IF EXISTS public.regions;

DROP TABLE IF EXISTS public.reportpersistencecontrol;

DROP TABLE IF EXISTS public.savedcareerprofiles;

DROP TABLE IF EXISTS public.savedindustryprofiles;

DROP TABLE IF EXISTS public.savedjobs;

DROP TABLE IF EXISTS public.securityquestions;

DROP TABLE IF EXISTS public.sysdiagrams;

DROP TABLE IF EXISTS public.systemsettings;

DROP TABLE IF EXISTS public.weeklyperiods;

-- ------------ Write DROP-DATABASE-stage scripts -----------

-- ------------ Write CREATE-DATABASE-stage scripts -----------

CREATE SCHEMA IF NOT EXISTS public;

-- ------------ Write CREATE-TABLE-stage scripts -----------

CREATE TABLE public.__efmigrationshistory(
    migrationid VARCHAR(150) NOT NULL,
    productversion VARCHAR(32) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.adminusers(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    samaccountname VARCHAR(20),
    displayname VARCHAR(60) NOT NULL,
    dateupdated TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    adminlevel INTEGER NOT NULL,
    datecreated TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    deleted NUMERIC(1,0) NOT NULL,
    guid VARCHAR(40),
    datelocked TIMESTAMP(6) WITHOUT TIME ZONE,
    lockedbyadminuserid INTEGER,
    modifiedbyadminuserid INTEGER,
    datelastlogin TIMESTAMP(6) WITHOUT TIME ZONE,
    givenname VARCHAR(40),
    surname VARCHAR(40)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.aspnetroleclaims(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    roleid VARCHAR(450) NOT NULL,
    claimtype TEXT,
    claimvalue TEXT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.aspnetroles(
    id VARCHAR(450) NOT NULL,
    name VARCHAR(256),
    normalizedname VARCHAR(256),
    concurrencystamp TEXT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.aspnetuserclaims(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    userid VARCHAR(450) NOT NULL,
    claimtype TEXT,
    claimvalue TEXT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.aspnetuserlogins(
    loginprovider VARCHAR(128) NOT NULL,
    providerkey VARCHAR(128) NOT NULL,
    providerdisplayname TEXT,
    userid VARCHAR(450) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.aspnetuserroles(
    userid VARCHAR(450) NOT NULL,
    roleid VARCHAR(450) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.aspnetusers(
    id VARCHAR(450) NOT NULL,
    username VARCHAR(256),
    normalizedusername VARCHAR(256),
    email VARCHAR(256),
    normalizedemail VARCHAR(256),
    emailconfirmed NUMERIC(1,0) NOT NULL,
    passwordhash TEXT,
    securitystamp TEXT,
    concurrencystamp TEXT,
    phonenumber TEXT,
    phonenumberconfirmed NUMERIC(1,0) NOT NULL,
    twofactorenabled NUMERIC(1,0) NOT NULL,
    lockoutend TIMESTAMP(6) WITH TIME ZONE,
    lockoutenabled NUMERIC(1,0) NOT NULL,
    accessfailedcount INTEGER NOT NULL,
    locationid INTEGER,
    city VARCHAR(50),
    countryid INTEGER,
    firstname VARCHAR(50),
    lastname VARCHAR(50),
    legacywebuserid INTEGER,
    provinceid INTEGER,
    accountstatus SMALLINT NOT NULL,
    dateregistered TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    lastlogon TIMESTAMP(6) WITHOUT TIME ZONE,
    lastmodified TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    verificationguid UUID,
    securityanswer VARCHAR(50),
    securityquestionid INTEGER,
    datelocked TIMESTAMP(6) WITHOUT TIME ZONE,
    lockedbyadminuserid INTEGER
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.aspnetusertokens(
    userid VARCHAR(450) NOT NULL,
    loginprovider VARCHAR(128) NOT NULL,
    name VARCHAR(128) NOT NULL,
    value TEXT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.countries(
    id INTEGER NOT NULL,
    name VARCHAR(50),
    countrytwolettercode VARCHAR(2),
    sortorder SMALLINT NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.dataprotectionkeys(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    friendlyname TEXT,
    xml TEXT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.deletedjobs(
    jobid BIGINT NOT NULL,
    deletedbyadminuserid INTEGER NOT NULL,
    datedeleted TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.expiredjobs(
    jobid BIGINT NOT NULL,
    removedfromelasticsearch NUMERIC(1,0) NOT NULL,
    dateremoved TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.geocodedlocationcache(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    name VARCHAR(120),
    latitude VARCHAR(25),
    longitude VARCHAR(25),
    dategeocoded TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    ispermanent NUMERIC(1,0) NOT NULL,
    city VARCHAR(80),
    province VARCHAR(2),
    frenchcity VARCHAR(80)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.impersonationlog(
    token VARCHAR(200) NOT NULL,
    aspnetuserid VARCHAR(450),
    adminuserid INTEGER NOT NULL,
    datetokencreated TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.importedjobsfederal(
    jobid BIGINT NOT NULL,
    apidate TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    datefirstimported TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    jobpostenglish TEXT,
    jobpostfrench TEXT,
    reindexneeded NUMERIC(1,0) NOT NULL,
    displayuntil TIMESTAMP(6) WITHOUT TIME ZONE,
    datelastimported TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000'
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.importedjobswanted(
    jobid BIGINT NOT NULL,
    jobpostenglish TEXT,
    datefirstimported TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    apidate TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    reindexneeded NUMERIC(1,0) NOT NULL,
    datelastimported TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    isfederalorworkbc NUMERIC(1,0) NOT NULL,
    hashid BIGINT NOT NULL,
    datelastseen TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000'
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.industries(
    id SMALLINT NOT NULL,
    title VARCHAR(150),
    titlebc VARCHAR(150)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobalerts(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    title VARCHAR(50),
    alertfrequency SMALLINT NOT NULL,
    urlparameters VARCHAR(1000),
    datecreated TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    datemodified TIMESTAMP(6) WITHOUT TIME ZONE,
    datedeleted TIMESTAMP(6) WITHOUT TIME ZONE,
    isdeleted NUMERIC(1,0) NOT NULL,
    aspnetuserid VARCHAR(450),
    jobsearchfilters TEXT,
    jobsearchfiltersversion INTEGER NOT NULL DEFAULT (0)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobids(
    id BIGINT NOT NULL,
    datefirstimported TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    jobsourceid SMALLINT NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobs(
    jobid BIGINT NOT NULL,
    title VARCHAR(300),
    noccodeid SMALLINT,
    positionsavailable SMALLINT NOT NULL,
    employername VARCHAR(100),
    dateposted TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    casual NUMERIC(1,0) NOT NULL,
    city VARCHAR(120),
    expiredate TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    fulltime NUMERIC(1,0) NOT NULL,
    lastupdated TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    leadingtofulltime NUMERIC(1,0) NOT NULL,
    locationid INTEGER NOT NULL DEFAULT (0),
    industryid SMALLINT,
    parttime NUMERIC(1,0) NOT NULL,
    permanent NUMERIC(1,0) NOT NULL,
    salary NUMERIC(18,2),
    salarysummary VARCHAR(60),
    seasonal NUMERIC(1,0) NOT NULL,
    temporary NUMERIC(1,0) NOT NULL,
    datefirstimported TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    isactive NUMERIC(1,0) NOT NULL,
    datelastimported TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    jobsourceid SMALLINT NOT NULL,
    originalsource VARCHAR(100),
    externalsourceurl VARCHAR(800),
    actualdateposted TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    noccodeid2021 INTEGER
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobseekeradmincomments(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    aspnetuserid VARCHAR(450),
    comment TEXT,
    ispinned NUMERIC(1,0) NOT NULL,
    enteredbyadminuserid INTEGER NOT NULL,
    dateentered TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobseekerchangelog(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    aspnetuserid VARCHAR(450),
    field VARCHAR(100),
    oldvalue TEXT,
    newvalue TEXT,
    modifiedbyadminuserid INTEGER,
    dateupdated TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobseekereventlog(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    aspnetuserid VARCHAR(450),
    eventtypeid INTEGER NOT NULL,
    datelogged TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobseekerflags(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    aspnetuserid VARCHAR(450),
    isapprentice NUMERIC(1,0) NOT NULL,
    isindigenousperson NUMERIC(1,0) NOT NULL,
    ismatureworker NUMERIC(1,0) NOT NULL,
    isnewimmigrant NUMERIC(1,0) NOT NULL,
    ispersonwithdisability NUMERIC(1,0) NOT NULL,
    isstudent NUMERIC(1,0) NOT NULL,
    isveteran NUMERIC(1,0) NOT NULL,
    isvisibleminority NUMERIC(1,0) NOT NULL,
    isyouth NUMERIC(1,0) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobseekerstatlabels(
    key VARCHAR(4) NOT NULL,
    label VARCHAR(100),
    istotal NUMERIC(1,0) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobseekerstats(
    weeklyperiodid INTEGER NOT NULL,
    value INTEGER NOT NULL,
    regionid INTEGER NOT NULL,
    labelkey VARCHAR(4) NOT NULL DEFAULT ''
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobseekerversions(
    id BIGINT NOT NULL GENERATED ALWAYS AS IDENTITY,
    aspnetuserid VARCHAR(450),
    countryid INTEGER,
    provinceid INTEGER,
    locationid INTEGER,
    dateregistered TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    accountstatus SMALLINT NOT NULL,
    emailconfirmed NUMERIC(1,0) NOT NULL,
    isapprentice NUMERIC(1,0) NOT NULL,
    isindigenousperson NUMERIC(1,0) NOT NULL,
    ismatureworker NUMERIC(1,0) NOT NULL,
    isnewimmigrant NUMERIC(1,0) NOT NULL,
    ispersonwithdisability NUMERIC(1,0) NOT NULL,
    isstudent NUMERIC(1,0) NOT NULL,
    isveteran NUMERIC(1,0) NOT NULL,
    isvisibleminority NUMERIC(1,0) NOT NULL,
    isyouth NUMERIC(1,0) NOT NULL,
    dateversionstart TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    dateversionend TIMESTAMP(6) WITHOUT TIME ZONE,
    iscurrentversion NUMERIC(1,0) NOT NULL,
    versionnumber SMALLINT NOT NULL,
    email VARCHAR(256)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobsources(
    id SMALLINT NOT NULL,
    name VARCHAR(50),
    groupname VARCHAR(50),
    listorder SMALLINT NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobstats(
    weeklyperiodid INTEGER NOT NULL,
    jobsourceid SMALLINT NOT NULL,
    regionid INTEGER NOT NULL,
    jobpostings INTEGER NOT NULL,
    positionsavailable INTEGER NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobversions(
    id BIGINT NOT NULL GENERATED ALWAYS AS IDENTITY,
    jobid BIGINT NOT NULL,
    locationid INTEGER NOT NULL,
    noccodeid SMALLINT,
    industryid SMALLINT,
    positionsavailable SMALLINT NOT NULL,
    dateposted TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    isactive NUMERIC(1,0) NOT NULL,
    dateversionstart TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    dateversionend TIMESTAMP(6) WITHOUT TIME ZONE,
    iscurrentversion NUMERIC(1,0) NOT NULL,
    versionnumber SMALLINT NOT NULL,
    actualdateposted TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    datefirstimported TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL DEFAULT '0001-01-01T00:00:00.0000000',
    jobsourceid SMALLINT NOT NULL,
    noccodeid2021 INTEGER
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.jobviews(
    jobid BIGINT NOT NULL,
    views INTEGER,
    datelastviewed TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.locations(
    locationid INTEGER NOT NULL,
    edm_location_districtlocationid INTEGER NOT NULL,
    regionid INTEGER,
    federalcityid INTEGER,
    city VARCHAR(50),
    label VARCHAR(50),
    isduplicate NUMERIC(1,0) NOT NULL,
    ishidden NUMERIC(1,0) NOT NULL,
    latitude VARCHAR(25),
    longitude VARCHAR(25),
    bcstatsplaceid INTEGER
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.noccategories(
    categorycode VARCHAR(3) NOT NULL,
    level SMALLINT NOT NULL,
    title VARCHAR(150)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.noccategories2021(
    categorycode VARCHAR(4) NOT NULL,
    level SMALLINT NOT NULL,
    title VARCHAR(150)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.noccodes(
    code VARCHAR(4),
    title VARCHAR(150),
    id SMALLINT NOT NULL,
    frenchtitle VARCHAR(180)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.noccodes2021(
    id INTEGER NOT NULL,
    code VARCHAR(5),
    title VARCHAR(150),
    frenchtitle VARCHAR(250),
    code2016 VARCHAR(30)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.provinces(
    provinceid INTEGER NOT NULL,
    name VARCHAR(50),
    shortname VARCHAR(2)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.regions(
    id INTEGER NOT NULL,
    name VARCHAR(50),
    listorder SMALLINT NOT NULL,
    ishidden NUMERIC(1,0) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.reportpersistencecontrol(
    weeklyperiodid INTEGER NOT NULL,
    datecalculated TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    istotaltodate NUMERIC(1,0) NOT NULL,
    tablename VARCHAR(25) NOT NULL DEFAULT ''
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.savedcareerprofiles(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    edm_careerprofile_careerprofileid INTEGER,
    aspnetuserid VARCHAR(450),
    datesaved TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    datedeleted TIMESTAMP(6) WITHOUT TIME ZONE,
    isdeleted NUMERIC(1,0) NOT NULL,
    noccodeid2021 INTEGER
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.savedindustryprofiles(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    aspnetuserid VARCHAR(450),
    datesaved TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    datedeleted TIMESTAMP(6) WITHOUT TIME ZONE,
    isdeleted NUMERIC(1,0) NOT NULL,
    industryid SMALLINT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.savedjobs(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    jobid BIGINT NOT NULL,
    aspnetuserid VARCHAR(450),
    datesaved TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    datedeleted TIMESTAMP(6) WITHOUT TIME ZONE,
    isdeleted NUMERIC(1,0) NOT NULL,
    note VARCHAR(800),
    noteupdateddate TIMESTAMP(6) WITHOUT TIME ZONE
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.securityquestions(
    id INTEGER NOT NULL,
    questiontext VARCHAR(40)
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.sysdiagrams(
    name VARCHAR(128) NOT NULL,
    principal_id INTEGER NOT NULL,
    diagram_id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    version INTEGER,
    definition BYTEA
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.systemsettings(
    name VARCHAR(400) NOT NULL,
    value TEXT,
    description TEXT,
    fieldtype INTEGER NOT NULL,
    modifiedbyadminuserid INTEGER NOT NULL,
    dateupdated TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    defaultvalue TEXT
)
        WITH (
        OIDS=FALSE
        );

CREATE TABLE public.weeklyperiods(
    id INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY,
    calendaryear SMALLINT NOT NULL,
    calendarmonth SMALLINT NOT NULL,
    fiscalyear SMALLINT NOT NULL,
    weekofmonth SMALLINT NOT NULL,
    weekstartdate TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    weekenddate TIMESTAMP(6) WITHOUT TIME ZONE NOT NULL,
    isendoffiscalyear NUMERIC(1,0) NOT NULL,
    isendofmonth NUMERIC(1,0) NOT NULL
)
        WITH (
        OIDS=FALSE
        );

-- ------------ Write CREATE-INDEX-stage scripts -----------

CREATE INDEX ix_adminusers_ix_adminusers_lockedbyadminuserid
ON public.adminusers
USING BTREE (lockedbyadminuserid ASC);

CREATE INDEX ix_adminusers_ix_adminusers_modifiedbyadminuserid
ON public.adminusers
USING BTREE (modifiedbyadminuserid ASC);

CREATE INDEX ix_aspnetroleclaims_ix_aspnetroleclaims_roleid
ON public.aspnetroleclaims
USING BTREE (roleid ASC);

CREATE UNIQUE INDEX ix_aspnetroles_rolenameindex
ON public.aspnetroles
USING BTREE (normalizedname ASC)
WHERE
(normalizedname IS NOT NULL);

CREATE INDEX ix_aspnetuserclaims_ix_aspnetuserclaims_userid
ON public.aspnetuserclaims
USING BTREE (userid ASC);

CREATE INDEX ix_aspnetuserlogins_ix_aspnetuserlogins_userid
ON public.aspnetuserlogins
USING BTREE (userid ASC);

CREATE INDEX ix_aspnetuserroles_ix_aspnetuserroles_roleid
ON public.aspnetuserroles
USING BTREE (roleid ASC);

CREATE INDEX ix_aspnetusers_emailindex
ON public.aspnetusers
USING BTREE (normalizedemail ASC);

CREATE INDEX ix_aspnetusers_ix_aspnetusers_accountstatus_lastname_firstname
ON public.aspnetusers
USING BTREE (accountstatus ASC, lastname ASC, firstname ASC) INCLUDE(email);

CREATE INDEX ix_aspnetusers_ix_aspnetusers_countryid
ON public.aspnetusers
USING BTREE (countryid ASC);

CREATE INDEX ix_aspnetusers_ix_aspnetusers_dateregistered
ON public.aspnetusers
USING BTREE (dateregistered ASC) INCLUDE(lastname, firstname, email, accountstatus);

CREATE UNIQUE INDEX ix_aspnetusers_ix_aspnetusers_email
ON public.aspnetusers
USING BTREE (email ASC) INCLUDE(lastname, firstname, accountstatus)
WHERE
(email IS NOT NULL);

CREATE INDEX ix_aspnetusers_ix_aspnetusers_firstname_lastname
ON public.aspnetusers
USING BTREE (firstname ASC, lastname ASC) INCLUDE(email, accountstatus);

CREATE INDEX ix_aspnetusers_ix_aspnetusers_lastmodified
ON public.aspnetusers
USING BTREE (lastmodified ASC) INCLUDE(lastname, firstname, email, accountstatus);

CREATE INDEX ix_aspnetusers_ix_aspnetusers_lastname_firstname
ON public.aspnetusers
USING BTREE (lastname ASC, firstname ASC) INCLUDE(email, accountstatus);

CREATE INDEX ix_aspnetusers_ix_aspnetusers_locationid
ON public.aspnetusers
USING BTREE (locationid ASC);

CREATE INDEX ix_aspnetusers_ix_aspnetusers_lockedbyadminuserid
ON public.aspnetusers
USING BTREE (lockedbyadminuserid ASC);

CREATE INDEX ix_aspnetusers_ix_aspnetusers_provinceid
ON public.aspnetusers
USING BTREE (provinceid ASC);

CREATE INDEX ix_aspnetusers_ix_aspnetusers_securityquestionid
ON public.aspnetusers
USING BTREE (securityquestionid ASC);

CREATE UNIQUE INDEX ix_aspnetusers_usernameindex
ON public.aspnetusers
USING BTREE (normalizedusername ASC)
WHERE
(normalizedusername IS NOT NULL);

CREATE INDEX ix_deletedjobs_ix_deletedjobs_deletedbyadminuserid
ON public.deletedjobs
USING BTREE (deletedbyadminuserid ASC);

CREATE UNIQUE INDEX ix_geocodedlocationcache_ix_geocodedlocationcache_name
ON public.geocodedlocationcache
USING BTREE (name ASC)
WHERE
(name IS NOT NULL);

CREATE INDEX ix_impersonationlog_ix_impersonationlog_adminuserid
ON public.impersonationlog
USING BTREE (adminuserid ASC);

CREATE INDEX ix_impersonationlog_ix_impersonationlog_aspnetuserid
ON public.impersonationlog
USING BTREE (aspnetuserid ASC);

CREATE UNIQUE INDEX ix_importedjobswanted_ix_importedjobswanted_hashid
ON public.importedjobswanted
USING BTREE (hashid ASC);

CREATE INDEX ix_jobalerts_ix_jobalerts_aspnetuserid
ON public.jobalerts
USING BTREE (aspnetuserid ASC);

CREATE INDEX ix_jobalerts_ix_jobalerts_datecreated
ON public.jobalerts
USING BTREE (datecreated ASC);

CREATE INDEX ix_jobids_ix_jobids_jobsourceid
ON public.jobids
USING BTREE (jobsourceid ASC);

CREATE INDEX ix_jobs_ix_jobs_jobsourceid
ON public.jobs
USING BTREE (jobsourceid ASC);

CREATE INDEX ix_jobs_ix_jobs_locationid
ON public.jobs
USING BTREE (locationid ASC);

CREATE INDEX ix_jobs_ix_jobs_naicsid
ON public.jobs
USING BTREE (industryid ASC);

CREATE INDEX ix_jobs_ix_jobs_noccodeid
ON public.jobs
USING BTREE (noccodeid ASC);

CREATE INDEX ix_jobseekeradmincomments_ix_jobseekeradmincomments_aspnetuserid
ON public.jobseekeradmincomments
USING BTREE (aspnetuserid ASC);

CREATE INDEX ix_jobseekeradmincomments_ix_jobseekeradmincomments_enteredbyadminuserid
ON public.jobseekeradmincomments
USING BTREE (enteredbyadminuserid ASC);

CREATE INDEX ix_jobseekerchangelog_ix_jobseekerchangelog_aspnetuserid
ON public.jobseekerchangelog
USING BTREE (aspnetuserid ASC);

CREATE INDEX ix_jobseekerchangelog_ix_jobseekerchangelog_modifiedbyadminuserid
ON public.jobseekerchangelog
USING BTREE (modifiedbyadminuserid ASC);

CREATE INDEX ix_jobseekereventlog_ix_jobseekereventlog_aspnetuserid
ON public.jobseekereventlog
USING BTREE (aspnetuserid ASC);

CREATE INDEX ix_jobseekereventlog_ix_jobseekereventlog_datelogged
ON public.jobseekereventlog
USING BTREE (datelogged ASC);

CREATE UNIQUE INDEX ix_jobseekerflags_ix_jobseekerflags_aspnetuserid
ON public.jobseekerflags
USING BTREE (aspnetuserid ASC)
WHERE
(aspnetuserid IS NOT NULL);

CREATE INDEX ix_jobseekerstats_ix_jobseekerstats_labelkey
ON public.jobseekerstats
USING BTREE (labelkey ASC);

CREATE INDEX ix_jobseekerstats_ix_jobseekerstats_regionid
ON public.jobseekerstats
USING BTREE (regionid ASC);

CREATE UNIQUE INDEX ix_jobseekerversions_ix_jobseekerversions_aspnetuserid_versionnumber
ON public.jobseekerversions
USING BTREE (aspnetuserid ASC, versionnumber ASC)
WHERE
(aspnetuserid IS NOT NULL);

CREATE INDEX ix_jobseekerversions_ix_jobseekerversions_countryid
ON public.jobseekerversions
USING BTREE (countryid ASC);

CREATE INDEX ix_jobseekerversions_ix_jobseekerversions_locationid
ON public.jobseekerversions
USING BTREE (locationid ASC);

CREATE INDEX ix_jobseekerversions_ix_jobseekerversions_provinceid
ON public.jobseekerversions
USING BTREE (provinceid ASC);

CREATE INDEX ix_jobstats_ix_jobstats_jobsourceid
ON public.jobstats
USING BTREE (jobsourceid ASC);

CREATE INDEX ix_jobstats_ix_jobstats_regionid
ON public.jobstats
USING BTREE (regionid ASC);

CREATE UNIQUE INDEX ix_jobversions_ix_jobversions_jobid_versionnumber
ON public.jobversions
USING BTREE (jobid ASC, versionnumber ASC);

CREATE INDEX ix_jobversions_ix_jobversions_locationid
ON public.jobversions
USING BTREE (locationid ASC);

CREATE INDEX ix_jobversions_ix_jobversions_naicsid
ON public.jobversions
USING BTREE (industryid ASC);

CREATE INDEX ix_jobversions_ix_jobversions_noccodeid
ON public.jobversions
USING BTREE (noccodeid ASC);

CREATE INDEX ix_locations_ix_locationlookups_regionid
ON public.locations
USING BTREE (regionid ASC);

CREATE INDEX ix_savedcareerprofiles_ix_savedcareerprofiles_aspnetuserid
ON public.savedcareerprofiles
USING BTREE (aspnetuserid ASC);

CREATE INDEX ix_savedcareerprofiles_ix_savedcareerprofiles_datedeleted
ON public.savedcareerprofiles
USING BTREE (datedeleted ASC);

CREATE INDEX ix_savedcareerprofiles_ix_savedcareerprofiles_datesaved
ON public.savedcareerprofiles
USING BTREE (datesaved ASC);

CREATE INDEX ix_savedindustryprofiles_ix_savedindustryprofiles_aspnetuserid
ON public.savedindustryprofiles
USING BTREE (aspnetuserid ASC);

CREATE INDEX ix_savedindustryprofiles_ix_savedindustryprofiles_datedeleted
ON public.savedindustryprofiles
USING BTREE (datedeleted ASC);

CREATE INDEX ix_savedindustryprofiles_ix_savedindustryprofiles_datesaved
ON public.savedindustryprofiles
USING BTREE (datesaved ASC);

CREATE INDEX ix_savedjobs_ix_savedjobs_aspnetuserid
ON public.savedjobs
USING BTREE (aspnetuserid ASC);

CREATE INDEX ix_savedjobs_ix_savedjobs_jobid
ON public.savedjobs
USING BTREE (jobid ASC);

CREATE INDEX ix_systemsettings_ix_systemsettings_modifiedbyadminuserid
ON public.systemsettings
USING BTREE (modifiedbyadminuserid ASC);

CREATE INDEX ix_weeklyperiods_ix_weeklyperiods_weekenddate
ON public.weeklyperiods
USING BTREE (weekenddate ASC);

-- ------------ Write CREATE-CONSTRAINT-stage scripts -----------

ALTER TABLE public.__efmigrationshistory
ADD CONSTRAINT pk___efmigrationshistory_1221579390 PRIMARY KEY (migrationid);

ALTER TABLE public.adminusers
ADD CONSTRAINT pk_adminusers_322100188 PRIMARY KEY (id);

ALTER TABLE public.aspnetroleclaims
ADD CONSTRAINT pk_aspnetroleclaims_1813581499 PRIMARY KEY (id);

ALTER TABLE public.aspnetroles
ADD CONSTRAINT pk_aspnetroles_1749581271 PRIMARY KEY (id);

ALTER TABLE public.aspnetuserclaims
ADD CONSTRAINT pk_aspnetuserclaims_1861581670 PRIMARY KEY (id);

ALTER TABLE public.aspnetuserlogins
ADD CONSTRAINT pk_aspnetuserlogins_418100530 PRIMARY KEY (providerkey);

ALTER TABLE public.aspnetuserroles
ADD CONSTRAINT pk_aspnetuserroles_1957582012 PRIMARY KEY (userid, roleid);

ALTER TABLE public.aspnetusers
ADD CONSTRAINT pk_aspnetusers_1781581385 PRIMARY KEY (id);

ALTER TABLE public.aspnetusertokens
ADD CONSTRAINT pk_aspnetusertokens_402100473 PRIMARY KEY (name);

ALTER TABLE public.countries
ADD CONSTRAINT pk_countries_398624463 PRIMARY KEY (id);

ALTER TABLE public.dataprotectionkeys
ADD CONSTRAINT pk_dataprotectionkeys_2078630448 PRIMARY KEY (id);

ALTER TABLE public.deletedjobs
ADD CONSTRAINT pk_deletedjobs_958626458 PRIMARY KEY (jobid);

ALTER TABLE public.expiredjobs
ADD CONSTRAINT pk_expiredjobs_1710629137 PRIMARY KEY (jobid);

ALTER TABLE public.geocodedlocationcache
ADD CONSTRAINT pk_geocodedlocationcache_1669580986 PRIMARY KEY (id);

ALTER TABLE public.impersonationlog
ADD CONSTRAINT pk_impersonationlog_1022626686 PRIMARY KEY (token);

ALTER TABLE public.importedjobsfederal
ADD CONSTRAINT pk_importedjobsfederal_290100074 PRIMARY KEY (jobid);

ALTER TABLE public.importedjobswanted
ADD CONSTRAINT pk_importedjobswanted_1509580416 PRIMARY KEY (jobid);

ALTER TABLE public.industries
ADD CONSTRAINT pk_naicscodes_2146106686 PRIMARY KEY (id);

ALTER TABLE public.jobalerts
ADD CONSTRAINT pk_jobalerts_1106102981 PRIMARY KEY (id);

ALTER TABLE public.jobids
ADD CONSTRAINT pk_jobids_1538104520 PRIMARY KEY (id);

ALTER TABLE public.jobs
ADD CONSTRAINT pk_jobs_802101898 PRIMARY KEY (jobid);

ALTER TABLE public.jobseekeradmincomments
ADD CONSTRAINT pk_jobseekeradmincomments_1874105717 PRIMARY KEY (id);

ALTER TABLE public.jobseekerchangelog
ADD CONSTRAINT pk_jobseekerchangelog_1646628909 PRIMARY KEY (id);

ALTER TABLE public.jobseekereventlog
ADD CONSTRAINT pk_jobseekereventlog_2002106173 PRIMARY KEY (id);

ALTER TABLE public.jobseekerflags
ADD CONSTRAINT pk_jobseekerflags_98099390 PRIMARY KEY (id);

ALTER TABLE public.jobseekerstatlabels
ADD CONSTRAINT pk_jobseekerstatlabels_1438628168 PRIMARY KEY (key);

ALTER TABLE public.jobseekerstats
ADD CONSTRAINT pk_jobseekerstats_1406628054 PRIMARY KEY (weeklyperiodid, labelkey, regionid);

ALTER TABLE public.jobseekerversions
ADD CONSTRAINT pk_jobseekerversions_286624064 PRIMARY KEY (id);

ALTER TABLE public.jobsources
ADD CONSTRAINT pk_jobsources_734625660 PRIMARY KEY (id);

ALTER TABLE public.jobstats
ADD CONSTRAINT pk_jobstats_1342627826 PRIMARY KEY (weeklyperiodid, regionid, jobsourceid);

ALTER TABLE public.jobversions
ADD CONSTRAINT pk_jobversions_46623209 PRIMARY KEY (id);

ALTER TABLE public.jobviews
ADD CONSTRAINT pk_jobviews_786101841 PRIMARY KEY (jobid);

ALTER TABLE public.locations
ADD CONSTRAINT pk_locationlookups_466100701 PRIMARY KEY (locationid);

ALTER TABLE public.noccategories
ADD CONSTRAINT pk_noccategories_2114106572 PRIMARY KEY (categorycode);

ALTER TABLE public.noccategories2021
ADD CONSTRAINT pk_noccategories2021_1131151075 PRIMARY KEY (categorycode);

ALTER TABLE public.noccodes
ADD CONSTRAINT pk_noccodes_1634104862 PRIMARY KEY (id);

ALTER TABLE public.noccodes2021
ADD CONSTRAINT pk_noccodes2021_1035150733 PRIMARY KEY (id);

ALTER TABLE public.provinces
ADD CONSTRAINT pk_provinces_142623551 PRIMARY KEY (provinceid);

ALTER TABLE public.regions
ADD CONSTRAINT pk_regions_174623665 PRIMARY KEY (id);

ALTER TABLE public.reportpersistencecontrol
ADD CONSTRAINT pk_reportpersistencecontrol_1566628624 PRIMARY KEY (weeklyperiodid, tablename);

ALTER TABLE public.savedcareerprofiles
ADD CONSTRAINT pk_savedcareerprofiles_1250103494 PRIMARY KEY (id);

ALTER TABLE public.savedindustryprofiles
ADD CONSTRAINT pk_savedindustryprofiles_1282103608 PRIMARY KEY (id);

ALTER TABLE public.savedjobs
ADD CONSTRAINT pk_savedjobs_498100815 PRIMARY KEY (id);

ALTER TABLE public.securityquestions
ADD CONSTRAINT pk_securityquestions_770101784 PRIMARY KEY (id);

ALTER TABLE public.sysdiagrams
ADD CONSTRAINT pk__sysdiagr__c2b05b619312df6e PRIMARY KEY (diagram_id);

ALTER TABLE public.sysdiagrams
ADD CONSTRAINT uk_principal_name_891150220 UNIQUE (principal_id, name);

ALTER TABLE public.systemsettings
ADD CONSTRAINT pk_systemsettings_1678629023 PRIMARY KEY (name);

ALTER TABLE public.weeklyperiods
ADD CONSTRAINT pk_weeklyperiods_542624976 PRIMARY KEY (id);

-- ------------ Write CREATE-FOREIGN-KEY-CONSTRAINT-stage scripts -----------

ALTER TABLE public.adminusers
ADD CONSTRAINT fk_adminusers_adminusers_lockedbyadminuserid_1346103836 FOREIGN KEY (lockedbyadminuserid)
REFERENCES public.adminusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.adminusers
ADD CONSTRAINT fk_adminusers_adminusers_modifiedbyadminuserid_1378103950 FOREIGN KEY (modifiedbyadminuserid)
REFERENCES public.adminusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.aspnetroleclaims
ADD CONSTRAINT fk_aspnetroleclaims_aspnetroles_roleid_1829581556 FOREIGN KEY (roleid)
REFERENCES public.aspnetroles (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.aspnetuserclaims
ADD CONSTRAINT fk_aspnetuserclaims_aspnetusers_userid_1877581727 FOREIGN KEY (userid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.aspnetuserlogins
ADD CONSTRAINT fk_aspnetuserlogins_aspnetusers_userid_1925581898 FOREIGN KEY (userid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.aspnetuserroles
ADD CONSTRAINT fk_aspnetuserroles_aspnetroles_roleid_1973582069 FOREIGN KEY (roleid)
REFERENCES public.aspnetroles (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.aspnetuserroles
ADD CONSTRAINT fk_aspnetuserroles_aspnetusers_userid_1989582126 FOREIGN KEY (userid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.aspnetusers
ADD CONSTRAINT fk_aspnetusers_adminusers_lockedbyadminuserid_2050106344 FOREIGN KEY (lockedbyadminuserid)
REFERENCES public.adminusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.aspnetusers
ADD CONSTRAINT fk_aspnetusers_countries_countryid_414624520 FOREIGN KEY (countryid)
REFERENCES public.countries (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.aspnetusers
ADD CONSTRAINT fk_aspnetusers_locationlookups_locationid_190623722 FOREIGN KEY (locationid)
REFERENCES public.locations (locationid)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.aspnetusers
ADD CONSTRAINT fk_aspnetusers_provinces_provinceid_206623779 FOREIGN KEY (provinceid)
REFERENCES public.provinces (provinceid)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.aspnetusers
ADD CONSTRAINT fk_aspnetusers_securityquestions_securityquestionid_850102069 FOREIGN KEY (securityquestionid)
REFERENCES public.securityquestions (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.aspnetusertokens
ADD CONSTRAINT fk_aspnetusertokens_aspnetusers_userid_2037582297 FOREIGN KEY (userid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.deletedjobs
ADD CONSTRAINT fk_deletedjobs_adminusers_deletedbyadminuserid_974626515 FOREIGN KEY (deletedbyadminuserid)
REFERENCES public.adminusers (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.deletedjobs
ADD CONSTRAINT fk_deletedjobs_jobs_jobid_990626572 FOREIGN KEY (jobid)
REFERENCES public.jobs (jobid)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.expiredjobs
ADD CONSTRAINT fk_expiredjobs_jobids_jobid_1726629194 FOREIGN KEY (jobid)
REFERENCES public.jobids (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.impersonationlog
ADD CONSTRAINT fk_impersonationlog_adminusers_adminuserid_1038626743 FOREIGN KEY (adminuserid)
REFERENCES public.adminusers (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.impersonationlog
ADD CONSTRAINT fk_impersonationlog_aspnetusers_aspnetuserid_1054626800 FOREIGN KEY (aspnetuserid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.importedjobsfederal
ADD CONSTRAINT fk_importedjobsfederal_jobids_jobid_1570104634 FOREIGN KEY (jobid)
REFERENCES public.jobids (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.importedjobswanted
ADD CONSTRAINT fk_importedjobswanted_jobids_jobid_1586104691 FOREIGN KEY (jobid)
REFERENCES public.jobids (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.jobalerts
ADD CONSTRAINT fk_jobalerts_aspnetusers_aspnetuserid_1122103038 FOREIGN KEY (aspnetuserid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobids
ADD CONSTRAINT fk_jobids_jobsources_jobsourceid_766625774 FOREIGN KEY (jobsourceid)
REFERENCES public.jobsources (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobs
ADD CONSTRAINT fk_jobs_jobids_jobid_1602104748 FOREIGN KEY (jobid)
REFERENCES public.jobids (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.jobs
ADD CONSTRAINT fk_jobs_jobsources_jobsourceid_782625831 FOREIGN KEY (jobsourceid)
REFERENCES public.jobsources (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobs
ADD CONSTRAINT fk_jobs_locationlookups_locationid_222623836 FOREIGN KEY (locationid)
REFERENCES public.locations (locationid)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.jobs
ADD CONSTRAINT fk_jobs_naicscodes_naicsid_14623095 FOREIGN KEY (industryid)
REFERENCES public.industries (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobs
ADD CONSTRAINT fk_jobs_noccodes2021_noccodeid2021_1051150790 FOREIGN KEY (noccodeid2021)
REFERENCES public.noccodes2021 (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobs
ADD CONSTRAINT fk_jobs_noccodes_noccodeid_1650104919 FOREIGN KEY (noccodeid)
REFERENCES public.noccodes (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobseekeradmincomments
ADD CONSTRAINT fk_jobseekeradmincomments_adminusers_enteredbyadminuserid_1906105831 FOREIGN KEY (enteredbyadminuserid)
REFERENCES public.adminusers (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.jobseekeradmincomments
ADD CONSTRAINT fk_jobseekeradmincomments_aspnetusers_aspnetuserid_1890105774 FOREIGN KEY (aspnetuserid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobseekerchangelog
ADD CONSTRAINT fk_jobseekeradminlog_aspnetusers_aspnetuserid_1662628966 FOREIGN KEY (aspnetuserid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobseekerchangelog
ADD CONSTRAINT fk_jobseekerchangelog_adminusers_modifiedbyadminuserid_1630628852 FOREIGN KEY (modifiedbyadminuserid)
REFERENCES public.adminusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobseekereventlog
ADD CONSTRAINT fk_jobseekereventlog_aspnetusers_aspnetuserid_2018106230 FOREIGN KEY (aspnetuserid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobseekerflags
ADD CONSTRAINT fk_jobseekerflags_aspnetusers_aspnetuserid_178099675 FOREIGN KEY (aspnetuserid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobseekerstats
ADD CONSTRAINT fk_jobseekerstats_jobseekerstatlabels_labelkey_1454628225 FOREIGN KEY (labelkey)
REFERENCES public.jobseekerstatlabels (key)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.jobseekerstats
ADD CONSTRAINT fk_jobseekerstats_regions_regionid_1502628396 FOREIGN KEY (regionid)
REFERENCES public.regions (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.jobseekerstats
ADD CONSTRAINT fk_jobseekerstats_weeklyperiods_weeklyperiodid_1262627541 FOREIGN KEY (weeklyperiodid)
REFERENCES public.weeklyperiods (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.jobseekerversions
ADD CONSTRAINT fk_jobseekerversions_aspnetusers_aspnetuserid_302624121 FOREIGN KEY (aspnetuserid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobseekerversions
ADD CONSTRAINT fk_jobseekerversions_countries_countryid_430624577 FOREIGN KEY (countryid)
REFERENCES public.countries (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobseekerversions
ADD CONSTRAINT fk_jobseekerversions_locations_locationid_366624349 FOREIGN KEY (locationid)
REFERENCES public.locations (locationid)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobseekerversions
ADD CONSTRAINT fk_jobseekerversions_provinces_provinceid_350624292 FOREIGN KEY (provinceid)
REFERENCES public.provinces (provinceid)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobstats
ADD CONSTRAINT fk_jobstats_jobsources_jobsourceid_1358627883 FOREIGN KEY (jobsourceid)
REFERENCES public.jobsources (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.jobstats
ADD CONSTRAINT fk_jobstats_regions_regionid_1518628453 FOREIGN KEY (regionid)
REFERENCES public.regions (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.jobstats
ADD CONSTRAINT fk_jobstats_weeklyperiods_weeklyperiodid_1374627940 FOREIGN KEY (weeklyperiodid)
REFERENCES public.weeklyperiods (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.jobversions
ADD CONSTRAINT fk_jobversions_jobs_jobid_62623266 FOREIGN KEY (jobid)
REFERENCES public.jobs (jobid)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobversions
ADD CONSTRAINT fk_jobversions_locationlookups_locationid_238623893 FOREIGN KEY (locationid)
REFERENCES public.locations (locationid)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.jobversions
ADD CONSTRAINT fk_jobversions_naicscodes_naicsid_94623380 FOREIGN KEY (industryid)
REFERENCES public.industries (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobversions
ADD CONSTRAINT fk_jobversions_noccodes2021_noccodeid2021_1067150847 FOREIGN KEY (noccodeid2021)
REFERENCES public.noccodes2021 (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobversions
ADD CONSTRAINT fk_jobversions_noccodes_noccodeid_110623437 FOREIGN KEY (noccodeid)
REFERENCES public.noccodes (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.jobviews
ADD CONSTRAINT fk_jobviews_jobs_jobid_1442104178 FOREIGN KEY (jobid)
REFERENCES public.jobs (jobid)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.locations
ADD CONSTRAINT fk_locationlookups_regions_regionid_254623950 FOREIGN KEY (regionid)
REFERENCES public.regions (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.locations
ADD CONSTRAINT fk_locations_regions_regionid_446624634 FOREIGN KEY (regionid)
REFERENCES public.regions (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.reportpersistencecontrol
ADD CONSTRAINT fk_reportpersistencecontrol_weeklyperiods_weeklyperiodid_1150627142 FOREIGN KEY (weeklyperiodid)
REFERENCES public.weeklyperiods (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.savedcareerprofiles
ADD CONSTRAINT fk_savedcareerprofiles_aspnetusers_aspnetuserid_1458104235 FOREIGN KEY (aspnetuserid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.savedcareerprofiles
ADD CONSTRAINT fk_savedcareerprofiles_noccodes2021_id_1083150904 FOREIGN KEY (noccodeid2021)
REFERENCES public.noccodes2021 (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.savedindustryprofiles
ADD CONSTRAINT fk_savedindustryprofiles_aspnetusers_aspnetuserid_1474104292 FOREIGN KEY (aspnetuserid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.savedindustryprofiles
ADD CONSTRAINT fk_savedindustryprofiles_industries_id_1099150961 FOREIGN KEY (industryid)
REFERENCES public.industries (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.savedjobs
ADD CONSTRAINT fk_savedjobs_aspnetusers_aspnetuserid_514100872 FOREIGN KEY (aspnetuserid)
REFERENCES public.aspnetusers (id)
ON UPDATE NO ACTION
ON DELETE NO ACTION;

ALTER TABLE public.savedjobs
ADD CONSTRAINT fk_savedjobs_jobs_jobid_1362103893 FOREIGN KEY (jobid)
REFERENCES public.jobs (jobid)
ON UPDATE NO ACTION
ON DELETE CASCADE;

ALTER TABLE public.systemsettings
ADD CONSTRAINT fk_systemsettings_adminusers_modifiedbyadminuserid_1490104349 FOREIGN KEY (modifiedbyadminuserid)
REFERENCES public.adminusers (id)
ON UPDATE NO ACTION
ON DELETE CASCADE;

-- ------------ Write CREATE-FUNCTION-stage scripts -----------

CREATE OR REPLACE FUNCTION public.tvf_getjobseekersfordate(IN par_enddateplus1 TIMESTAMP WITHOUT TIME ZONE)
RETURNS TABLE (aspnetuserid VARCHAR, email VARCHAR, locationid INTEGER, provinceid INTEGER, countryid INTEGER, dateregistered TIMESTAMP WITHOUT TIME ZONE, accountstatus SMALLINT, emailconfirmed NUMERIC, isapprentice NUMERIC, isindigenousperson NUMERIC, ismatureworker NUMERIC, isnewimmigrant NUMERIC, ispersonwithdisability NUMERIC, isstudent NUMERIC, isveteran NUMERIC, isvisibleminority NUMERIC, isyouth NUMERIC)
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
        aspnetuserid, MAX(versionnumber) AS versionnumber
        FROM public.jobseekerversions
        WHERE dateversionstart < par_EndDatePlus1 AND (dateversionend IS NULL OR dateversionend >= par_EndDatePlus1)
        GROUP BY aspnetuserid)
    SELECT
        js.aspnetuserid, js.email, js.locationid, js.provinceid, js.countryid, js.dateregistered, js.accountstatus, js.emailconfirmed, js.isapprentice, js.isindigenousperson, js.ismatureworker, js.isnewimmigrant, js.ispersonwithdisability, js.isstudent, js.isveteran, js.isvisibleminority, js.isyouth
        FROM public.jobseekerversions AS js
        INNER JOIN periodversion AS pv
            ON LOWER(pv.aspnetuserid) = LOWER(js.aspnetuserid) AND pv.versionnumber = js.versionnumber
        WHERE par_EndDatePlus1 < clock_timestamp()
    UNION
    SELECT
        js.id AS aspnetuserid, js.email, js.locationid, js.provinceid, js.countryid, js.dateregistered, js.accountstatus, js.emailconfirmed, jsf.isapprentice, jsf.isindigenousperson, jsf.ismatureworker, jsf.isnewimmigrant, jsf.ispersonwithdisability, jsf.isstudent, jsf.isveteran, jsf.isvisibleminority, jsf.isyouth
        FROM public.aspnetusers AS js
        LEFT OUTER JOIN public.jobseekerflags AS jsf
            ON LOWER(jsf.aspnetuserid) = LOWER(js.id)
        WHERE par_EndDatePlus1 >= clock_timestamp();
END;
$BODY$
LANGUAGE  plpgsql;

CREATE OR REPLACE FUNCTION public.tvf_getjobsfordate(IN par_enddateplus1 TIMESTAMP WITHOUT TIME ZONE)
RETURNS TABLE (jobid BIGINT, jobsourceid SMALLINT, locationid INTEGER, noccodeid2021 INTEGER, industryid SMALLINT, datefirstimported TIMESTAMP WITHOUT TIME ZONE, positionsavailable SMALLINT)
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
    WITH periodversion (jobid, versionnumber)
    AS (SELECT
        jobversions.jobid, MAX(jobversions.versionnumber) AS versionnumber
        FROM public.jobversions
        WHERE jobversions.dateversionstart < par_EndDatePlus1 AND (jobversions.dateversionend IS NULL OR jobversions.dateversionend >= par_EndDatePlus1)
        GROUP BY jobversions.jobid)
    SELECT
        jv.jobid, jv.jobsourceid, jv.locationid, jv.noccodeid2021, jv.industryid, jv.datefirstimported, jv.positionsavailable
        FROM public.jobversions AS jv
        INNER JOIN periodversion AS pv
            ON pv.jobid = jv.jobid AND pv.versionnumber = jv.versionnumber;
END;
$BODY$
LANGUAGE  plpgsql;

-- ------------ Write CREATE-PROCEDURE-stage scripts -----------

CREATE OR REPLACE PROCEDURE public.usp_generatejobseekerstats(IN par_weekenddate TIMESTAMP WITHOUT TIME ZONE, INOUT return_code int DEFAULT 0)
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
        weekstartdate, id
        INTO var_StartDate, var_PeriodId
        FROM public.weeklyperiods
        WHERE weekenddate = par_WeekEndDate;
    var_EndDatePlus1 := par_WeekEndDate + (1::NUMERIC || ' DAY')::INTERVAL;
    DELETE FROM public.reportpersistencecontrol
        WHERE LOWER(tablename) = LOWER(var_TableName) AND weeklyperiodid = var_PeriodId;
    /* also delete associated record from JobSeekerStats */
    DELETE FROM public.jobseekerstats
        WHERE weeklyperiodid = var_PeriodId;

    BEGIN
        IF NOT EXISTS (SELECT
            *
            FROM public.reportpersistencecontrol
            WHERE weeklyperiodid = var_PeriodId AND LOWER(tablename) = LOWER(var_TableName)) THEN
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
                    FROM public.tvf_getjobseekersfordate(var_EndDatePlus1)
                        AS js
                    LEFT OUTER JOIN public.locations AS l
                        ON l.locationid = js.locationid;
            ELSE
                INSERT INTO jobseekerdata$usp_generatejobseekerstats
                SELECT
                    js.id AS aspnetuserid, (CASE
                        WHEN regionid IS NOT NULL THEN regionid
                        WHEN countryid = 37 AND provinceid <> 2 THEN - 1
                        WHEN (countryid IS NOT NULL AND countryid <> 37) THEN - 2
                        ELSE 0
                    END) AS regionid, dateregistered, accountstatus, emailconfirmed, isapprentice, isindigenousperson, ismatureworker, isnewimmigrant, ispersonwithdisability, isstudent, isveteran, isvisibleminority, isyouth
                    FROM public.aspnetusers AS js
                    LEFT OUTER JOIN public.jobseekerflags AS jf
                        ON LOWER(jf.aspnetuserid) = LOWER(js.id)
                    LEFT OUTER JOIN public.locations AS l
                        ON l.locationid = js.locationid;
            END IF;
            /* insert a record into ReportPersistenceControl */
            INSERT INTO public.reportpersistencecontrol (weeklyperiodid, tablename, datecalculated, istotaltodate)
            SELECT
                var_PeriodId AS weeklyperiodid, var_TableName AS report, clock_timestamp() AS datecalculated, (CASE
                    WHEN var_EndDatePlus1 > clock_timestamp() THEN 1
                    ELSE 0
                END) AS istotaltodate;
            /* ACCOUNTS BY STATUS */
            /* New Registrations */
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'REGD', regionid, COUNT(*)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE dateregistered >= var_StartDate AND dateregistered < var_EndDatePlus1
                GROUP BY regionid;
            /* Awaiting Email Activation: This is the total at the end of the selected period. */
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'CFEM', regionid, COUNT(DISTINCT je.aspnetuserid)
                FROM public.jobseekereventlog AS je
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON LOWER(jd.aspnetuserid) = LOWER(je.aspnetuserid)
                WHERE eventtypeid = 3 AND datelogged >= var_StartDate AND datelogged < var_EndDatePlus1
                GROUP BY jd.regionid;
            /* Get the net number of new unactivated accounts for the period */
            /* by subtracting new email confirmations from new registrations. */
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                weeklyperiodid, 'NOAC' AS labelkey, regionid, SUM(value) AS value
                FROM (SELECT
                    weeklyperiodid, labelkey, regionid, value
                    FROM public.jobseekerstats
                    WHERE weeklyperiodid = var_PeriodId AND LOWER(labelkey) = LOWER('REGD')
                UNION
                SELECT
                    weeklyperiodid, labelkey, regionid, - 1 * value
                    FROM public.jobseekerstats
                    WHERE weeklyperiodid = var_PeriodId AND LOWER(labelkey) = LOWER('CFEM')) AS regd_cfem
                GROUP BY weeklyperiodid, regionid;
            /* Get statistics from the JobSeekerEventLog */
            /* Deactivated: This is accounts deactivated for this period. */
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'DEAC', regionid, COUNT(DISTINCT je.aspnetuserid)
                FROM public.jobseekereventlog AS je
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON LOWER(jd.aspnetuserid) = LOWER(je.aspnetuserid)
                WHERE eventtypeid = 4 AND datelogged >= var_StartDate AND datelogged < var_EndDatePlus1
                GROUP BY jd.regionid;
            /* Deleted: This is total account deleted for this period. */
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'DEL', regionid, COUNT(DISTINCT je.aspnetuserid)
                FROM public.jobseekereventlog AS je
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON LOWER(jd.aspnetuserid) = LOWER(je.aspnetuserid)
                WHERE eventtypeid = 6 AND datelogged >= var_StartDate AND datelogged < var_EndDatePlus1
                GROUP BY jd.regionid;
            /* JOB SEEKER EMPLOYMENT GROUPS */
            /* Employment Groups: Total number of accounts */
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'APPR', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isapprentice = 1
                GROUP BY regionid;
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'INDP', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isindigenousperson = 1
                GROUP BY regionid;
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'MAT', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE ismatureworker = 1
                GROUP BY regionid;
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'IMMG', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isnewimmigrant = 1
                GROUP BY regionid;
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'PWD', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE ispersonwithdisability = 1
                GROUP BY regionid;
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'STUD', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isstudent = 1
                GROUP BY regionid;
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'VET', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isveteran = 1
                GROUP BY regionid;
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'VMIN', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isvisibleminority = 1
                GROUP BY regionid;
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'YTH', regionid, COUNT(DISTINCT aspnetuserid)
                FROM jobseekerdata$usp_generatejobseekerstats
                WHERE isyouth = 1
                GROUP BY regionid;
            /* ACCOUNT ACTIVITY */
            /* Get statistics from the JobSeekerEventLog */
            /* Logins: This is total number of times users successfully logged in for this period. */
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'LOGN', regionid, COUNT(je.aspnetuserid)
                FROM public.jobseekereventlog AS je
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON LOWER(jd.aspnetuserid) = LOWER(je.aspnetuserid)
                WHERE eventtypeid = 1 AND datelogged >= var_StartDate AND datelogged < var_EndDatePlus1
                GROUP BY jd.regionid;
            /* Job Seekers with Job Alerts, Job Seekers with Saved Career Profiles: */
            /* These are total number of accounts, not new registrations. */
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'ALRT', regionid, COUNT(DISTINCT ja.aspnetuserid)
                FROM public.jobalerts AS ja
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON LOWER(jd.aspnetuserid) = LOWER(ja.aspnetuserid)
                WHERE datecreated < var_EndDatePlus1 AND (isdeleted = 0 OR datedeleted >= var_EndDatePlus1)
                GROUP BY jd.regionid;
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'CAPR', jd.regionid, COUNT(DISTINCT sc.aspnetuserid)
                FROM public.savedcareerprofiles AS sc
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON LOWER(jd.aspnetuserid) = LOWER(sc.aspnetuserid)
                WHERE datesaved < var_EndDatePlus1 AND (isdeleted = 0 OR datedeleted >= var_EndDatePlus1)
                GROUP BY jd.regionid;
            INSERT INTO public.jobseekerstats (weeklyperiodid, labelkey, regionid, value)
            SELECT
                var_PeriodId, 'INPR', regionid, COUNT(DISTINCT si.aspnetuserid)
                FROM public.savedindustryprofiles AS si
                INNER JOIN jobseekerdata$usp_generatejobseekerstats AS jd
                    ON LOWER(jd.aspnetuserid) = LOWER(si.aspnetuserid)
                WHERE datesaved < var_EndDatePlus1 AND (isdeleted = 0 OR datedeleted >= var_EndDatePlus1)
                GROUP BY jd.regionid;
            DROP TABLE IF EXISTS jobseekerdata$usp_generatejobseekerstats;
        END IF;
        EXCEPTION
            WHEN OTHERS THEN
                DELETE FROM public.reportpersistencecontrol
                    WHERE LOWER(tablename) = LOWER(var_TableName) AND weeklyperiodid = var_PeriodId;
                /* also delete associated record from JobSeekerStats */
                DELETE FROM public.jobseekerstats
                    WHERE weeklyperiodid = var_PeriodId;
                return_code := - 1;
                RETURN;
    END;
    return_code := 0;
    RETURN;
END;
$BODY$
LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE public.usp_generatejobstats(IN par_weekenddate TIMESTAMP WITHOUT TIME ZONE, INOUT return_code int DEFAULT 0)
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
        weekstartdate, id
        INTO var_StartDate, var_PeriodId
        FROM public.weeklyperiods
        WHERE weekenddate = par_WeekEndDate;
    var_EndDatePlus1 := par_WeekEndDate + (1::NUMERIC || ' DAY')::INTERVAL;
    /* - Check if a ReportPersistenceControl record exists.  Delete if it is a TotalToDate record */

    IF EXISTS (SELECT
        *
        FROM public.reportpersistencecontrol
        WHERE LOWER(tablename) = LOWER(var_TableName) AND weeklyperiodid = var_PeriodId AND datecalculated < par_WeekEndDate + (48::NUMERIC || ' HOUR')::INTERVAL) THEN
        DELETE FROM public.reportpersistencecontrol
            WHERE LOWER(tablename) = LOWER(var_TableName) AND weeklyperiodid = var_PeriodId;
        /* also delete associated record from JobStats */
        DELETE FROM public.jobstats
            WHERE weeklyperiodid = var_PeriodId;
    END IF;

    BEGIN
        IF NOT EXISTS (SELECT
            *
            FROM public.reportpersistencecontrol
            WHERE weeklyperiodid = var_PeriodId AND LOWER(tablename) = LOWER(var_TableName)) THEN
            /* insert a record into ReportPersistenceControl */
            INSERT INTO public.reportpersistencecontrol (weeklyperiodid, tablename, datecalculated, istotaltodate)
            SELECT
                var_PeriodId AS weeklyperiodid, var_TableName AS tablename, clock_timestamp() AS datecalculated, (CASE
                    WHEN par_WeekEndDate + (48::NUMERIC || ' HOUR')::INTERVAL > clock_timestamp() THEN 1
                    ELSE 0
                END) AS istotaltodate;
            /* insert records into JobStats */
            INSERT INTO public.jobstats (weeklyperiodid, jobsourceid, regionid, jobpostings, positionsavailable)
            SELECT
                var_PeriodId AS weeklyperiodid, j.jobsourceid, COALESCE(l.regionid, 0) AS regionid, COUNT(*) AS jobpostings, SUM(positionsavailable) AS positionsavailable
                FROM public.tvf_getjobsfordate(var_EndDatePlus1)
                    AS j
                LEFT OUTER JOIN public.locations AS l
                    ON l.locationid = j.locationid
                WHERE j.datefirstimported >= var_StartDate AND j.datefirstimported < var_EndDatePlus1
                GROUP BY j.jobsourceid, l.regionid;
        END IF;
        EXCEPTION
            WHEN OTHERS THEN
                /* if an error occurs, undo any inserts */
                DELETE FROM public.reportpersistencecontrol
                    WHERE LOWER(tablename) = LOWER(var_TableName) AND weeklyperiodid = var_PeriodId;
                /* also delete associated record from JobStats */
                DELETE FROM public.jobstats
                    WHERE weeklyperiodid = var_PeriodId;
                return_code := - 1;
                RETURN;
    END;
    return_code := 0;
    RETURN;
END;
$BODY$
LANGUAGE plpgsql;

