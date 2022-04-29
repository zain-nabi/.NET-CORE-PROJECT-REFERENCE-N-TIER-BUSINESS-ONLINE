CREATE TABLE [dbo].[FuelSurchargeClassAudits](
	[FuelSurchargeClassAuditID] [int] IDENTITY(1,1) NOT NULL,
	[FuelSurchargeClassID] [int] NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[CurrentValue] [decimal](18, 2) NULL,
	[MinumumValue] [decimal](18, 2) NULL,
	[MonthValid] [date] NULL,
 CONSTRAINT [PK_FuelSurchargeClassAudits] PRIMARY KEY CLUSTERED 
(
	[FuelSurchargeClassAuditID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


