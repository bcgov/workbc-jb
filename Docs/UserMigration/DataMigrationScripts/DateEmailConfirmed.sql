-- Adds data to the JobSeekerEventLog table to track the approximate date that user's confirmed their email
-- Data is needed for the Job Seeker Account Report.  The query below assumes that all verifird users verified their email 5
-- minutes after they registered.
INSERT INTO JobSeekerEventLog (AspNetUserId, EventTypeId, DateLogged)
SELECT AU.Id AS AspnetUserId, 3 AS EventTypeId, DATEADD(minute,5,DateRegistered) AS DateLogged
FROM AspNetUsers au
	LEFT OUTER JOIN JobSeekerEventLog el ON AU.Id = el.AspNetUserId AND el.EventTypeId = 3
WHERE EmailConfirmed = 1 AND el.AspNetUserId IS NULL

-- Change West Kelowna to Kelowna
update aspnetusers set city = 'Kelowna', LocationId = 1028 Where City = 'West Kelowna'

-- Fill in vancouver people with missing location id
update aspnetusers set LocationId = 2060 Where City = 'Vancouver' AND IsNull(LocationId,0) <> 2060

-- Fill in surrey people with missing location id
update aspnetusers set LocationId = 1917 Where City = 'Surrey' AND IsNull(LocationId,0) <> 1917

-- clean up some frequent errors
update AspnetUsers set City = 'Vancouver' where city = 'Van'
update AspnetUsers set city = 'Tsawwassen' where city = 'Tsawwassen Beach'
update AspnetUsers set city = 'Lake Country' where city = 'Lake Country, District of'
update AspnetUsers set city = 'Burnaby' where city = 'Burnabay'
update AspnetUsers set city = 'Abbotsford' where city = 'abbatsford'

-- Fill in other people with missing location id
update u 
set u.City = l.city, u.LocationId = l.LocationId
from locations l
inner join AspnetUsers u on l.city = u.city 
where u.city <> '' and u.locationId is null AND l.IsHidden = 0

-- clear bad cities
update aspnetusers set city = '' where locationid is null and city <> ''