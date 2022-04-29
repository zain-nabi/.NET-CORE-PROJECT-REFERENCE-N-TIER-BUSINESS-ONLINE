CREATE TABLE [dbo].[FuelSurchargeClasss](
	[FuelSurchargeClassID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[CurrentValue] [decimal](18, 2) NULL,
	[MinumumValue] [decimal](18, 2) NULL,
	[IntegerCents] [int] NULL,
	[IntegerPerc] [decimal](18, 3) NULL,
 CONSTRAINT [PK_FuelSurchargeClasss] PRIMARY KEY CLUSTERED 
(
	[FuelSurchargeClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[FuelSurchargeClasss] ON 
GO
INSERT [dbo].[FuelSurchargeClasss] ([FuelSurchargeClassID], [Code], [Description], [CurrentValue], [MinumumValue], [IntegerCents], [IntegerPerc]) VALUES (2, N'STD', N'Standard', CAST(19.00 AS Decimal(18, 2)), CAST(19.00 AS Decimal(18, 2)), 26, CAST(0.010 AS Decimal(18, 3)))
GO
INSERT [dbo].[FuelSurchargeClasss] ([FuelSurchargeClassID], [Code], [Description], [CurrentValue], [MinumumValue], [IntegerCents], [IntegerPerc]) VALUES (3, N'SW', N'Swaziland', CAST(21.00 AS Decimal(18, 2)), CAST(21.00 AS Decimal(18, 2)), 22, CAST(0.010 AS Decimal(18, 3)))
GO
INSERT [dbo].[FuelSurchargeClasss] ([FuelSurchargeClassID], [Code], [Description], [CurrentValue], [MinumumValue], [IntegerCents], [IntegerPerc]) VALUES (4, N'MM', N'MisterMover', CAST(46.00 AS Decimal(18, 2)), CAST(38.00 AS Decimal(18, 2)), 22, CAST(0.010 AS Decimal(18, 3)))
GO
INSERT [dbo].[FuelSurchargeClasss] ([FuelSurchargeClassID], [Code], [Description], [CurrentValue], [MinumumValue], [IntegerCents], [IntegerPerc]) VALUES (5, N'MMU', N'MisterMoverUnique', NULL, CAST(38.00 AS Decimal(18, 2)), 10, CAST(0.005 AS Decimal(18, 3)))
GO
INSERT [dbo].[FuelSurchargeClasss] ([FuelSurchargeClassID], [Code], [Description], [CurrentValue], [MinumumValue], [IntegerCents], [IntegerPerc]) VALUES (6, N'SPE', N'Special', NULL, CAST(18.00 AS Decimal(18, 2)), 26, CAST(0.010 AS Decimal(18, 3)))
GO
SET IDENTITY_INSERT [dbo].[FuelSurchargeClasss] OFF
GO
