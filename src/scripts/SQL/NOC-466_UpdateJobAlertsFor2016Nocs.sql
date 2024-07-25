--Create a temp table for all Job alerts with NOC filters.
SELECT * 
INTO #TempJobAlerts 
FROM [dbo].[JobAlerts]
where UrlParameters like '%noc%' and isdeleted = '0' and JobSearchFilters like '%SearchNocField%'  

--Add NOC2016 column in the temp table. 
Alter table #TempJobAlerts
Add NOC2016 varchar(10);

--Add NOC2021 column in the temp table. 
Alter table #TempJobAlerts
Add NOC2021 varchar(10);

--Extract the noc value from UrlParameters field.
Update #TempJobAlerts
Set NOC2016 = SUBSTRING(UrlParameters, 6, 5)
From #TempJobAlerts

--Delete the 5 digit NOCs from the temp table.
Delete from #TempJobAlerts
where NOC2016 not like '%;%'

--Extract the NOC 2016 value from UrlParameters field.
Update #TempJobAlerts
Set NOC2016 = LEFT(NOC2016,charindex(';',NOC2016)-1)
From #TempJobAlerts

--Find the NOC 2021 value for the NOC 2016 value.
Update #TempJobAlerts
Set NOC2021 = n.Code
From #TempJobAlerts t JOIN NocCodes2021 n
ON n.Code2016 like '%' + t.NOC2016 + '%'

--Update UrlParameters and JobSearchFilters fields in temp table.
Update #TempJobAlerts
Set UrlParameters = REPLACE(UrlParameters, NOC2016, NOC2021);

Update #TempJobAlerts
Set JobSearchFilters = REPLACE(JobSearchFilters, NOC2016, NOC2021);

--Replace UrlParameters and JobSearchFilters fields in JobAlerts table.
Update JobAlerts
Set UrlParameters = t.UrlParameters, JobSearchFilters= t.JobSearchFilters
from #TempJobAlerts t
where JobAlerts.Id = t.Id

-----Drop the temp table-----
Drop table #TempJobAlerts

Select *
FROM [dbo].[JobAlerts]
where UrlParameters like '%noc%' and isdeleted = '0' and JobSearchFilters like '%SearchNocField%'

