-- JobSeekerFlags.IX_JobSeekerFlags_AspNetUserId --
USE [WorkBC_JobBoard]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_JobSeekerFlags_AspNetUserId]    Script Date: 10/9/2024 2:18:08 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_JobSeekerFlags_AspNetUserId] ON [dbo].[JobSeekerFlags]
(
	[AspNetUserId] ASC
)
WHERE ([AspNetUserId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- JobSeekerVersions.IX_JobSeekerVersions_AspNetUserId_VersionNumber --
USE [WorkBC_JobBoard]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_JobSeekerVersions_AspNetUserId_VersionNumber]    Script Date: 10/9/2024 2:19:25 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_JobSeekerVersions_AspNetUserId_VersionNumber] ON [dbo].[JobSeekerVersions]
(
	[AspNetUserId] ASC,
	[VersionNumber] ASC
)
WHERE ([AspNetUserId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- GeocodedLocationCache.IX_GeocodedLocationCache_Name --
USE [WorkBC_JobBoard]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_GeocodedLocationCache_Name]    Script Date: 10/9/2024 2:20:10 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_GeocodedLocationCache_Name] ON [dbo].[GeocodedLocationCache]
(
	[Name] ASC
)
WHERE ([Name] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- AspNetRoles.RoleNameIndex --
USE [WorkBC_JobBoard]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [RoleNameIndex]    Script Date: 10/9/2024 2:21:07 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- AspNetUsers.UserNameIndex --
USE [WorkBC_JobBoard]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [UserNameIndex]    Script Date: 10/9/2024 2:21:39 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

-- AspNetUsers.IX_AspNetUsers_Email --
USE [WorkBC_JobBoard]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_AspNetUsers_Email]    Script Date: 10/9/2024 2:22:25 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetUsers_Email] ON [dbo].[AspNetUsers]
(
	[Email] ASC
)
INCLUDE([LastName],[FirstName],[AccountStatus])
WHERE ([Email] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
