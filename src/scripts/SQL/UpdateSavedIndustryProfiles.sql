--------------------------------------------------------------------------------------------------------------
--Fix 1: For 38 users with duplicate IndustryId = 1, we are converting the seocnd value to IndustryId = 33.
--------------------------------------------------------------------------------------------------------------
Update SavedIndustryProfiles Set IndustryId =33
WHERE Id in (
    SELECT t1.Id
    FROM SavedIndustryProfiles t1, SavedIndustryProfiles t2
    WHERE t1.IndustryId = t2.IndustryId and t1.Id > t2.Id
	and t1.AspNetUserId = t2.AspNetUserId	
	and t1.IsDeleted =0
	and t1.IndustryId =1
	)

---------------------------------------------------------------------------------------------------------------------------------------------------
--Fix 2: For 408 users with single entry of IndustryId = 1 or 33, we are adding a new entry so that they have both the industries in their profile.
---------------------------------------------------------------------------------------------------------------------------------------------------
Insert into SavedIndustryProfiles(AspNetUserId, DateSaved, DateDeleted, IsDeleted, IndustryId)
SELECT t1.AspNetUserId, GETDATE() , NULL, 0, 1
  FROM dbo.SavedIndustryProfiles AS t1
  WHERE t1.IsDeleted =0
  and t1.IndustryId =1
  AND NOT EXISTS
    (
      SELECT 1 FROM dbo.SavedIndustryProfiles AS t2
      WHERE t1.AspNetUserId = t2.AspNetUserId	
      AND t2.IndustryId = 33  
    );

Insert into SavedIndustryProfiles(AspNetUserId, DateSaved, DateDeleted, IsDeleted, IndustryId)
SELECT t1.AspNetUserId, GETDATE() , NULL, 0, 33
  FROM dbo.SavedIndustryProfiles AS t1
  WHERE t1.IsDeleted =0
  and t1.IndustryId = 33
  AND NOT EXISTS
    (
      SELECT 1 FROM dbo.SavedIndustryProfiles AS t2
      WHERE t1.AspNetUserId = t2.AspNetUserId	
      AND t2.IndustryId = 1 
    );
