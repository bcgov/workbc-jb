---------------------------------------------------------------------------------------------------------------------------------------------------------------------
--Fix 1: For 38 users with duplicate IndustryId = 1, we are converting the seocnd value to IndustryId = 33, provided there is no active record for IndustryId =33.
---------------------------------------------------------------------------------------------------------------------------------------------------------------------
Update SavedIndustryProfiles Set IndustryId =33
WHERE Id in (
    SELECT t1.Id
    FROM SavedIndustryProfiles t1, SavedIndustryProfiles t2
    WHERE t1.IndustryId = t2.IndustryId and t1.Id > t2.Id
	and t1.AspNetUserId = t2.AspNetUserId	
	and t1.IsDeleted =0 and t2.IsDeleted = 0
	and t1.IndustryId =1
	AND NOT EXISTS
    (
      SELECT 1 FROM dbo.SavedIndustryProfiles AS t3
      WHERE t3.AspNetUserId = t2.AspNetUserId	
	  and t3.IsDeleted =0
      and t3.IndustryId = 33  
    )
	)
----------------------------------------------------------------------------------------------------------------------------------------------------------------
--Fix 2: For 408 users with single entry of IndustryId = 1, we are adding a new entry of IndustryId =33 (if missing) so that they have both the industries in their profile.
----------------------------------------------------------------------------------------------------------------------------------------------------------------
Insert into SavedIndustryProfiles(AspNetUserId, DateSaved, DateDeleted, IsDeleted, IndustryId)
SELECT t1.AspNetUserId, GETDATE() , NULL, 0, 33
  FROM dbo.SavedIndustryProfiles AS t1
  WHERE t1.IsDeleted =0
  and t1.IndustryId =1
  AND NOT EXISTS
    (
      SELECT 1 FROM dbo.SavedIndustryProfiles AS t2
      WHERE t1.AspNetUserId = t2.AspNetUserId	
	  AND t2.IsDeleted =0
      AND t2.IndustryId = 33  
    );

--------------------------------------------------------------------------------------------------------------
--Fix 3: For the users with duplicate IndustryId = 1 in active status and a an active IndustryId=33 record.
--------------------------------------------------------------------------------------------------------------

Update SavedIndustryProfiles Set IsDeleted =1, DateDeleted = GETDATE()
WHERE Id in (
    SELECT t2.Id
    FROM SavedIndustryProfiles t1, SavedIndustryProfiles t2
    WHERE t1.IndustryId = t2.IndustryId and t1.Id > t2.Id
	and t1.AspNetUserId = t2.AspNetUserId	
	and t1.IsDeleted =0 and t2.IsDeleted = 0
	and t1.IndustryId =1
	)
--------------------------------------------------------------------------------------------------------------
--Fix 4: For the users with duplicate IndustryId = 33 in active status.
--------------------------------------------------------------------------------------------------------------

Update SavedIndustryProfiles Set IsDeleted =1, DateDeleted = GETDATE()
WHERE Id in (
    SELECT t2.Id
    FROM SavedIndustryProfiles t1, SavedIndustryProfiles t2
    WHERE t1.IndustryId = t2.IndustryId and t1.Id > t2.Id
	and t1.AspNetUserId = t2.AspNetUserId	
	and t1.IsDeleted =0 and t2.IsDeleted = 0
	and t1.IndustryId =33
	)
