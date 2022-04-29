CREATE TABLE [dbo].[UserRoles](
	[UserRoleID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[RoleID] [int] NULL,
	[BranchID] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedByUserID] [int] NULL,
	[DeletedOn] [datetime] NULL,
	[DeletedByUserID] [int] NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[UserRoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[UserRoles] ON 
GO
INSERT [dbo].[UserRoles] ([UserRoleID], [UserID], [RoleID], [BranchID], [CreatedOn], [CreatedByUserID], [DeletedOn], [DeletedByUserID]) VALUES (1, 1, 1, NULL, CAST(N'2020-04-09T13:51:42.733' AS DateTime), 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[UserRoles] OFF
GO
