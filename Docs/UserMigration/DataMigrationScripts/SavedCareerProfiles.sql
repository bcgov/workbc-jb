-- DEV
insert into WorkBC_JobBoard_Dev.dbo.SavedCareerProfiles
select max(cp.CareerProfileID) as EDM_CareerProfile_CareerProfileID, 
	u.id as AspNetUserId, scp.date_saved AS DateSaved, NULL as DateDeleted, 0 as IsDeleted 
from workbc_jobbank_dev.dbo.user_saved_career_profiles scp
inner join WorkBC_Enterprise_DEV.dbo.EDM_NOC n on scp.noc_code = n.NOCCode 
inner join workbc_enterprise_dev.[dbo].[EDM_CareerProfile] cp on cp.NOC_ID = n.NOC_ID 
inner join WorkBC_JobBoard_Dev.dbo.AspNetUsers u on u.LegacyWebUserId = scp.web_user_id
group by u.id, scp.date_saved

-- TEST 
insert into WorkBC_JobBoard_TEST.dbo.SavedCareerProfiles
select max(cp.CareerProfileID) as EDM_CareerProfile_CareerProfileID, 
	u.id as AspNetUserId, scp.date_saved AS DateSaved, NULL as DateDeleted, 0 as IsDeleted 
from workbc_jobbank_TEST.dbo.user_saved_career_profiles scp
inner join WorkBC_Enterprise_TEST.dbo.EDM_NOC n on scp.noc_code = n.NOCCode 
inner join workbc_enterprise_TEST.[dbo].[EDM_CareerProfile] cp on cp.NOC_ID = n.NOC_ID 
inner join WorkBC_JobBoard_TEST.dbo.AspNetUsers u on u.LegacyWebUserId = scp.web_user_id
group by u.id, scp.date_saved