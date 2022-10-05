-- DEV
insert into WorkBC_JobBoard_Dev.dbo.SavedIndustryProfiles
select ip.IndustryProfileID, u.id AS AspNetUserId, 
	sip.date_saved AS DateSaved, NULL as DateDeleted, 0 as IsDeleted 
from WorkBC_JobBank_DEV.dbo.user_saved_industry_profiles sip
inner join WorkBC_Enterprise_DEV.dbo.EDM_IndustryProfile ip
on ip.NAICS_ID = sip.naics_code
inner join WorkBC_JobBoard_Dev.dbo.AspNetUsers u on u.LegacyWebUserId = sip.web_user_id

-- TEST 
insert into WorkBC_JobBoard_TEST.dbo.SavedIndustryProfiles
select ip.IndustryProfileID, u.id AS AspNetUserId, 
	sip.date_saved AS DateSaved, NULL as DateDeleted, 0 as IsDeleted 
from WorkBC_JobBank_TEST.dbo.user_saved_industry_profiles sip
inner join WorkBC_Enterprise_TEST.dbo.EDM_IndustryProfile ip
on ip.NAICS_ID = sip.naics_code
inner join WorkBC_JobBoard_TEST.dbo.AspNetUsers u on u.LegacyWebUserId = sip.web_user_id
