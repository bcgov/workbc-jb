USE [master]
GO

ALTER DATABASE [WorkBC_JobBoard] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

USE [WorkBC_JobBoard]
GO

DROP INDEX JobSeekerFlags.IX_JobSeekerFlags_AspNetUserId;
DROP INDEX JobSeekerVersions.IX_JobSeekerVersions_AspNetUserId_VersionNumber;
DROP INDEX GeocodedLocationCache.IX_GeocodedLocationCache_Name;
DROP INDEX AspNetRoles.RoleNameIndex;
DROP INDEX AspNetUsers.UserNameIndex;
DROP INDEX AspNetUsers.IX_AspNetUsers_Email;
GO

USE [master]
GO

ALTER DATABASE [WorkBC_JobBoard] COLLATE SQL_Latin1_General_CP1_CS_AS;
GO

ALTER DATABASE [WorkBC_JobBoard] SET MULTI_USER;
GO