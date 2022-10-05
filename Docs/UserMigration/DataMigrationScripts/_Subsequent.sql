-- Copy JobBankUserImport into a third table called JobBankUserImport3
select * 
into JobBankUserImport3
from JobBankUserImport

-- get a list of previously imported accounts that have been modified in the old environment since the last import

-- Diff the JobBankUserImport and JobBankUserImport2 tables using SQL queries 
select j1.LegacyWebUserId 
into #modifiedusers
from JobBankUserImport j1
inner join JobBankUserImport2 j2 on j1.LegacyWebUserId = j2.LegacyWebUserId 
where j1.Email <> j2.Email
or j1.Password <> j2.Password
or j1.AccountStatus <> j2.AccountStatus
or j1.VerificationGuid <> j2.VerificationGuid
or j1.LastModified <> j2.LastModified
or j1.DateCreated <> j2.DateCreated
or j1.FirstName <> j2.FirstName
or j1.LastName <> j2.LastName
or j1.LastLogon <> j2.LastLogon
or j1.LocationId <> j2.LocationId
or j1.City <> j2.City
or j1.CountryId <> j2.CountryId
or j1.ProvinceId <> j2.ProvinceId
or j1.IsIndigenousPerson <> j2.IsIndigenousPerson
or j1.IsPersonWithDisability <> j2.IsPersonWithDisability
or j1.IsMatureWorker <> j2.IsMatureWorker
or j1.IsReservist <> j2.IsReservist
or j1.IsApprentice <> j2.IsApprentice


-- Delete users from AspNetUsers, JobSeekerFlags and JobSeekerVersions, etc that have changed

delete from JobSeekerVersions Where AspNetUserId in (
	select id from AspNetUsers where LegacyWebUserId in (
		select LegacyWebUserId from #modifiedusers
	)
)

delete from JobSeekerFlags Where AspNetUserId in (
	select id from AspNetUsers where LegacyWebUserId in (
		select LegacyWebUserId from #modifiedusers
	)
)

delete from ImpersonationLog Where AspNetUserId in (
	select id from AspNetUsers where LegacyWebUserId in (
		select LegacyWebUserId from #modifiedusers
	)
)

delete from JobAlerts Where AspNetUserId in (
	select id from AspNetUsers where LegacyWebUserId in (
		select LegacyWebUserId from #modifiedusers
	)
)

delete from JobSeekerAdminComments Where AspNetUserId in (
	select id from AspNetUsers where LegacyWebUserId in (
		select LegacyWebUserId from #modifiedusers
	)
)

delete from JobSeekerChangeLog Where AspNetUserId in (
	select id from AspNetUsers where LegacyWebUserId in (
		select LegacyWebUserId from #modifiedusers
	)
)

delete from JobSeekerEventLog Where AspNetUserId in (
	select id from AspNetUsers where LegacyWebUserId in (
		select LegacyWebUserId from #modifiedusers
	)
)

delete from SavedCareerProfiles Where AspNetUserId in (
	select id from AspNetUsers where LegacyWebUserId in (
		select LegacyWebUserId from #modifiedusers
	)
)

delete from SavedIndustryProfiles Where AspNetUserId in (
	select id from AspNetUsers where LegacyWebUserId in (
		select LegacyWebUserId from #modifiedusers
	)
)

delete from SavedJobs Where AspNetUserId in (
	select id from AspNetUsers where LegacyWebUserId in (
		select LegacyWebUserId from #modifiedusers
	)
)

delete from AspNetUsers where LegacyWebUserId in (
	select LegacyWebUserId from #modifiedusers
)


-- Delete records from JobBankUserImport that have not changed (keeping any new records as well)
select LegacyWebUserId
into #newandmodifiedusers
from JobBankUserImport 
where LegacyWebUserId not in (
	select LegacyWebUserId from JobBankUserImport2
) or LegacyWebUserId in (
	select LegacyWebUserId from #modifiedusers
)

delete from JobBankUserImport
where LegacyWebUserId not in (select LegacyWebUserId from #newandmodifiedusers)


drop table #modifiedusers
drop table #newandmodifiedusers